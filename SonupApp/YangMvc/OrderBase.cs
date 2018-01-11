using System;
using System.Collections.Generic;
using System.Text;

namespace YangMvc
{
    public class OrderBase
    {

        public string ParseImei(byte[] cmdBytes, int startPos)
        {
            List<byte> lb = new List<byte>();
            lb.Add(cmdBytes[startPos]);
            lb.Add(cmdBytes[startPos + 1]);
            lb.Add(cmdBytes[startPos + 2]);
            lb.Add(cmdBytes[startPos + 3]);

            int b = (lb[0] >= 0x80 ? 8 : 0) + (lb[1] >= 0x80 ? 4 : 0) + (lb[2] >= 0x80 ? 2 : 0) + (lb[3] >= 0x80 ? 1 : 0);

            string code = (130 + b).ToString()
                + (lb[0] >= 0x80 ? lb[0] - 0x80 : lb[0]).ToString().PadLeft(2, '0')
                + (lb[1] >= 0x80 ? lb[1] - 0x80 : lb[1]).ToString().PadLeft(2, '0')
                + (lb[2] >= 0x80 ? lb[2] - 0x80 : lb[2]).ToString().PadLeft(2, '0')
                + (lb[3] >= 0x80 ? lb[3] - 0x80 : lb[3]).ToString().PadLeft(2, '0');

            return code;
        }

        public static byte[] PackImei(string sn)
        {
            sn = sn.Substring(1);
            int i0 = int.Parse(sn.Substring(0, 2)) - 30;
            i0 = i0 % 16;
            byte n1 = Convert.ToByte(byte.Parse(sn.Substring(2, 2)) + 0x80 * (i0 & 8) / 8);
            byte n2 = Convert.ToByte(byte.Parse(sn.Substring(4, 2)) + 0x80 * (i0 & 4) / 4);
            byte n3 = Convert.ToByte(byte.Parse(sn.Substring(6, 2)) + 0x80 * (i0 & 2) / 2);
            byte n4 = Convert.ToByte(byte.Parse(sn.Substring(8, 2)) + 0x80 * (i0 & 1) / 1);

            byte[] imeis = new byte[4];
            imeis[0] = n1;
            imeis[1] = n2;
            imeis[2] = n3;
            imeis[3] = n4;
            return imeis;
        }

        public static byte GetCheckCode(byte[] cmdBytes)
        {
            byte code = (byte)0;
            for (int i = 0; i < cmdBytes.Length - 2; i++)
            {
                code = Convert.ToByte(code ^ cmdBytes[i]);
            }
            return code;
        }


    }
}
