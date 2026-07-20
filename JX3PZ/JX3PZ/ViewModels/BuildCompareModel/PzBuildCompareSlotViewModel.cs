using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JX3PZ.Messages;
using JX3PZ.Models;
using JYCalculator.Globals;
using JYCalculator.Models;
using PropertyChanged;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JX3PZ.ViewModels
{
    public class PzBuildCompareSlotViewModel : ObservableObject
    {
        private static readonly int MaxParallelism = Math.Min(6, Math.Max(1, Environment.ProcessorCount - 1));
        private readonly PzMainWindowViewModels _pzMainVM;
        private readonly PzTabItemViewModel _slotVM;
        private PzBuildCompareRowViewModel _selectedRow;
        private int _calcVersion;
        private Task _calcTask = Task.CompletedTask;

        public event Action<PzBuildCompareSlotViewModel> CurrentEquipChanged;

        public int Position => _slotVM.Position;
        public string Header => _slotVM.Header;
        public string Label => _slotVM.Map.Label;
        public double BaseDps { get; private set; }
        public ObservableCollection<PzBuildCompareRowViewModel> Items { get; }
        public BuildCompareModel CompareModel { get; private set; }
        public RelayCommand SetCurrentEquipCmd { get; }
        public bool IncludeEmptyCandidate { get; } = false;

        public PzBuildCompareRowViewModel SelectedRow
        {
            get => _selectedRow;
            set
            {
                if (SetProperty(ref _selectedRow, value))
                {
                    SetCurrentEquipCmd.NotifyCanExecuteChanged();
                }
            }
        }

        [AlsoNotifyFor(nameof(ProgressMaximum), nameof(ProgressPercent), nameof(ProgressText))]
        public int Completed { get; private set; }

        [AlsoNotifyFor(nameof(ProgressMaximum), nameof(ProgressPercent), nameof(ProgressText))]
        public int Total { get; private set; }

        public int ProgressMaximum => Total > 0 ? Total : 1;
        public double ProgressPercent => Total > 0 ? Completed * 100.0 / Total : 0;
        public string ProgressText => $"{Completed} / {Total}";
        public string ElapsedText { get; private set; } = "";
        public bool IsCalculating { get; private set; }
        public bool HasCalculated { get; private set; }

        [AlsoNotifyFor(nameof(StatusText), nameof(StatusColor))]
        public PzBuildCompareSlotStatusText CalcStatus { get; private set; }
            = PzBuildCompareSlotStatusText.未计算;

        public string StatusColor => CalcStatus.GetStatusColor();

        public string StatusText => CalcStatus.ToString();

        public PzBuildCompareSlotViewModel(PzMainWindowViewModels pzMainVM, PzTabItemViewModel slotVM)
        {
            _pzMainVM = pzMainVM;
            _slotVM = slotVM;
            Items = new ObservableCollection<PzBuildCompareRowViewModel>();
            SetCurrentEquipCmd = new RelayCommand(SetCurrentEquip, CanSetCurrentEquip);
        }

        public void EnsureCalculated()
        {
            if (HasCalculated || IsCalculating)
            {
                return;
            }

            RefreshCompare();
        }

        public Task EnsureCalculatedAsync()
        {
            if (HasCalculated)
            {
                return Task.CompletedTask;
            }

            if (!IsCalculating)
            {
                RefreshCompare();
            }

            return _calcTask ?? Task.CompletedTask;
        }

        public void MarkNotCalculated()
        {
            _calcVersion++;
            SelectedRow = null;
            Items.Clear();
            CompareModel = null;
            BaseDps = 0;
            Completed = 0;
            Total = 0;
            ElapsedText = "";
            _calcTask = Task.CompletedTask;
            HasCalculated = false;
            IsCalculating = false;
            CalcStatus = PzBuildCompareSlotStatusText.未计算;
            RefreshCommandState();
        }

        public PzBuildCompareSlotReport CreateReport()
        {
            return new PzBuildCompareSlotReport(this);
        }

        private void RefreshCompare()
        {
            var version = ++_calcVersion;
            HasCalculated = false;

            var mainModel = GlobalContext.ViewModels.Main.Model;
            var condition = BuildCompareConditionSnapshot.FromMainModel(mainModel);
            BaseDps = condition.BaseDps;

            var provider = new PzEquipBuildCompareCandidateProvider(_pzMainVM.Model, _slotVM);
            var candidates = provider.GetCandidates(IncludeEmptyCandidate);
            CompareModel = new BuildCompareModel(new BuildCompareArg
            {
                Condition = condition,
                Candidates = candidates
            });

            var rows = candidates
                .OrderByDescending(candidate => candidate.IsCurrent)
                .ThenBy(candidate => candidate.IsEmpty ? 0 : 1)
                .ThenByDescending(candidate => candidate.Level)
                .ThenBy(candidate => candidate.Name)
                .Select(candidate => new PzBuildCompareRowViewModel(candidate, BaseDps));

            SelectedRow = null;
            Items.Clear();
            foreach (var row in rows)
            {
                Items.Add(row);
            }

            Completed = 0;
            Total = Items.Count;
            ElapsedText = "";
            _calcTask = StartCalcAsync(version);
        }

        private async Task StartCalcAsync(int version)
        {
            await Task.Yield();
            if (version != _calcVersion)
            {
                return;
            }

            IsCalculating = true;
            RefreshCommandState();
            CalcStatus = PzBuildCompareSlotStatusText.计算中;
            var sw = Stopwatch.StartNew();

            using (var semaphore = new SemaphoreSlim(MaxParallelism))
            {
                var tasks = Items
                    .Select(row => CalcRowAsync(row, semaphore, version))
                    .ToArray();
                await Task.WhenAll(tasks);
            }

            if (version != _calcVersion)
            {
                return;
            }

            UpdateRanks();
            UpdateRelativeDeltaBars();
            sw.Stop();
            ElapsedText = $"{sw.Elapsed.TotalSeconds:F3} 秒";
            HasCalculated = true;
            IsCalculating = false;
            RefreshCommandState();
            CalcStatus = PzBuildCompareSlotStatusText.完成;
            Log.Information("[BuildCompare] {Header} 智能对比完成，共 {Total} 件，耗时 {Elapsed} ms",
                Header, Total, sw.ElapsedMilliseconds);
        }

        private void SetCurrentEquip()
        {
            var candidate = SelectedRow?.Candidate;
            if (candidate == null)
            {
                return;
            }

            var EID = candidate.IsEmpty ? null : candidate.Equip?.EID ?? candidate.Key;
            WeakReferenceMessenger.Default.Send(new PzSetEquipMessage(_slotVM.Position, EID), _slotVM.Position);
            CurrentEquipChanged?.Invoke(this);
        }

        private bool CanSetCurrentEquip()
        {
            return !IsCalculating && SelectedRow != null && !SelectedRow.IsCurrent;
        }

        private void RefreshCommandState()
        {
            SetCurrentEquipCmd.NotifyCanExecuteChanged();
        }

        private void UpdateRanks()
        {
            var rankedRows = Items
                .Where(row => row.HasValue)
                .OrderByDescending(row => row.Dps)
                .ToList();

            var rank = 0;
            double? lastDps = null;
            for (var i = 0; i < rankedRows.Count; i++)
            {
                var row = rankedRows[i];
                if (lastDps == null || row.Dps != lastDps.Value)
                {
                    rank = i + 1;
                    lastDps = row.Dps;
                }

                row.SetRank(rank);
            }

            foreach (var row in Items.Where(row => !row.HasValue))
            {
                row.SetRank(0);
            }
        }

        private void UpdateRelativeDeltaBars()
        {
            var values = Items
                .Where(row => row.HasValue && !row.Candidate.IsEmpty && !double.IsNaN(row.DeltaDpsPct))
                .Select(row => row.DeltaDpsPct)
                .ToList();

            var maxPositive = values
                .Where(value => value > 0)
                .DefaultIfEmpty(0)
                .Max();
            var maxNegativeAbs = values
                .Where(value => value < 0)
                .Select(Math.Abs)
                .DefaultIfEmpty(0)
                .Max();

            foreach (var row in Items)
            {
                row.RelativeDeltaBar.Update(row.DeltaDpsPct, maxPositive, maxNegativeAbs, row.HasValue);
            }
        }

        private async Task CalcRowAsync(PzBuildCompareRowViewModel row, SemaphoreSlim semaphore, int version)
        {
            await semaphore.WaitAsync();
            try
            {
                if (version != _calcVersion)
                {
                    return;
                }

                row.BeginCalc();
                var result = await Task.Run(() => CompareModel.CalcCandidate(row.Candidate));
                if (version != _calcVersion)
                {
                    return;
                }

                row.UpdateFrom(result);
            }
            catch (Exception ex)
            {
                if (version == _calcVersion)
                {
                    row.UpdateFrom(new BuildCompareResultItem
                    {
                        Candidate = row.Candidate,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }
            finally
            {
                if (version == _calcVersion)
                {
                    Completed++;
                }

                semaphore.Release();
            }
        }
    }
}
