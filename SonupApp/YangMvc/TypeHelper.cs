using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShareCarApi.Common; 
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System
{
    public static class TypeHelper
    {
        public static int ToInt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            int i = 0;
            int.TryParse(str, out i);
            return i;
        }

        public static string ShowDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        public static string ShowDateText(this DateTime dt)
        {
            return (DateTime.Now - dt).TotalMinutes.ShowMinutes();
        }

        public static string ShowDateText(this DateTime? dt)
        {
            if (dt == null)
                return "";
            return (DateTime.Now - dt.Value).TotalMinutes.ShowMinutes();
        }

        public static string ShowDateTime(this DateTime dt, bool withSeconds = false)
        {
            return dt.ToString("yyyy-MM-dd HH:mm" + (withSeconds ? ":ss" : ""));
        }

        public static string ShowTime(this DateTime dt)
        {
            return dt.ToString("HH:mm:ss");
        }

        public static string ShowDate(this DateTime? dt)
        {
            if (dt == null)
                return "";
            return dt.Value.ToString("yyyy-MM-dd");
        }

        public static string ShowDateTime(this DateTime? dt, bool withSeconds = false)
        {
            if (dt == null)
                return "";
            if (dt.Value.Year != DateTime.Now.Year)
                return dt.Value.ToString("yyyy-MM-dd HH:mm" + (withSeconds ? ":ss" : ""));
            else
                return dt.Value.ToString("M月d日 HH:mm" + (withSeconds ? ":ss" : ""));
        }

        public static string ShowTime(this DateTime? dt)
        {
            if (dt == null)
                return "";
            return dt.Value.ToString("HH:mm:ss");
        }

        public static string DisplayName(this Enum item)
        {
            Type type = item.GetType();
            System.Reflection.FieldInfo fi = type.GetField(item.ToString());
            if (fi == null)
            {
                return item.ToString();
            }
            object[] attrs = fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attrs.Length == 0)
                return item.ToString();
            else
                return ((DisplayAttribute)attrs[0]).Name;
        }

        public static string HtmlOptions(this Enum item, params Enum[] exceptItem)
        {
            StringBuilder sb = new StringBuilder(50);
            foreach (Enum ei in Enum.GetValues(item.GetType()))
            {
                if (exceptItem != null && exceptItem.Contains(ei))
                {
                    continue;
                }
                string selected = (ei == item ? "selected" : "");
                sb.Append($"<option value='{Convert.ToInt32(ei)}' {selected}>{ei.DisplayName()}</option>");
            }
            return sb.ToString();
        }
         
        public static string ToJsString(this bool val)
        {
            return val.ToString().ToLower();
        }

        public static string ToJsonBat(this object obj)
        {
            if (obj == null)
                return "";
            JsonSerializerSettings jss = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToObject<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToLogString(this NameValueCollection form)
        {
            if (form == null)
                return "";
            StringBuilder sb = new StringBuilder(form.Count * 30);
            foreach (string key in form.Keys)
            {
                sb.Append($"'{key}':'{form[key]}'");
            }
            return sb.ToString();
        }

        public static string ShowMinutes(this int mi)
        {
            if (mi < 60)
                return $"{mi}分钟";
            else if (mi < 1440)
                return $"{mi / 60}小时";
            else if (mi < 525600)
                return $"{mi / 1440}天";
            else if (mi > 1440000)
                return "";
            else
                return "一年";
        }

        public static string ShowMinutes(this double mi)
        {
            return Convert.ToInt32(mi).ShowMinutes();
        }

        public static string ShowMinutes(this TimeSpan span)
        {
            var mi = Convert.ToInt64(span.TotalMinutes);
            if (mi < 60)
                return $"{mi}分钟";
            else if (mi < 1440)
                return $"{mi / 60}小时";
            else
                return $"{mi / 1440}天";
        }

        public static string Left(this string str, int len)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= len)
                return str;
            return str.Substring(0, len);
        }

        private static byte[] EncryptBat(byte[] bytes, string pk)
        {
            var pvkeys = Encoding.ASCII.GetBytes(pk).ToList(); 
            while (pvkeys.Count < bytes.Length)
                pvkeys.AddRange(pvkeys);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= pvkeys[i];
            }
            return bytes;
        }

        public static string EncryBat(this string str, string pk = "123abc@bat_II&10oo00OO0")
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            bytes = EncryptBat(bytes, pk);
            return Convert.ToBase64String(bytes);
        }

        public static string DecryBat(this string str, string pk = "123abc@bat_II&10oo00OO0")
        {
            var bytes = Convert.FromBase64String(str);
            bytes = EncryptBat(bytes, pk);
            return Encoding.UTF8.GetString(bytes);
        }

        #region 原加密方法

        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public static string Encrypt(this string encryptString, string encryptKey)
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));//转换为字节
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();//实例化数据加密标准
            MemoryStream mStream = new MemoryStream();//实例化内存流
                                                      //将数据流链接到加密转换的流
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string Decrypt(this string decryptString, string decryptKey)
        {
            if (string.IsNullOrEmpty(decryptString))
                return decryptString;

            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        public static string Encrypt(this string str)
        {
            return str.Encrypt("baT &-gp");
        }

        public static string Decrypt(this string str)
        {
            return str.Decrypt("baT &-gp");
        }
        #endregion



        public static byte[] ToBytes(this Object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        public static Object ToObject(this byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                IFormatter formatter = new BinaryFormatter();
                var obj = formatter.Deserialize(ms);
                return obj;
            }
        }
    }
}
