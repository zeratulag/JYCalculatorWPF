using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JX3PZ.Models;
using JX3PZ.Views.BuildCompare;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Growl = HandyControl.Controls.Growl;

namespace JX3PZ.ViewModels
{
    public class PzBuildCompareWindowViewModel : ObservableObject
    {
        private PzBuildCompareSlotViewModel _selectedSlot;
        private bool _isExporting;
        private bool _isCalculatingAll;

        public event Action RequestClose;

        public string Title { get; }
        public ObservableCollection<PzBuildCompareSlotViewModel> Slots { get; }
        public RelayCommand CalcAllSlotsCmd { get; }
        public RelayCommand ExportCurrentSlotReportCmd { get; }
        public RelayCommand ExportAllSlotReportsCmd { get; }

        public bool IsExporting
        {
            get => _isExporting;
            private set
            {
                if (SetProperty(ref _isExporting, value))
                    RefreshButtonStates();
            }
        }

        public bool IsCalculatingAll
        {
            get => _isCalculatingAll;
            private set
            {
                if (SetProperty(ref _isCalculatingAll, value))
                    RefreshButtonStates();
            }
        }

        public PzBuildCompareSlotViewModel SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                if (SetProperty(ref _selectedSlot, value))
                {
                    _selectedSlot?.EnsureCalculated();
                    RefreshButtonStates();
                }
            }
        }

        private void RefreshButtonStates()
        {
            CalcAllSlotsCmd.NotifyCanExecuteChanged();
            ExportCurrentSlotReportCmd.NotifyCanExecuteChanged();
            ExportAllSlotReportsCmd.NotifyCanExecuteChanged();
        }

        public PzBuildCompareWindowViewModel(PzMainWindowViewModels pzMainVM, PzTabItemViewModel initialSlotVM)
        {
            Title = "装备智能对比";
            CalcAllSlotsCmd = new RelayCommand(CalcAllSlots, CanCalcAllSlots);
            ExportCurrentSlotReportCmd = new RelayCommand(ExportCurrentSlotReport, CanExportCurrentSlotReport);
            ExportAllSlotReportsCmd = new RelayCommand(ExportAllSlotReports, CanExportAllSlotReports);
            Slots = new ObservableCollection<PzBuildCompareSlotViewModel>(
                pzMainVM.Data.Select(slotVM => new PzBuildCompareSlotViewModel(pzMainVM, slotVM)));

            foreach (var slot in Slots)
            {
                slot.CurrentEquipChanged += HandleCurrentEquipChanged;
            }

            SelectedSlot = Slots.FirstOrDefault(slot => slot.Position == initialSlotVM.Position) ?? Slots.FirstOrDefault();
        }

        public PzBuildCompareSlotReport CreateSlotReport()
        {
            return SelectedSlot?.CreateReport();
        }

        public PzBuildCompareSlotReport[] CreateAllSlotReports()
        {
            return Slots.Select(slot => slot.CreateReport()).ToArray();
        }

        private async void ExportCurrentSlotReport()
        {
            var slot = SelectedSlot;
            if (slot == null)
            {
                return;
            }

            var path = SelectExportPath(PzBuildCompareReportExporter.CreateDefaultFileName(slot.Header));
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            IsExporting = true;
            try
            {
                await slot.EnsureCalculatedAsync();
                var report = slot.CreateReport();
                await Task.Run(() => PzBuildCompareReportExporter.ExportSlot(path, report));
                NotifyExportSuccess("当前装备对比报告", path);
            }
            catch (Exception ex)
            {
                Growl.Error($"导出失败：{ex.Message}");
            }
            finally
            {
                IsExporting = false;
            }
        }

        private async void CalcAllSlots()
        {
            IsCalculatingAll = true;
            var progressVM = new PzBuildCompareExportProgressViewModel(
                Slots.Count, "正在计算所有部位装备对比...");
            var progressWindow = new PzBuildCompareExportProgressWindow(progressVM)
            {
                Owner = GetOwnerWindow()
            };

            Exception calcException = null;
            progressWindow.Loaded += async (sender, args) =>
            {
                try
                {
                    await CalcAllSlotsCoreAsync(progressVM);
                }
                catch (Exception ex)
                {
                    calcException = ex;
                }
                finally
                {
                    progressWindow.Close();
                }
            };

            progressWindow.ShowDialog();
            IsCalculatingAll = false;

            if (calcException != null)
            {
                Growl.Error($"计算失败：{calcException.Message}");
            }
        }

        private void ExportAllSlotReports()
        {
            var path = SelectExportPath(PzBuildCompareReportExporter.CreateDefaultFileName("全部"));
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            IsExporting = true;
            var progressVM = new PzBuildCompareExportProgressViewModel(Slots.Count);
            var progressWindow = new PzBuildCompareExportProgressWindow(progressVM)
            {
                Owner = GetOwnerWindow()
            };

            var reports = new List<PzBuildCompareSlotReport>();
            Exception exportException = null;

            progressWindow.Loaded += async (sender, args) =>
            {
                try
                {
                    await CalcAllSlotsCoreAsync(progressVM,
                        slot => reports.Add(slot.CreateReport()));
                    progressVM.BeginExportFile();
                    await Task.Run(() => PzBuildCompareReportExporter.ExportAll(path, reports.ToArray()));
                }
                catch (Exception ex)
                {
                    exportException = ex;
                }
                finally
                {
                    progressWindow.Close();
                }
            };

            progressWindow.ShowDialog();
            IsExporting = false;

            if (exportException != null)
            {
                Growl.Error($"导出失败：{exportException.Message}");
                return;
            }

            NotifyExportSuccess("全部装备对比报告", path);
        }

        private async Task CalcAllSlotsCoreAsync(
            PzBuildCompareExportProgressViewModel progressVM,
            Action<PzBuildCompareSlotViewModel> onSlotComplete = null)
        {
            for (var i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                progressVM.BeginSlot(slot, i + 1);
                SelectedSlot = slot;
                await slot.EnsureCalculatedAsync();
                onSlotComplete?.Invoke(slot);
                progressVM.CompleteSlot();
            }
        }

        private bool CanCalcAllSlots()
        {
            return !IsExporting && !IsCalculatingAll && Slots != null && Slots.Count > 0;
        }

        private bool CanExportCurrentSlotReport()
        {
            return !IsExporting && !IsCalculatingAll && SelectedSlot != null;
        }

        private bool CanExportAllSlotReports()
        {
            return !IsExporting && !IsCalculatingAll && Slots != null && Slots.Count > 0;
        }

        private static void NotifyExportSuccess(string label, string path)
        {
            Growl.Ask($"{label}已导出到：\n{path}\n\n是否打开文件？", confirmed =>
            {
                if (confirmed)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                return true;
            });
        }

        private string SelectExportPath(string fileName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel 工作簿 (*.xlsx)|*.xlsx",
                FileName = fileName,
                DefaultExt = ".xlsx",
                AddExtension = true,
                OverwritePrompt = true
            };

            return saveFileDialog.ShowDialog(GetOwnerWindow()) == true ? saveFileDialog.FileName : null;
        }

        private Window GetOwnerWindow()
        {
            return Application.Current.Windows.OfType<Window>().FirstOrDefault(window => window.IsActive)
                   ?? Application.Current.MainWindow;
        }

        private void HandleCurrentEquipChanged(PzBuildCompareSlotViewModel changedSlot)
        {
            foreach (var slot in Slots)
            {
                slot.MarkNotCalculated();
            }

            RequestClose?.Invoke();
        }
    }
}
