namespace ThoughtWorks.QRCode.Codec.Data
{
    using System;
    using System.Collections;
    using ThoughtWorks.QRCode.Codec.Ecc;
    using ThoughtWorks.QRCode.Codec.Reader.Pattern;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.Geom;

    public class QRCodeSymbol
    {
        internal int version;
        internal int errorCollectionLevel;
        internal int maskPattern;
        internal int dataCapacity;
        internal bool[][] moduleMatrix;
        internal int width;
        internal int height;
        internal Point[][] alignmentPattern;
        internal int[][] numErrorCollectionCode = new int[][] { 
            new int[] { 7, 10, 13, 0x11 }, new int[] { 10, 0x10, 0x16, 0x1c }, new int[] { 15, 0x1a, 0x24, 0x2c }, new int[] { 20, 0x24, 0x34, 0x40 }, new int[] { 0x1a, 0x30, 0x48, 0x58 }, new int[] { 0x24, 0x40, 0x60, 0x70 }, new int[] { 40, 0x48, 0x6c, 130 }, new int[] { 0x30, 0x58, 0x84, 0x9c }, new int[] { 60, 110, 160, 0xc0 }, new int[] { 0x48, 130, 0xc0, 0xe0 }, new int[] { 80, 150, 0xe0, 0x108 }, new int[] { 0x60, 0xb0, 260, 0x134 }, new int[] { 0x68, 0xc6, 0x120, 0x160 }, new int[] { 120, 0xd8, 320, 0x180 }, new int[] { 0x84, 240, 360, 0x1b0 }, new int[] { 0x90, 280, 0x198, 480 },
            new int[] { 0xa8, 0x134, 0x1c0, 0x214 }, new int[] { 180, 0x152, 0x1f8, 0x24c }, new int[] { 0xc4, 0x16c, 0x222, 650 }, new int[] { 0xe0, 0x1a0, 600, 700 }, new int[] { 0xe0, 0x1ba, 0x284, 750 }, new int[] { 0xfc, 0x1dc, 690, 0x330 }, new int[] { 270, 0x1f8, 750, 900 }, new int[] { 300, 560, 810, 960 }, new int[] { 0x138, 0x24c, 870, 0x41a }, new int[] { 0x150, 0x284, 0x3b8, 0x456 }, new int[] { 360, 700, 0x3fc, 0x4b0 }, new int[] { 390, 0x2d8, 0x41a, 0x4ec }, new int[] { 420, 0x310, 0x474, 0x546 }, new int[] { 450, 0x32c, 0x4b0, 0x5a0 }, new int[] { 480, 0x364, 0x50a, 0x5fa }, new int[] { 510, 0x39c, 0x546, 0x654 },
            new int[] { 540, 980, 0x5a0, 0x6ae }, new int[] { 570, 0x40c, 0x5fa, 0x708 }, new int[] { 570, 0x428, 0x636, 0x762 }, new int[] { 600, 0x460, 0x690, 0x7bc }, new int[] { 630, 0x4b4, 0x6ea, 0x834 }, new int[] { 660, 0x4ec, 0x744, 0x8ac }, new int[] { 720, 0x524, 0x79e, 0x906 }, new int[] { 750, 0x55c, 0x7f8, 0x97e }
        };
        internal int[][] numRSBlocks = new int[][] { 
            new int[] { 1, 1, 1, 1 }, new int[] { 1, 1, 1, 1 }, new int[] { 1, 1, 2, 2 }, new int[] { 1, 2, 2, 4 }, new int[] { 1, 2, 4, 4 }, new int[] { 2, 4, 4, 4 }, new int[] { 2, 4, 6, 5 }, new int[] { 2, 4, 6, 6 }, new int[] { 2, 5, 8, 8 }, new int[] { 4, 5, 8, 8 }, new int[] { 4, 5, 8, 11 }, new int[] { 4, 8, 10, 11 }, new int[] { 4, 9, 12, 0x10 }, new int[] { 4, 9, 0x10, 0x10 }, new int[] { 6, 10, 12, 0x12 }, new int[] { 6, 10, 0x11, 0x10 },
            new int[] { 6, 11, 0x10, 0x13 }, new int[] { 6, 13, 0x12, 0x15 }, new int[] { 7, 14, 0x15, 0x19 }, new int[] { 8, 0x10, 20, 0x19 }, new int[] { 8, 0x11, 0x17, 0x19 }, new int[] { 9, 0x11, 0x17, 0x22 }, new int[] { 9, 0x12, 0x19, 30 }, new int[] { 10, 20, 0x1b, 0x20 }, new int[] { 12, 0x15, 0x1d, 0x23 }, new int[] { 12, 0x17, 0x22, 0x25 }, new int[] { 12, 0x19, 0x22, 40 }, new int[] { 13, 0x1a, 0x23, 0x2a }, new int[] { 14, 0x1c, 0x26, 0x2d }, new int[] { 15, 0x1d, 40, 0x30 }, new int[] { 0x10, 0x1f, 0x2b, 0x33 }, new int[] { 0x11, 0x21, 0x2d, 0x36 },
            new int[] { 0x12, 0x23, 0x30, 0x39 }, new int[] { 0x13, 0x25, 0x33, 60 }, new int[] { 0x13, 0x26, 0x35, 0x3f }, new int[] { 20, 40, 0x38, 0x42 }, new int[] { 0x15, 0x2b, 0x3b, 70 }, new int[] { 0x16, 0x2d, 0x3e, 0x4a }, new int[] { 0x18, 0x2f, 0x41, 0x4d }, new int[] { 0x19, 0x31, 0x44, 0x51 }
        };

