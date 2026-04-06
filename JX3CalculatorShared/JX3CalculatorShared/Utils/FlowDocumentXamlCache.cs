using System;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Markup;

namespace JX3CalculatorShared.Utils
{
    public static class FlowDocumentXamlCache
    {
        private sealed class Holder
        {
            public string Xaml { get; set; }

            public Holder(string xaml)
            {
                Xaml = xaml;
            }
        }

        private static readonly ConditionalWeakTable<FlowDocument, Holder> Table
            = new ConditionalWeakTable<FlowDocument, Holder>();

        public static string GetOrAddXaml(FlowDocument source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (Table.TryGetValue(source, out var holder) && holder.Xaml != null)
                return holder.Xaml;

            string xaml = XamlWriter.Save(source); // 需在 UI 线程
            Table.GetValue(source, _ => new Holder(xaml));
            return xaml;
        }

        /// <summary>
        /// 使用缓存获取XAML字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetXamlUsingCache(this FlowDocument source)
        {
            return GetOrAddXaml(source);
        }

        public static void Invalidate(FlowDocument source)
        {
            if (source == null) return;
            if (Table.TryGetValue(source, out var holder))
                holder.Xaml = null;
        }
    }
}