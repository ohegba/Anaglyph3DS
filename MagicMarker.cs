using System;
using System.Collections.Generic;

using System.Text;

namespace Anaglyph3DS
{
    class MagicMarker
    {
        public const UInt16 START_OF_IMAGE = 0xFFD8;
        public const UInt16 APP1 = 0xFFE1;
        public const UInt16 APP2 = 0xFFE2;

        public const UInt16 DQT = 0xFFDB;
        public const UInt16 DHT = 0xFFC4;
        public const UInt16 DRI = 0xFFDD;

        public const UInt16 SOF = 0xFFC0;
        public const UInt16 SOS = 0xFFDA;

        public const UInt16 EOI = 0xFFD9;

        public const UInt32 LEMP = 0x49492A00;

        public const String MP_EXTENSIONS_ID_STRING = "MPF\0";
    }
}