        public QRCodeSymbol(bool[][] moduleMatrix)
        {
            this.moduleMatrix = moduleMatrix;
            this.width = moduleMatrix.Length;
            this.height = moduleMatrix[0].Length;
            this.initialize();
        }

        private int calcDataCapacity()
        {
            int num = 0;
            int num2 = 0;
            int version = this.Version;
            if (version <= 6)
            {
                num2 = 0x1f;
            }
            else
            {
                num2 = 0x43;
            }
            int num4 = (version / 7) + 2;
            int num5 = (version == 1) ? 0xc0 : (0xc0 + (((num4 * num4) - 3) * 0x19));
            num = ((num5 + (8 * version)) + 2) - ((num4 - 2) * 10);
            return ((((this.width * this.width) - num) - num2) / 8);
        }

        internal virtual void decodeFormatInformation(bool[] formatInformation)
        {
            if (!formatInformation[4])
            {
                if (formatInformation[3])
                {
                    this.errorCollectionLevel = 0;
                }
                else
                {
                    this.errorCollectionLevel = 1;
                }
            }
            else if (formatInformation[3])
            {
                this.errorCollectionLevel = 2;
            }
            else
            {
                this.errorCollectionLevel = 3;
            }
            for (int i = 2; i >= 0; i--)
            {
                if (formatInformation[i])
                {
                    this.maskPattern += ((int) 1) << i;
                }
            }
        }

