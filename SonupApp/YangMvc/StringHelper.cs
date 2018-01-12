using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace YangMvc
{
    public static class StringHelper
    {
        public static string ToHex(this byte b)
        {
            return b.ToString("X").PadLeft(2, '0');
        }

        public static string ToHex(this IEnumerable<byte> bytes, bool split = true)
        {
            List<byte> list = bytes.ToList();
            while (list.Count > 0 && list.Last<byte>() == 0)
            {
                list.RemoveAt(list.Count - 1);
            }
            return string.Join(split ? " " : "", list.Select(b => b.ToString("X").PadLeft(2, '0')));
        }

        public static string ToMd5(this string str)
        {
            str = str.ToLower();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(str);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            StringBuilder sTemp = new StringBuilder(str.Length);
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp.Append(bytHash[i].ToString("X").PadLeft(2, '0'));
            }
            return sTemp.ToString().ToLower();
        }

        public static bool EqualString(this string str , string strOther , bool ignoreCase = true)
        {
            if (strOther == str)
                return true;
            return str?.Equals(strOther, StringComparison.CurrentCultureIgnoreCase)??false;
        }
    }
}
