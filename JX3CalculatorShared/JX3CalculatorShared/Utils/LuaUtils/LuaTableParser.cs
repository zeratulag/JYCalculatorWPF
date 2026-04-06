using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LuaUtils
{
    /// <summary>
    /// 手动实现的简易 Lua 解析器，移植自 Python luadata 库
    /// 不依赖第三方包
    /// </summary>
    public static class LuaTableParser
    {
        // 定义解析器的状态枚举
        private enum State
        {
            SeekChild,
            Value,
            ValueEnd,
            KeySimple,
            KeySimpleEnd,
            KeyExpressionOpen,
            KeyExpressionFinish,
            KeyExpressionClose,
            Text,
            Int,
            Float
        }

        // 内部节点类，用于构建树状结构
        private class LuaNode
        {
            public List<KeyValuePair<object, object>> Entries { get; } = new List<KeyValuePair<object, object>>();
            public bool IsRoot { get; set; } = false;
            // 记录当前作为数组的最大连续索引
            public int LuaLen { get; set; } = 0;
        }

        // 上下文，用于栈存储
        private class ParseContext
        {
            public LuaNode Node { get; set; }
            public State State { get; set; }
            public object Key { get; set; }
        }

        /// <summary>
        /// 解析 Lua 数据字符串为 C# 对象 (List<object> 或 Dictionary<string, object>)
        /// </summary>
        public static object Parse(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            // 预处理：去除 return 前缀（如果存在）
            raw = raw.Trim();
            if (raw.StartsWith("return"))
            {
                raw = raw.Substring(6).TrimStart();
            }

            var root = new LuaNode { IsRoot = true };
            var stack = new Stack<ParseContext>();

            LuaNode node = root;
            State state = State.SeekChild;
            object key = null;

            // 字符串处理变量
            char? quoteChar = null;
            bool escaping = false;
            StringBuilder sb = new StringBuilder();

            // 注释处理状态
            bool inCommentInline = false;
            bool inCommentMultiline = false;

            // 遍历指针
            int pos = 0;
            int len = raw.Length;

            while (pos <= len)
            {
                // 获取当前字符 (pos == len 时为 null，用于处理结束状态)
                char? ch = pos < len ? raw[pos] : (char?)null;
                bool isSpace = ch.HasValue && char.IsWhiteSpace(ch.Value);

                // --- 1. 全局注释处理 ---
                if (inCommentMultiline)
                {
                    // 检测 ]] 结束
                    if (ch == ']' && CheckLookAhead(raw, pos, "]]"))
                    {
                        inCommentMultiline = false;
                        pos += 2;
                        continue;
                    }
                    pos++; continue;
                }
                else if (inCommentInline)
                {
                    // 检测换行结束
                    if (ch == '\n') inCommentInline = false;
                    pos++; continue;
                }
                else if (state != State.Text && ch == '-')
                {
                    // 检测注释开始
                    if (CheckLookAhead(raw, pos, "--[["))
                    {
                        inCommentMultiline = true;
                        pos += 4; continue;
                    }
                    else if (CheckLookAhead(raw, pos, "--"))
                    {
                        inCommentInline = true;
                        pos += 2; continue;
                    }
                }

                if (ch == null && state == State.SeekChild && node.IsRoot) break;

                // --- 2. 状态机逻辑 ---
                switch (state)
                {
                    case State.SeekChild:
                        if (ch == null) break;

                        // 键名开始 (KeySimple): 字母或下划线
                        if (!node.IsRoot && (char.IsLetter(ch.Value) || ch == '_'))
                        {
                            state = State.KeySimple;
                            sb.Clear();
                            sb.Append(ch.Value);
                        }
                        // 键表达式开始 (KeyExpression): [
                        else if (!node.IsRoot && ch == '[')
                        {
                            state = State.KeyExpressionOpen;
                        }
                        // 表结束: }
                        else if (ch == '}')
                        {
                            if (stack.Count == 0) throw new Exception("Unexpected '}' at root level.");

                            var finalVal = NodeToObj(node); // 将当前节点转为 C# 对象
                            var ctx = stack.Pop();

                            // 恢复上一层环境
                            node = ctx.Node;
                            state = ctx.State; // 恢复之前的状态

                            // 将当前表作为值，写入父节点
                            if (state == State.KeyExpressionOpen) // 特殊情况：作为 key
                            {
                                // Python版暂不支持 table 作为 key，这里简化处理
                                key = finalVal;
                                state = State.KeyExpressionFinish; // 等待 ]
                            }
                            else
                            {
                                AddEntry(node, ctx.Key, finalVal);
                                state = State.ValueEnd;
                                key = null;
                            }
                        }
                        // 值的开始 (如果是数组项，自动生成 Key)
                        else if (!isSpace && ch != ',') // 忽略空白和逗号
                        {
                            key = node.LuaLen + 1; // 自动索引 1, 2, 3...
                            state = State.Value;
                            pos--; // 回退一步，让 Value 状态处理这个字符
                        }
                        break;

                    case State.Value:
                        if (ch == null) throw new Exception("Unexpected end of input in Value.");

                        if (ch == '"' || ch == '\'')
                        {
                            state = State.Text;
                            quoteChar = ch;
                            sb.Clear();
                        }
                        else if (ch == '-' || char.IsDigit(ch.Value))
                        {
                            state = State.Int;
                            sb.Clear();
                            sb.Append(ch.Value);
                        }
                        else if (ch == '.')
                        {
                            state = State.Float;
                            sb.Clear();
                            sb.Append(ch.Value);
                        }
                        else if (ch == '{')
                        {
                            // 嵌套表开始：压栈
                            stack.Push(new ParseContext { Node = node, State = State.Value, Key = key });
                            node = new LuaNode();
                            state = State.SeekChild;
                        }
                        else if (ch == 't' && CheckLookAhead(raw, pos, "true"))
                        {
                            AddEntry(node, key, true);
                            state = State.ValueEnd;
                            key = null;
                            pos += 3; // 跳过 true
                        }
                        else if (ch == 'f' && CheckLookAhead(raw, pos, "false"))
                        {
                            AddEntry(node, key, false);
                            state = State.ValueEnd;
                            key = null;
                            pos += 4; // 跳过 false
                        }
                        else if (ch == 'n' && CheckLookAhead(raw, pos, "nil"))
                        {
                            // nil 通常不添加，或者添加 null
                            state = State.ValueEnd;
                            key = null;
                            pos += 2;
                        }
                        break;

                    case State.Text:
                        if (ch == null) throw new Exception("Unclosed string.");
                        if (escaping)
                        {
                            // 简单处理转义，如需完整支持可扩展
                            if (ch == 'n') sb.Append('\n');
                            else if (ch == 't') sb.Append('\t');
                            else sb.Append(ch.Value);
                            escaping = false;
                        }
                        else if (ch == '\\')
                        {
                            escaping = true;
                        }
                        else if (ch == quoteChar)
                        {
                            // 字符串结束
                            string strVal = sb.ToString();
                            // 判断是 Key 还是 Value
                            if (key == null) // 说明刚才在解析 KeyExpression ([ "key" ])
                            {
                                key = strVal;
                                state = State.KeyExpressionFinish;
                            }
                            else // 说明在解析 Value (key="value")
                            {
                                AddEntry(node, key, strVal);
                                state = State.ValueEnd;
                                key = null;
                            }
                        }
                        else
                        {
                            sb.Append(ch.Value);
                        }
                        break;

                    case State.Int:
                        // 继续读取数字
                        if (ch.HasValue && (char.IsDigit(ch.Value) || ch == '.'))
                        {
                            if (ch == '.') state = State.Float;
                            sb.Append(ch.Value);
                        }
                        else
                        {
                            // 数字结束
                            object numVal = ParseNumber(sb.ToString(), state);
                            if (key == null) // 在 KeyExpression 中
                            {
                                key = numVal;
                                state = State.KeyExpressionFinish;
                            }
                            else
                            {
                                AddEntry(node, key, numVal);
                                state = State.ValueEnd;
                                key = null;
                            }
                            pos--; // 回退，因为当前字符属于下一个状态
                        }
                        break;

                    case State.Float:
                        if (ch.HasValue && char.IsDigit(ch.Value))
                        {
                            sb.Append(ch.Value);
                        }
                        else
                        {
                            object numVal = ParseNumber(sb.ToString(), state);
                            if (key == null)
                            {
                                key = numVal;
                                state = State.KeyExpressionFinish;
                            }
                            else
                            {
                                AddEntry(node, key, numVal);
                                state = State.ValueEnd;
                                key = null;
                            }
                            pos--;
                        }
                        break;

                    case State.KeySimple:
                        // 允许字母、数字、下划线
                        if (ch.HasValue && (char.IsLetterOrDigit(ch.Value) || ch == '_'))
                        {
                            sb.Append(ch.Value);
                        }
                        else
                        {
                            key = sb.ToString();
                            state = State.KeySimpleEnd;
                            pos--;
                        }
                        break;

                    case State.KeySimpleEnd:
                        if (ch == '=')
                        {
                            state = State.Value;
                        }
                        else if (!isSpace)
                        {
                            throw new Exception($"Expected '=' after key '{key}' at pos {pos}");
                        }
                        break;

                    case State.KeyExpressionOpen:
                        // 等待 Value 逻辑来解析 Key 的具体值 (int/string)
                        if (!isSpace)
                        {
                            // 进入 Value 状态来读取 key 的值，但不设置 key 变量
                            state = State.Value;
                            pos--;
                        }
                        break;

                    case State.KeyExpressionFinish:
                        if (ch == ']')
                        {
                            state = State.KeyExpressionClose;
                        }
                        else if (!isSpace) throw new Exception("Expected ']'");
                        break;

                    case State.KeyExpressionClose:
                        if (ch == '=')
                        {
                            state = State.Value;
                        }
                        else if (!isSpace) throw new Exception("Expected '='");
                        break;

                    case State.ValueEnd:
                        if (ch == ',' || ch == ';')
                        {
                            state = State.SeekChild;
                        }
                        else if (ch == '}')
                        {
                            state = State.SeekChild; // 交给 SeekChild 处理闭合
                            pos--;
                        }
                        else if (!isSpace && ch != null)
                        {
                            // 有些Lua写得不规范，没有逗号分隔，这里容错处理
                            // 只要不是空格，就认为开始下一个了
                            state = State.SeekChild;
                            pos--;
                        }
                        break;
                }
                pos++;
            }

            return NodeToObj(root);
        }

        // --- 辅助方法 ---

        private static bool CheckLookAhead(string raw, int pos, string target)
        {
            if (pos + target.Length > raw.Length) return false;
            for (int i = 0; i < target.Length; i++)
            {
                if (raw[pos + i] != target[i]) return false;
            }
            return true;
        }

        private static object ParseNumber(string str, State type)
        {
            if (type == State.Int)
            {
                if (long.TryParse(str, out long l)) return l; // 优先尝试 long
                if (double.TryParse(str, out double d)) return d;
            }
            else
            {
                if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double d)) return d;
            }
            return 0;
        }

        private static void AddEntry(LuaNode node, object key, object val)
        {
            node.Entries.Add(new KeyValuePair<object, object>(key, val));

            // 维护 LuaLen (如果是连续整数键)
            if (key is int iKey)
            {
                if (iKey == node.LuaLen + 1) node.LuaLen++;
            }
            else if (key is long lKey)
            {
                if (lKey == node.LuaLen + 1) node.LuaLen++;
            }
        }

        private static object NodeToObj(LuaNode node)
        {
            // 判断是否为纯数组
            // 规则：条目数量等于 LuaLen (即所有 key 都是 1..N 的连续整数)
            bool isArray = node.Entries.Count == node.LuaLen && node.LuaLen > 0;

            // 如果是空表，既可以是空 List 也可以是空 Dict。
            // 按照惯例，如果上下文是 Python list 则为 [], 否则 {}。
            // 这里我们默认空表为 List<object> (JSON []).
            if (node.Entries.Count == 0) return new List<object>();

            if (isArray)
            {
                // 排序并提取值
                var list = new List<object>();
                // 对 entries 按 key 排序 (Key 肯定是数字)
                var sorted = node.Entries.OrderBy(x => Convert.ToInt64(x.Key));
                foreach (var kv in sorted)
                {
                    list.Add(kv.Value);
                }
                return list;
            }
            else
            {
                // 转为 Dictionary
                // 为了兼容性，Key 统一转 string。如果需要原始 Key (如 int key)，可改为 Dictionary<object, object>
                var dict = new Dictionary<string, object>();
                foreach (var kv in node.Entries)
                {
                    string keyStr = kv.Key.ToString();
                    dict[keyStr] = kv.Value;
                }
                return dict;
            }
        }
    }
}