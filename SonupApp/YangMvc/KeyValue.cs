using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace YangMvc
{
    public class KeyValue<TValue>
    {
        public string Key { get; set; }

        public TValue Value { get; set; }

        public KeyValue(string key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        } 
    }

    public class KeyValueList<TValue> : List<KeyValue<TValue>>
    {
        public bool UrlEncode = true;

        public static KeyValueList<TValue> Create(string key, TValue value)
        {
            var list = new KeyValueList<TValue>();
            list.Add(new KeyValue<TValue>(key, value));
            return list;
        }

        public KeyValueList<TValue> Join(string key, TValue value)
        {
            this.Add(new KeyValue<TValue>(key, value));
            return this;
        }

        public string ToUrlParamString(bool urlEncode = true)
        {
            StringBuilder sb = new StringBuilder(100);
            foreach (var item in this)
            {
                if (sb.Length > 0)
                    sb.Append("&");

                if (urlEncode)
                    sb.Append(item.Key + "=" + WebUtility.UrlEncode(item.Value?.ToString() ?? ""));
                else
                    sb.Append(item.Key + "=" + item.Value?.ToString() ?? "");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(100);
            foreach (var item in this)
            {
                if (sb.Length > 0)
                    sb.Append("&");
                sb.Append(item.Key + "=" + WebUtility.UrlEncode(item.Value?.ToString() ?? ""));
            }
            return sb.ToString();
        }

        public string CreateSn()
        {
            var list = this.OrderBy(kv => kv.Key).ToList();
            string qs = string.Join("&", list.Select(kv => $"{kv.Key}={kv.Value}")) + DateTime.Today.ToString("yyyy-MM-dd");
            return qs.ToMd5();
        }

    }
}
