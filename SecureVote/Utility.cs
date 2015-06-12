using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecureVote
{
    class Utility
    {
        public static byte[] UTF8ToBytes(String strUTF)
        {
            return System.Text.Encoding.UTF8.GetBytes(strUTF);
        }

        public static String BytesToHexString(Byte[] bytes)
        {
            String plainHex = "";

            //byte array to hex 
            for (int i = 0; i < bytes.Length; i++)
            {
                int val = bytes[i];
                plainHex += val.ToString("X2");
            }

            return plainHex;
        }

        public static Byte[] HexStringToBytes(String strHex)
        {
            return Enumerable.Range(0, strHex.Length)
    .Where(x => x % 2 == 0)
    .Select(x => Convert.ToByte(strHex.Substring(x, 2), 16))
    .ToArray();
            /*
            Byte[] bytes = new Byte[strHex.Length / 2];
            for (int i = 0; i < strHex.Length; i += 2)
            {
                bytes[i] = Convert.ToByte(strHex.Substring(i, 2));
            }

            return bytes;
             */
        }

        public static String BytesToUTF8(Byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
