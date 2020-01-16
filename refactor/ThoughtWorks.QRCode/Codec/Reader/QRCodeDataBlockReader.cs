namespace ThoughtWorks.QRCode.Codec.Reader
{
    using System;
    using System.IO;
    using ThoughtWorks.QRCode.Codec;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.ExceptionHandler;

    public class QRCodeDataBlockReader
    {
        private const int MODE_NUMBER = 1;
        private const int MODE_ROMAN_AND_NUMBER = 2;
        private const int MODE_8BIT_BYTE = 4;
        private const int MODE_KANJI = 8;
        internal int[] blocks;
        internal int dataLengthMode;
        internal int blockPointer = 0;
        internal int bitPointer = 7;
        internal int dataLength = 0;
        internal int numErrorCorrectionCode;
        internal DebugCanvas canvas;
        private int[][] sizeOfDataLengthInfo = new int[][] { new int[] { 10, 9, 8, 8 }, new int[] { 12, 11, 0x10, 10 }, new int[] { 14, 13, 0x10, 12 } };

        public QRCodeDataBlockReader(int[] blocks, int version, int numErrorCorrectionCode)
        {
            this.blocks = blocks;
            this.numErrorCorrectionCode = numErrorCorrectionCode;
            if (version <= 9)
            {
                this.dataLengthMode = 0;
            }
            else if ((version >= 10) && (version <= 0x1a))
            {
                this.dataLengthMode = 1;
            }
            else if ((version >= 0x1b) && (version <= 40))
            {
                this.dataLengthMode = 2;
            }
            this.canvas = QRCodeDecoder.Canvas;
        }

        public virtual sbyte[] get8bitByteArray(int dataLength)
        {
            int num = dataLength;
            int num2 = 0;
            MemoryStream stream = new MemoryStream();
            do
            {
                this.canvas.println("Length: " + num);
                num2 = this.getNextBits(8);
                stream.WriteByte((byte) num2);
                num--;
            }
            while (num > 0);
            return SystemUtils.ToSByteArray(stream.ToArray());
        }

        internal virtual string get8bitByteString(int dataLength)
        {
            int num = dataLength;
            int num2 = 0;
            string str = "";
            do
            {
                num2 = this.getNextBits(8);
                str = str + ((char) num2);
                num--;
            }
            while (num > 0);
            return str;
        }

        internal virtual int getDataLength(int modeIndicator)
        {
            int index = 0;
            while (true)
            {
                if ((modeIndicator >> index) == 1)
                {
                    return this.getNextBits(this.sizeOfDataLengthInfo[this.dataLengthMode][index]);
                }
                index++;
            }
        }

        internal virtual string getFigureString(int dataLength)
        {
            int num = dataLength;
            int num2 = 0;
            string str = "";
            do
            {
                if (num >= 3)
                {
                    num2 = this.getNextBits(10);
                    if (num2 < 100)
                    {
                        str = str + "0";
                    }
                    if (num2 < 10)
                    {
                        str = str + "0";
                    }
                    num -= 3;
                }
                else
                {
                    switch (num)
                    {
                        case 2:
                            num2 = this.getNextBits(7);
                            if (num2 < 10)
                            {
                                str = str + "0";
                            }
                            num -= 2;
                            break;

                        case 1:
                            num2 = this.getNextBits(4);
                            num--;
                            break;
                    }
                }
                str = str + Convert.ToString(num2);
            }
            while (num > 0);
            return str;
        }

        internal virtual string getKanjiString(int dataLength)
        {
            int num = dataLength;
            int num2 = 0;
            string str = "";
            do
            {
                num2 = this.getNextBits(13);
                int num3 = num2 % 0xc0;
                int num4 = num2 / 0xc0;
                int num5 = (num4 << 8) + num3;
                int num6 = 0;
                if ((num5 + 0x8140) <= 0x9ffc)
                {
                    num6 = num5 + 0x8140;
                }
                else
                {
                    num6 = num5 + 0xc140;
                }
                sbyte[] sbyteArray = new sbyte[] { (sbyte) (num6 >> 8), (sbyte) (num6 & 0xff) };
                str = str + new string(SystemUtils.ToCharArray(SystemUtils.ToByteArray(sbyteArray)));
                num--;
            }
            while (num > 0);
            return str;
        }

        internal virtual int getNextBits(int numBits)
        {
            int num3;
            int num4;
            int num = 0;
            if (numBits < (this.bitPointer + 1))
            {
                int num2 = 0;
                for (num3 = 0; num3 < numBits; num3++)
                {
                    num2 += ((int) 1) << num3;
                }
                num2 = num2 << ((this.bitPointer - numBits) + 1);
                num = (this.blocks[this.blockPointer] & num2) >> ((this.bitPointer - numBits) + 1);
                this.bitPointer -= numBits;
                return num;
            }
            if (numBits < ((this.bitPointer + 1) + 8))
            {
                num4 = 0;
                for (num3 = 0; num3 < (this.bitPointer + 1); num3++)
                {
                    num4 += ((int) 1) << num3;
                }
                num = (this.blocks[this.blockPointer] & num4) << (numBits - (this.bitPointer + 1));
                this.blockPointer++;
                num += this.blocks[this.blockPointer] >> (8 - (numBits - (this.bitPointer + 1)));
                this.bitPointer -= numBits % 8;
                if (this.bitPointer < 0)
                {
                    this.bitPointer = 8 + this.bitPointer;
                }
                return num;
            }
            if (numBits < ((this.bitPointer + 1) + 0x10))
            {
                num4 = 0;
                int num5 = 0;
                for (num3 = 0; num3 < (this.bitPointer + 1); num3++)
                {
                    num4 += ((int) 1) << num3;
                }
                int num6 = (this.blocks[this.blockPointer] & num4) << (numBits - (this.bitPointer + 1));
                this.blockPointer++;
                int num7 = this.blocks[this.blockPointer] << (numBits - ((this.bitPointer + 1) + 8));
                this.blockPointer++;
                for (num3 = 0; num3 < (numBits - ((this.bitPointer + 1) + 8)); num3++)
                {
                    num5 += ((int) 1) << num3;
                }
                num5 = num5 << (8 - (numBits - ((this.bitPointer + 1) + 8)));
                int num8 = (this.blocks[this.blockPointer] & num5) >> (8 - (numBits - ((this.bitPointer + 1) + 8)));
                num = (num6 + num7) + num8;
                this.bitPointer -= (numBits - 8) % 8;
                if (this.bitPointer < 0)
                {
                    this.bitPointer = 8 + this.bitPointer;
                }
                return num;
            }
            Console.Out.WriteLine("ERROR!");
            return 0;
        }

        internal virtual string getRomanAndFigureString(int dataLength)
        {
            int num = dataLength;
            int index = 0;
            string str = "";
            char[] chArray = new char[] { 
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',
                'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',
                'W', 'X', 'Y', 'Z', ' ', '$', '%', '*', '+', '-', '.', '/', ':'
            };
            do
            {
                if (num > 1)
                {
                    index = this.getNextBits(11);
                    int num3 = index / 0x2d;
                    int num4 = index % 0x2d;
                    str = str + Convert.ToString(chArray[num3]) + Convert.ToString(chArray[num4]);
                    num -= 2;
                }
                else if (num == 1)
                {
                    index = this.getNextBits(6);
                    str = str + Convert.ToString(chArray[index]);
                    num--;
                }
            }
            while (num > 0);
            return str;
        }

        internal virtual int guessMode(int mode)
        {
            switch (mode)
            {
                case 3:
                    return 1;

                case 5:
                    return 4;

                case 6:
                    return 4;

                case 7:
                    return 4;

                case 9:
                    return 8;

                case 10:
                    return 8;

                case 11:
                    return 8;

                case 12:
                    return 4;

                case 13:
                    return 4;

                case 14:
                    return 4;

                case 15:
                    return 4;
            }
            return 8;
        }

        internal virtual int NextMode
        {
            get
            {
                if (this.blockPointer > ((this.blocks.Length - this.numErrorCorrectionCode) - 2))
                {
                    return 0;
                }
                return this.getNextBits(4);
            }
        }

        public virtual sbyte[] DataByte
        {
            get
            {
                this.canvas.println("Reading data blocks.");
                MemoryStream stream = new MemoryStream();
                try
                {
                    int num;
                Label_0019:
                    num = this.NextMode;
                    if (num == 0)
                    {
                        if (stream.Length <= 0L)
                        {
                            throw new InvalidDataBlockException("Empty data block");
                        }
                    }
                    else
                    {
                        if ((((num != 1) && (num != 2)) && (num != 4)) && (num != 8))
                        {
                            throw new InvalidDataBlockException(string.Concat(new object[] { "Invalid mode: ", num, " in (block:", this.blockPointer, " bit:", this.bitPointer, ")" }));
                        }
                        this.dataLength = this.getDataLength(num);
                        if (this.dataLength < 1)
                        {
                            throw new InvalidDataBlockException("Invalid data length: " + this.dataLength);
                        }
                        switch (num)
                        {
                            case 1:
                            {
                                sbyte[] sbyteArray = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getFigureString(this.dataLength)));
                                stream.Write(SystemUtils.ToByteArray(sbyteArray), 0, sbyteArray.Length);
                                break;
                            }
                            case 2:
                            {
                                sbyte[] sbyteArray = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getRomanAndFigureString(this.dataLength)));
                                stream.Write(SystemUtils.ToByteArray(sbyteArray), 0, sbyteArray.Length);
                                break;
                            }
                            case 4:
                            {
                                sbyte[] sbyteArray = this.get8bitByteArray(this.dataLength);
                                stream.Write(SystemUtils.ToByteArray(sbyteArray), 0, sbyteArray.Length);
                                break;
                            }
                            case 8:
                            {
                                sbyte[] sbyteArray = SystemUtils.ToSByteArray(SystemUtils.ToByteArray(this.getKanjiString(this.dataLength)));
                                stream.Write(SystemUtils.ToByteArray(sbyteArray), 0, sbyteArray.Length);
                                break;
                            }
                        }
                        goto Label_0019;
                    }
                }
                catch (IndexOutOfRangeException exception)
                {
                    SystemUtils.WriteStackTrace(exception, Console.Error);
                    throw new InvalidDataBlockException(string.Concat(new object[] { "Data Block Error in (block:", this.blockPointer, " bit:", this.bitPointer, ")" }));
                }
                catch (IOException exception2)
                {
                    throw new InvalidDataBlockException(exception2.Message);
                }
                return SystemUtils.ToSByteArray(stream.ToArray());
            }
        }

        public virtual string DataString
        {
            get
            {
                this.canvas.println("Reading data blocks...");
                string str = "";
                while (true)
                {
                    int nextMode = this.NextMode;
                    this.canvas.println("mode: " + nextMode);
                    switch (nextMode)
                    {
                        case 0:
                            Console.Out.WriteLine("");
                            return str;
                    }
                    this.dataLength = this.getDataLength(nextMode);
                    this.canvas.println(Convert.ToString(this.blocks[this.blockPointer]));
                    Console.Out.WriteLine("length: " + this.dataLength);
                    switch (nextMode)
                    {
                        case 1:
                            str = str + this.getFigureString(this.dataLength);
                            break;

                        case 2:
                            str = str + this.getRomanAndFigureString(this.dataLength);
                            break;

                        case 4:
                            str = str + this.get8bitByteString(this.dataLength);
                            break;

                        case 8:
                            str = str + this.getKanjiString(this.dataLength);
                            break;
                    }
                }
            }
        }
    }
}

