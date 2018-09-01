using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace Anaglyph3DS
{
    class IO
    {
        public static UInt32 readUINT(BinaryReader r, bool little = false)
        {
            byte[] d = r.ReadBytes(4);
            
            if (!little)
                d = reverseBytes(d);
          // littleEndian(d, little);
           
            return BitConverter.ToUInt32(d, 0);
        }

        public static String hexy(UInt32 i)
        {
            return BitConverter.ToString(BitConverter.GetBytes(i), 0);
        }

        public static UInt16 readUSHORT(BinaryReader r, bool little = false)
        {
            byte[] d = r.ReadBytes(2);
            if (!little)
                d = reverseBytes(d);
            //littleEndian(d, little);
            return BitConverter.ToUInt16(d, 0);
        }

        public static byte[] reverseBytes(byte[] inn)
        {
            int l = inn.Length;
            byte[] nb = new byte[l];
            for (int i = 0 ; i < l ; i++)
                nb[i] = inn[l-1-i];
            return nb;
        }

        public static byte[] littleEndian(byte[] inn, bool littleize)
        {
            if (littleize != BitConverter.IsLittleEndian) 
                return reverseBytes(inn);
            return inn;
        }

    }
}
