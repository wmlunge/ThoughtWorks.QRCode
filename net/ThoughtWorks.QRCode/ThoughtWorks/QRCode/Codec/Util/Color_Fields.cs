namespace ThoughtWorks.QRCode.Codec.Util
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Size=1)]
    public struct Color_Fields
    {
        public static readonly int GRAY;
        public static readonly int LIGHTGRAY;
        public static readonly int DARKGRAY;
        public static readonly int BLACK;
        public static readonly int WHITE;
        public static readonly int BLUE;
        public static readonly int GREEN;
        public static readonly int LIGHTBLUE;
        public static readonly int LIGHTGREEN;
        public static readonly int RED;
        public static readonly int ORANGE;
        public static readonly int LIGHTRED;
        static Color_Fields()
        {
            GRAY = 0xaaaaaa;
            LIGHTGRAY = 0xbbbbbb;
            DARKGRAY = 0x444444;
            BLACK = 0;
            WHITE = 0xffffff;
            BLUE = 0x8888ff;
            GREEN = 0x88ff88;
            LIGHTBLUE = 0xbbbbff;
            LIGHTGREEN = 0xbbffbb;
            RED = 0xff88888;
            ORANGE = 0xffff88;
            LIGHTRED = 0xffbbbb;
        }
    }
}

