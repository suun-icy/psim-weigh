using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pism_weigh.Models
{
    /// <summary>
    /// 修改历史记录项
    /// </summary>
    public class ModifyHistoryItem
    {
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 修改人 ID
        /// </summary>
        public string ModifierId { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string ModifierName { get; set; }

        /// <summary>
        /// 修改的字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 修改前的值
        /// </summary>
        public string OldValue { get; set; }

        /// <summary>
        /// 修改后的值
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// 修改原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ModifyHistoryItem()
        {
            ModifyTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 称重记录完整修改历史
    /// </summary>
    public class WeighRecordModifyHistory
    {
        /// <summary>
        /// 记录 ID
        /// </summary>
        public string RecordId { get; set; }

        /// <summary>
        /// 修改历史列表
        /// </summary>
        public List<ModifyHistoryItem> Items { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WeighRecordModifyHistory()
        {
            Items = new List<ModifyHistoryItem>();
        }

        /// <summary>
        /// 添加修改记录
        /// </summary>
        public void AddItem(ModifyHistoryItem item)
        {
            if (Items == null)
            {
                Items = new List<ModifyHistoryItem>();
            }
            Items.Add(item);
        }

        /// <summary>
        /// 转换为 JSON 字符串（兼容.NET 4.7.2）
        /// </summary>
        public string ToJsonString()
        {
            if (Items == null || Items.Count == 0)
            {
                return "[]";
            }

            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (i > 0) sb.Append(",");
                sb.Append("{");
                sb.AppendFormat("\"ModifyTime\":\"{0}\"", item.ModifyTime.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendFormat(",\"ModifierId\":\"{0}\"", EscapeJson(item.ModifierId));
                sb.AppendFormat(",\"ModifierName\":\"{0}\"", EscapeJson(item.ModifierName));
                sb.AppendFormat(",\"FieldName\":\"{0}\"", EscapeJson(item.FieldName));
                sb.AppendFormat(",\"OldValue\":\"{0}\"", EscapeJson(item.OldValue));
                sb.AppendFormat(",\"NewValue\":\"{0}\"", EscapeJson(item.NewValue));
                sb.AppendFormat(",\"Reason\":\"{0}\"", EscapeJson(item.Reason));
                sb.Append("}");
            }
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 从 JSON 字符串解析（简单实现，兼容.NET 4.7.2）
        /// </summary>
        public static WeighRecordModifyHistory FromJsonString(string json)
        {
            var history = new WeighRecordModifyHistory();
            if (string.IsNullOrEmpty(json) || json == "[]")
            {
                return history;
            }

            try
            {
                // 简单的 JSON 解析（针对特定格式）
                json = json.Trim();
                if (json.StartsWith("["))
                {
                    json = json.Substring(1);
                }
                if (json.EndsWith("]"))
                {
                    json = json.Substring(0, json.Length - 1);
                }

                // 分割每个对象
                var items = SplitJsonObjects(json);
                foreach (var itemJson in items)
                {
                    var item = ParseHistoryItem(itemJson);
                    if (item != null)
                    {
                        history.Items.Add(item);
                    }
                }
            }
            catch
            {
                // 解析失败返回空列表
            }

            return history;
        }

        private static List<string> SplitJsonObjects(string json)
        {
            var result = new List<string>();
            int depth = 0;
            int start = 0;

            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    if (depth == 0) start = i;
                    depth++;
                }
                else if (json[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        result.Add(json.Substring(start, i - start + 1));
                    }
                }
            }

            return result;
        }

        private static ModifyHistoryItem ParseHistoryItem(string json)
        {
            var item = new ModifyHistoryItem();
            try
            {
                item.ModifyTime = ExtractStringValue(json, "ModifyTime", DateTime.Now);
                item.ModifierId = ExtractStringValue(json, "ModifierId", "");
                item.ModifierName = ExtractStringValue(json, "ModifierName", "");
                item.FieldName = ExtractStringValue(json, "FieldName", "");
                item.OldValue = ExtractStringValue(json, "OldValue", "");
                item.NewValue = ExtractStringValue(json, "NewValue", "");
                item.Reason = ExtractStringValue(json, "Reason", "");
            }
            catch
            {
                return null;
            }
            return item;
        }

        private static string ExtractStringValue(string json, string key, string defaultValue)
        {
            var searchKey = "\"" + key + "\":";
            int index = json.IndexOf(searchKey);
            if (index < 0) return defaultValue;

            int start = json.IndexOf('"', index + searchKey.Length);
            if (start < 0) return defaultValue;

            int end = start + 1;
            while (end < json.Length)
            {
                if (json[end] == '"' && json[end - 1] != '\\')
                {
                    break;
                }
                end++;
            }

            if (end >= json.Length) return defaultValue;

            return UnescapeJson(json.Substring(start + 1, end - start - 1));
        }

        private static DateTime ExtractStringValue(string json, string key, DateTime defaultValue)
        {
            var searchKey = "\"" + key + "\":";
            int index = json.IndexOf(searchKey);
            if (index < 0) return defaultValue;

            int start = json.IndexOf('"', index + searchKey.Length);
            if (start < 0) return defaultValue;

            int end = start + 1;
            while (end < json.Length)
            {
                if (json[end] == '"' && json[end - 1] != '\\')
                {
                    break;
                }
                end++;
            }

            if (end >= json.Length) return defaultValue;

            DateTime result;
            if (DateTime.TryParse(json.Substring(start + 1, end - start - 1), out result))
            {
                return result;
            }
            return defaultValue;
        }

        private static string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\")
                       .Replace("\"", "\\\"")
                       .Replace("\n", "\\n")
                       .Replace("\r", "\\r")
                       .Replace("\t", "\\t");
        }

        private static string UnescapeJson(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\t", "\t")
                       .Replace("\\n", "\n")
                       .Replace("\\r", "\r")
                       .Replace("\\\"", "\"")
                       .Replace("\\\\", "\\");
        }
    }
}