        internal virtual bool[][] generateMaskPattern()
        {
            int maskPatternReferer = this.MaskPatternReferer;
            int width = this.Width;
            int height = this.Height;
            bool[][] flagArray = new bool[width][];
            for (int i = 0; i < width; i++)
            {
                flagArray[i] = new bool[height];
            }
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    if (!this.isInFunctionPattern(k, j))
                    {
                        switch (maskPatternReferer)
                        {
                            case 0:
                                if (((j + k) % 2) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 1:
                                if ((j % 2) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 2:
                                if ((k % 3) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 3:
                                if (((j + k) % 3) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 4:
                                if ((((j / 2) + (k / 3)) % 2) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 5:
                                if ((((j * k) % 2) + ((j * k) % 3)) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 6:
                                if (((((j * k) % 2) + ((j * k) % 3)) % 2) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;

                            case 7:
                                if (((((j * k) % 3) + ((j + k) % 2)) % 2) == 0)
                                {
                                    flagArray[k][j] = true;
                                }
                                break;
                        }
                    }
                }
            }
            return flagArray;
        }

        public virtual bool getElement(int x, int y) => 
            this.moduleMatrix[x][y];

        internal virtual void initialize()
        {
            this.version = (this.width - 0x11) / 4;
            Point[][] pointArray = new Point[1][];
            for (int i = 0; i < 1; i++)
            {
                pointArray[i] = new Point[1];
            }
            int[] numArray = new int[1];
            if ((this.version >= 2) && (this.version <= 40))
            {
                numArray = LogicalSeed.getSeed(this.version);
                Point[][] pointArray2 = new Point[numArray.Length][];
                for (int k = 0; k < numArray.Length; k++)
                {
                    pointArray2[k] = new Point[numArray.Length];
                }
                pointArray = pointArray2;
            }
            for (int j = 0; j < numArray.Length; j++)
            {
                for (int k = 0; k < numArray.Length; k++)
                {
                    pointArray[k][j] = new Point(numArray[k], numArray[j]);
                }
            }
            this.alignmentPattern = pointArray;
            this.dataCapacity = this.calcDataCapacity();
            bool[] formatInformation = this.readFormatInformation();
            this.decodeFormatInformation(formatInformation);
            this.unmask();
        }

        public virtual bool isInFunctionPattern(int targetX, int targetY)
        {
            if ((targetX < 9) && (targetY < 9))
            {
                return true;
            }
            if ((targetX > (this.Width - 9)) && (targetY < 9))
            {
                return true;
            }
            if ((targetX < 9) && (targetY > (this.Height - 9)))
            {
                return true;
            }
            if (this.version >= 7)
            {
                if ((targetX > (this.Width - 12)) && (targetY < 6))
                {
                    return true;
                }
                if ((targetX < 6) && (targetY > (this.Height - 12)))
                {
                    return true;
                }
            }
            if ((targetX == 6) || (targetY == 6))
            {
                return true;
            }
            Point[][] alignmentPattern = this.AlignmentPattern;
            int length = alignmentPattern.Length;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (((((j != 0) || (i != 0)) && ((j != (length - 1)) || (i != 0))) && ((j != 0) || (i != (length - 1)))) && ((Math.Abs((int) (alignmentPattern[j][i].X - targetX)) < 3) && (Math.Abs((int) (alignmentPattern[j][i].Y - targetY)) < 3)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal virtual bool[] readFormatInformation()
        {
            int num;
            bool[] source = new bool[15];
            for (num = 0; num <= 5; num++)
            {
                source[num] = this.getElement(8, num);
            }
            source[6] = this.getElement(8, 7);
            source[7] = this.getElement(8, 8);
            source[8] = this.getElement(7, 8);
            for (num = 9; num <= 14; num++)
            {
                source[num] = this.getElement(14 - num, 8);
            }
            int number = 0x5412;
            for (num = 0; num <= 14; num++)
            {
                bool flag = false;
                if ((SystemUtils.URShift(number, num) & 1) == 1)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                if (source[num] == flag)
                {
                    source[num] = false;
                }
                else
                {
                    source[num] = true;
                }
            }
            bool[] flagArray2 = new BCH15_5(source).correct();
            bool[] flagArray3 = new bool[5];
            for (num = 0; num < 5; num++)
            {
                flagArray3[num] = flagArray2[10 + num];
            }
            return flagArray3;
        }

        public virtual void reverseElement(int x, int y)
        {
            this.moduleMatrix[x][y] = !this.moduleMatrix[x][y];
        }

        internal virtual void unmask()
        {
            bool[][] flagArray = this.generateMaskPattern();
            int width = this.Width;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (flagArray[j][i])
                    {
                        this.reverseElement(j, i);
                    }
                }
            }
        }

        public virtual int NumErrorCollectionCode =>
            this.numErrorCollectionCode[this.version - 1][this.errorCollectionLevel];

        public virtual int NumRSBlocks =>
            this.numRSBlocks[this.version - 1][this.errorCollectionLevel];

        public virtual int Version =>
            this.version;

        public virtual string VersionReference
        {
            get
            {
                char[] chArray = new char[] { 'L', 'M', 'Q', 'H' };
                return (Convert.ToString(this.version) + "-" + chArray[this.errorCollectionLevel]);
            }
        }

        public virtual Point[][] AlignmentPattern =>
            this.alignmentPattern;

        public virtual int DataCapacity =>
            this.dataCapacity;

        public virtual int ErrorCollectionLevel =>
            this.errorCollectionLevel;

        public virtual int MaskPatternReferer =>
            this.maskPattern;

        public virtual string MaskPatternRefererAsString
        {
            get
            {
                string str = Convert.ToString(this.MaskPatternReferer, 2);
                int length = str.Length;
                for (int i = 0; i < (3 - length); i++)
                {
                    str = "0" + str;
                }
                return str;
            }
        }

        public virtual int Width =>
            this.width;

        public virtual int Height =>
            this.height;

        public virtual int[] Blocks
        {
            get
            {
                int width = this.Width;
                int height = this.Height;
                int x = width - 1;
                int y = height - 1;
                ArrayList list = ArrayList.Synchronized(new ArrayList(10));
                ArrayList list2 = ArrayList.Synchronized(new ArrayList(10));
                int num5 = 0;
                int num6 = 7;
                int num7 = 0;
                bool flag = true;
                bool flag2 = false;
                bool flag3 = flag;
                do
                {
                    list.Add(this.getElement(x, y));
                    if (this.getElement(x, y))
                    {
                        num5 += ((int) 1) << num6;
                    }
                    num6--;
                    if (num6 == -1)
                    {
                        list2.Add(num5);
                        num6 = 7;
                        num5 = 0;
                    }
                    do
                    {
                        if (flag3 == flag)
                        {
                            if (((x + num7) % 2) == 0)
                            {
                                x--;
                            }
                            else if (y > 0)
                            {
                                x++;
                                y--;
                            }
                            else
                            {
                                x--;
                                if (x == 6)
                                {
                                    x--;
                                    num7 = 1;
                                }
                                flag3 = flag2;
                            }
                        }
                        else if (((x + num7) % 2) == 0)
                        {
                            x--;
                        }
                        else if (y < (height - 1))
                        {
                            x++;
                            y++;
                        }
                        else
                        {
                            x--;
                            if (x == 6)
                            {
                                x--;
                                num7 = 1;
                            }
                            flag3 = flag;
                        }
                    }
                    while (this.isInFunctionPattern(x, y));
                }
                while (x != -1);
                int[] numArray = new int[list2.Count];
                for (int i = 0; i < list2.Count; i++)
                {
                    numArray[i] = (int) list2[i];
                }
                return numArray;
            }
        }
    }
}

