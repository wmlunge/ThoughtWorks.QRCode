namespace ThoughtWorks.QRCode.Codec
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.Properties;

    public class QRCodeEncoder
    {
        internal ERROR_CORRECTION qrcodeErrorCorrect = ERROR_CORRECTION.M;
        internal ENCODE_MODE qrcodeEncodeMode = ENCODE_MODE.BYTE;
        internal int qrcodeVersion = 7;
        internal int qrcodeStructureappendN = 0;
        internal int qrcodeStructureappendM = 0;
        internal int qrcodeStructureappendParity = 0;
        internal System.Drawing.Color qrCodeBackgroundColor = System.Drawing.Color.White;
        internal System.Drawing.Color qrCodeForegroundColor = System.Drawing.Color.Black;
        internal int qrCodeScale = 4;
        internal string qrcodeStructureappendOriginaldata = "";

        private static sbyte[] calculateByteArrayBits(sbyte[] xa, sbyte[] xb, string ind)
        {
            sbyte[] numArray2;
            sbyte[] numArray3;
            if (xa.Length > xb.Length)
            {
                numArray2 = new sbyte[xa.Length];
                xa.CopyTo(numArray2, 0);
                numArray3 = new sbyte[xb.Length];
                xb.CopyTo(numArray3, 0);
            }
            else
            {
                numArray2 = new sbyte[xb.Length];
                xb.CopyTo(numArray2, 0);
                numArray3 = new sbyte[xa.Length];
                xa.CopyTo(numArray3, 0);
            }
            int length = numArray2.Length;
            int num2 = numArray3.Length;
            sbyte[] numArray = new sbyte[length];
            for (int i = 0; i < length; i++)
            {
                if (i < num2)
                {
                    if (ind == "xor")
                    {
                        numArray[i] = (sbyte)(numArray2[i] ^ numArray3[i]);
                    }
                    else
                    {
                        numArray[i] = (sbyte)(numArray2[i] | numArray3[i]);
                    }
                }
                else
                {
                    numArray[i] = numArray2[i];
                }
            }
            return numArray;
        }

        private static sbyte[] calculateRSECC(sbyte[] codewords, sbyte rsEccCodewords, sbyte[] rsBlockOrder, int maxDataCodewords, int maxCodewords)
        {
            int num;
            sbyte[][] numArray = new sbyte[0x100][];
            for (num = 0; num < 0x100; num++)
            {
                numArray[num] = new sbyte[rsEccCodewords];
            }
            try
            {
                string name = "rsc" + rsEccCodewords.ToString();
                MemoryStream stream = new MemoryStream((byte[])Resources.ResourceManager.GetObject(name));
                BufferedStream sourceStream = new BufferedStream(stream);
                for (num = 0; num < 0x100; num++)
                {
                    SystemUtils.ReadInput(sourceStream, numArray[num], 0, numArray[num].Length);
                }
                sourceStream.Close();
                stream.Close();
            }
            catch (Exception exception)
            {
                SystemUtils.WriteStackTrace(exception, Console.Error);
            }
            int index = 0;
            int num3 = 0;
            int num4 = 0;
            sbyte[][] numArray2 = new sbyte[rsBlockOrder.Length][];
            sbyte[] destinationArray = new sbyte[maxCodewords];
            Array.Copy(codewords, 0, destinationArray, 0, codewords.Length);
            for (index = 0; index < rsBlockOrder.Length; index++)
            {
                numArray2[index] = new sbyte[(rsBlockOrder[index] & 0xff) - rsEccCodewords];
            }
            for (index = 0; index < maxDataCodewords; index++)
            {
                numArray2[num4][num3] = codewords[index];
                num3++;
                if (num3 >= ((rsBlockOrder[num4] & 0xff) - rsEccCodewords))
                {
                    num3 = 0;
                    num4++;
                }
            }
            for (num4 = 0; num4 < rsBlockOrder.Length; num4++)
            {
                sbyte[] array = new sbyte[numArray2[num4].Length];
                numArray2[num4].CopyTo(array, 0);
                int num5 = rsBlockOrder[num4] & 0xff;
                int num6 = num5 - rsEccCodewords;
                for (num3 = num6; num3 > 0; num3--)
                {
                    sbyte num7 = array[0];
                    if (num7 != 0)
                    {
                        sbyte[] numArray5 = new sbyte[array.Length - 1];
                        Array.Copy(array, 1, numArray5, 0, array.Length - 1);
                        sbyte[] xb = numArray[num7 & 0xff];
                        array = calculateByteArrayBits(numArray5, xb, "xor");
                    }
                    else
                    {
                        sbyte[] numArray7;
                        if (rsEccCodewords < array.Length)
                        {
                            numArray7 = new sbyte[array.Length - 1];
                            Array.Copy(array, 1, numArray7, 0, array.Length - 1);
                            array = new sbyte[numArray7.Length];
                            numArray7.CopyTo(array, 0);
                        }
                        else
                        {
                            numArray7 = new sbyte[rsEccCodewords];
                            Array.Copy(array, 1, numArray7, 0, array.Length - 1);
                            numArray7[rsEccCodewords - 1] = 0;
                            array = new sbyte[numArray7.Length];
                            numArray7.CopyTo(array, 0);
                        }
                    }
                }
                Array.Copy(array, 0, destinationArray, codewords.Length + (num4 * rsEccCodewords), (byte)rsEccCodewords);
            }
            return destinationArray;
        }

        public virtual bool[][] calQrcode(byte[] qrcodeData)
        {
            int[] numArray3;
            int num3;
            int num4;
            int num7;
            string str;
            MemoryStream stream;
            BufferedStream stream2;
            Exception exception;
            int num18;
            int index = 0;
            int length = qrcodeData.Length;
            int[] data = new int[length + 0x20];
            sbyte[] bits = new sbyte[length + 0x20];
            if (length <= 0)
            {
                bool[][] flagArray4 = new bool[1][];
                flagArray4[0] = new bool[1];
                return flagArray4;
            }
            if (this.qrcodeStructureappendN > 1)
            {
                data[0] = 3;
                bits[0] = 4;
                data[1] = this.qrcodeStructureappendM - 1;
                bits[1] = 4;
                data[2] = this.qrcodeStructureappendN - 1;
                bits[2] = 4;
                data[3] = this.qrcodeStructureappendParity;
                bits[3] = 8;
                index = 4;
            }
            bits[index] = 4;
            switch (this.qrcodeEncodeMode)
            {
                case ENCODE_MODE.ALPHA_NUMERIC:
                    numArray3 = new int[] {
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2,
                        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4,
                        4, 4, 4, 4, 4, 4, 4, 4, 4
                    };
                    data[index] = 2;
                    index++;
                    data[index] = length;
                    bits[index] = 9;
                    num3 = index;
                    index++;
                    for (num4 = 0; num4 < length; num4++)
                    {
                        char ch = (char)qrcodeData[num4];
                        sbyte num5 = 0;
                        if ((ch >= '0') && (ch < ':'))
                        {
                            num5 = (sbyte)(ch - '0');
                        }
                        else if ((ch >= 'A') && (ch < '['))
                        {
                            num5 = (sbyte)(ch - '7');
                        }
                        else
                        {
                            if (ch == ' ')
                            {
                                num5 = 0x24;
                            }
                            if (ch == '$')
                            {
                                num5 = 0x25;
                            }
                            if (ch == '%')
                            {
                                num5 = 0x26;
                            }
                            if (ch == '*')
                            {
                                num5 = 0x27;
                            }
                            if (ch == '+')
                            {
                                num5 = 40;
                            }
                            if (ch == '-')
                            {
                                num5 = 0x29;
                            }
                            if (ch == '.')
                            {
                                num5 = 0x2a;
                            }
                            if (ch == '/')
                            {
                                num5 = 0x2b;
                            }
                            if (ch == ':')
                            {
                                num5 = 0x2c;
                            }
                        }
                        if ((num4 % 2) == 0)
                        {
                            data[index] = num5;
                            bits[index] = 6;
                        }
                        else
                        {
                            data[index] = (data[index] * 0x2d) + num5;
                            bits[index] = 11;
                            if (num4 < (length - 1))
                            {
                                index++;
                            }
                        }
                    }
                    index++;
                    break;

                case ENCODE_MODE.NUMERIC:
                    numArray3 = new int[] {
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2,
                        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 4, 4, 4, 4, 4,
                        4, 4, 4, 4, 4, 4, 4, 4, 4
                    };
                    data[index] = 1;
                    index++;
                    data[index] = length;
                    bits[index] = 10;
                    num3 = index;
                    index++;
                    num4 = 0;
                    while (num4 < length)
                    {
                        if ((num4 % 3) == 0)
                        {
                            data[index] = qrcodeData[num4] - 0x30;
                            bits[index] = 4;
                        }
                        else
                        {
                            data[index] = (data[index] * 10) + (qrcodeData[num4] - 0x30);
                            if ((num4 % 3) == 1)
                            {
                                bits[index] = 7;
                            }
                            else
                            {
                                bits[index] = 10;
                                if (num4 < (length - 1))
                                {
                                    index++;
                                }
                            }
                        }
                        num4++;
                    }
                    index++;
                    break;

                default:
                    numArray3 = new int[] {
                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 8, 8, 8, 8, 8,
                        8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
                        8, 8, 8, 8, 8, 8, 8, 8, 8
                    };
                    data[index] = 4;
                    index++;
                    data[index] = length;
                    bits[index] = 8;
                    num3 = index;
                    index++;
                    for (num4 = 0; num4 < length; num4++)
                    {
                        data[num4 + index] = qrcodeData[num4] & 0xff;
                        bits[num4 + index] = 8;
                    }
                    index += length;
                    break;
            }
            int num6 = 0;
            for (num4 = 0; num4 < index; num4++)
            {
                num6 += bits[num4];
            }
            switch (this.qrcodeErrorCorrect)
            {
                case ERROR_CORRECTION.L:
                    num7 = 1;
                    break;

                case ERROR_CORRECTION.Q:
                    num7 = 3;
                    break;

                case ERROR_CORRECTION.H:
                    num7 = 2;
                    break;

                default:
                    num7 = 0;
                    break;
            }
            int[][] numArray4 = new int[][] { new int[] {
                0, 0x80, 0xe0, 0x160, 0x200, 0x2b0, 0x360, 0x3e0, 0x4d0, 0x5b0, 0x6c0, 0x7f0, 0x910, 0xa70, 0xb68, 0xcf8,
                0xe28, 0xfd8, 0x1198, 0x1398, 0x14e8, 0x1650, 0x1870, 0x1ae0, 0x1c90, 0x1f40, 0x2130, 0x2340, 0x2548, 0x2798, 0x2ae8, 0x2d78,
                0x3028, 0x32f8, 0x35e8, 0x38a0, 0x3bd0, 0x3e40, 0x41b0, 0x4540, 0x48f0
            }, new int[] {
                0, 0x98, 0x110, 440, 640, 0x360, 0x440, 0x4e0, 0x610, 0x740, 0x890, 0xa20, 0xb90, 0xd60, 0xe68, 0x1058,
                0x1268, 0x1438, 0x1688, 0x18d8, 0x1ae8, 0x1d20, 0x1f70, 0x2230, 0x24b0, 0x27e0, 0x2ad0, 0x2de0, 0x2fd8, 0x32f8, 0x3638, 0x3998,
                0x3d18, 0x40b8, 0x4478, 0x4810, 0x4c10, 0x5030, 0x5470, 0x57e0, 0x5c60
            }, new int[] {
                0, 0x48, 0x80, 0xd0, 0x120, 0x170, 480, 0x210, 0x2b0, 800, 0x3d0, 0x460, 0x4f0, 0x5a0, 0x628, 0x6f8,
                0x7e8, 0x8d8, 0x9c8, 0xaa8, 0xc08, 0xcb0, 0xdd0, 0xe80, 0x1010, 0x10d0, 0x12a0, 0x13a0, 0x14a8, 0x15e8, 0x1748, 0x18c8,
                0x1a68, 0x1c28, 0x1e08, 0x1ed0, 0x20f0, 0x2240, 0x23b0, 0x2630, 0x27e0
            }, new int[] {
                0, 0x68, 0xb0, 0x110, 0x180, 0x1f0, 0x260, 0x2c0, 880, 0x420, 0x4d0, 0x5a0, 0x670, 0x7a0, 0x828, 0x938,
                0xa28, 0xb78, 0xc68, 0xde8, 0xf28, 0x1000, 0x11c0, 0x1330, 0x14c0, 0x1670, 0x1790, 0x1940, 0x1b38, 0x1c78, 0x1ec8, 0x2048,
                0x22d8, 0x2498, 0x2678, 0x2830, 0x2a50, 0x2c90, 0x2ef0, 0x3170, 0x3410
            } };
            int num8 = 0;
            if (this.qrcodeVersion == 0)
            {
                this.qrcodeVersion = 1;
                for (num4 = 1; num4 <= 40; num4++)
                {
                    if (numArray4[num7][num4] >= (num6 + numArray3[this.qrcodeVersion]))
                    {
                        num8 = numArray4[num7][num4];
                        break;
                    }
                    this.qrcodeVersion++;
                }
            }
            else
            {
                num8 = numArray4[num7][this.qrcodeVersion];
            }
            num6 += numArray3[this.qrcodeVersion];
            bits[num3] = (sbyte)(bits[num3] + numArray3[this.qrcodeVersion]);
            int[] numArray5 = new int[] {
                0, 0x1a, 0x2c, 70, 100, 0x86, 0xac, 0xc4, 0xf2, 0x124, 0x15a, 0x194, 0x1d2, 0x214, 0x245, 0x28f,
                0x2dd, 0x32f, 0x385, 0x3df, 0x43d, 0x484, 0x4ea, 0x554, 0x5c2, 0x634, 0x6aa, 0x724, 0x781, 0x803, 0x889, 0x913,
                0x9a1, 0xa33, 0xac9, 0xb3c, 0xbda, 0xc7c, 0xd22, 0xdcc, 0xe7a
            };
            int maxCodewords = numArray5[this.qrcodeVersion];
            int num10 = 0x11 + (this.qrcodeVersion << 2);
            int[] numArray6 = new int[] {
                0, 0, 7, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 3, 3,
                3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 3,
                3, 3, 3, 0, 0, 0, 0, 0, 0
            };
            int num11 = numArray6[this.qrcodeVersion] + (maxCodewords << 3);
            sbyte[] target = new sbyte[num11];
            sbyte[] numArray8 = new sbyte[num11];
            sbyte[] numArray9 = new sbyte[num11];
            sbyte[] numArray10 = new sbyte[15];
            sbyte[] numArray11 = new sbyte[15];
            sbyte[] numArray12 = new sbyte[1];
            sbyte[] numArray13 = new sbyte[0x80];
            try
            {
                str = "qrv" + Convert.ToString(this.qrcodeVersion) + "_" + Convert.ToString(num7);
                stream = new MemoryStream((byte[])Resources.ResourceManager.GetObject(str));
                stream2 = new BufferedStream(stream);
                SystemUtils.ReadInput(stream2, target, 0, target.Length);
                SystemUtils.ReadInput(stream2, numArray8, 0, numArray8.Length);
                SystemUtils.ReadInput(stream2, numArray9, 0, numArray9.Length);
                SystemUtils.ReadInput(stream2, numArray10, 0, numArray10.Length);
                SystemUtils.ReadInput(stream2, numArray11, 0, numArray11.Length);
                SystemUtils.ReadInput(stream2, numArray12, 0, numArray12.Length);
                SystemUtils.ReadInput(stream2, numArray13, 0, numArray13.Length);
                stream2.Close();
                stream.Close();
            }
            catch (Exception exception1)
            {
                exception = exception1;
                SystemUtils.WriteStackTrace(exception, Console.Error);
            }
            sbyte num12 = 1;
            for (sbyte i = 1; i < 0x80; i = (sbyte)(i + 1))
            {
                if (numArray13[i] == 0)
                {
                    num12 = i;
                    break;
                }
            }
            sbyte[] destinationArray = new sbyte[num12];
            Array.Copy(numArray13, 0, destinationArray, 0, (byte)num12);
            sbyte[] numArray15 = new sbyte[] { 0, 1, 2, 3, 4, 5, 7, 8, 8, 8, 8, 8, 8, 8, 8 };
            sbyte[] numArray16 = new sbyte[] { 8, 8, 8, 8, 8, 8, 8, 8, 7, 5, 4, 3, 2, 1, 0 };
            int maxDataCodewords = num8 >> 3;
            int num15 = (4 * this.qrcodeVersion) + 0x11;
            int num16 = num15 * num15;
            sbyte[] numArray17 = new sbyte[num16 + num15];
            try
            {
                str = "qrvfr" + Convert.ToString(this.qrcodeVersion);
                stream = new MemoryStream((byte[])Resources.ResourceManager.GetObject(str));
                stream2 = new BufferedStream(stream);
                SystemUtils.ReadInput(stream2, numArray17, 0, numArray17.Length);
                stream2.Close();
                stream.Close();
            }
            catch (Exception exception2)
            {
                exception = exception2;
                SystemUtils.WriteStackTrace(exception, Console.Error);
            }
            if (num6 <= (num8 - 4))
            {
                data[index] = 0;
                bits[index] = 4;
            }
            else if (num6 < num8)
            {
                data[index] = 0;
                bits[index] = (sbyte)(num8 - num6);
            }
            else if (num6 > num8)
            {
                Console.Out.WriteLine("overflow");
            }
            sbyte[] numArray19 = calculateRSECC(divideDataBy8Bits(data, bits, maxDataCodewords), numArray12[0], destinationArray, maxDataCodewords, maxCodewords);
            sbyte[][] matrixContent = new sbyte[num15][];
            for (int j = 0; j < num15; j++)
            {
                matrixContent[j] = new sbyte[num15];
            }
            for (num4 = 0; num4 < num15; num4++)
            {
                num18 = 0;
                while (num18 < num15)
                {
                    matrixContent[num18][num4] = 0;
                    num18++;
                }
            }
            for (num4 = 0; num4 < maxCodewords; num4++)
            {
                sbyte num19 = numArray19[num4];
                num18 = 7;
                while (num18 >= 0)
                {
                    int num20 = (num4 * 8) + num18;
                    matrixContent[target[num20] & 0xff][numArray8[num20] & 0xff] = (sbyte)((0xff * (num19 & 1)) ^ numArray9[num20]);
                    num19 = (sbyte)SystemUtils.URShift((int)(num19 & 0xff), 1);
                    num18--;
                }
            }
            for (int k = numArray6[this.qrcodeVersion]; k > 0; k--)
            {
                int num22 = (k + (maxCodewords * 8)) - 1;
                matrixContent[target[num22] & 0xff][numArray8[num22] & 0xff] = (sbyte)(0xff ^ numArray9[num22]);
            }
            sbyte num23 = selectMask(matrixContent, numArray6[this.qrcodeVersion] + (maxCodewords * 8));
            sbyte num24 = (sbyte)(((int)1) << num23);
            sbyte num25 = (sbyte)((num7 << 3) | num23);
            string[] strArray = new string[] {
                "101010000010010", "101000100100101", "101111001111100", "101101101001011", "100010111111001", "100000011001110", "100111110010111", "100101010100000", "111011111000100", "111001011110011", "111110110101010", "111100010011101", "110011000101111", "110001100011000", "110110001000001", "110100101110110",
                "001011010001001", "001001110111110", "001110011100111", "001100111010000", "000011101100010", "000001001010101", "000110100001100", "000100000111011", "011010101011111", "011000001101000", "011111100110001", "011101000000110", "010010010110100", "010000110000011", "010111011011010", "010101111101101"
            };
            for (num4 = 0; num4 < 15; num4++)
            {
                sbyte num26 = sbyte.Parse(strArray[num25].Substring(num4, (num4 + 1) - num4));
                matrixContent[numArray15[num4] & 0xff][numArray16[num4] & 0xff] = (sbyte)(num26 * 0xff);
                matrixContent[numArray10[num4] & 0xff][numArray11[num4] & 0xff] = (sbyte)(num26 * 0xff);
            }
            bool[][] flagArray2 = new bool[num15][];
            for (int m = 0; m < num15; m++)
            {
                flagArray2[m] = new bool[num15];
            }
            int num28 = 0;
            for (num4 = 0; num4 < num15; num4++)
            {
                for (num18 = 0; num18 < num15; num18++)
                {
                    if (((matrixContent[num18][num4] & num24) != 0) || (numArray17[num28] == 0x31))
                    {
                        flagArray2[num18][num4] = true;
                    }
                    else
                    {
                        flagArray2[num18][num4] = false;
                    }
                    num28++;
                }
                num28++;
            }
            return flagArray2;
        }

        public virtual int calStructureappendParity(sbyte[] originaldata)
        {
            int index = 0;
            int num3 = 0;
            int length = originaldata.Length;
            if (length > 1)
            {
                num3 = 0;
                while (index < length)
                {
                    num3 ^= originaldata[index] & 0xff;
                    index++;
                }
                return num3;
            }
            return -1;
        }

        private static sbyte[] divideDataBy8Bits(int[] data, sbyte[] bits, int maxDataCodewords)
        {
            bool flag;
            int num8;
            int length = bits.Length;
            int index = 0;
            int num4 = 8;
            int num5 = 0;
            if (length != data.Length)
            {
            }
            for (num8 = 0; num8 < length; num8++)
            {
                num5 += bits[num8];
            }
            int num2 = ((num5 - 1) / 8) + 1;
            sbyte[] numArray = new sbyte[maxDataCodewords];
            for (num8 = 0; num8 < num2; num8++)
            {
                numArray[num8] = 0;
            }
            for (num8 = 0; num8 < length; num8++)
            {
                int num6 = data[num8];
                int num7 = bits[num8];
                flag = true;
                if (num7 == 0)
                {
                    break;
                }
                while (flag)
                {
                    if (num4 > num7)
                    {
                        numArray[index] = (sbyte)((numArray[index] << num7) | num6);
                        num4 -= num7;
                        flag = false;
                    }
                    else
                    {
                        num7 -= num4;
                        numArray[index] = (sbyte)((numArray[index] << num4) | (num6 >> num7));
                        if (num7 == 0)
                        {
                            flag = false;
                        }
                        else
                        {
                            num6 &= (((int)1) << num7) - 1;
                            flag = true;
                        }
                        index++;
                        num4 = 8;
                    }
                }
            }
            if (num4 != 8)
            {
                numArray[index] = (sbyte)(numArray[index] << num4);
            }
            else
            {
                index--;
            }
            if (index < (maxDataCodewords - 1))
            {
                for (flag = true; index < (maxDataCodewords - 1); flag = !flag)
                {
                    index++;
                    if (flag)
                    {
                        numArray[index] = -20;
                    }
                    else
                    {
                        numArray[index] = 0x11;
                    }
                }
            }
            return numArray;
        }

        public virtual Bitmap Encode(string content)
        {
            if (QRCodeUtility.IsUniCode(content))
            {
                return this.Encode(content, Encoding.UTF8);
            }
            return this.Encode(content, Encoding.ASCII);
        }

        public virtual Bitmap Encode(string content, Encoding encoding)
        {
            bool[][] flagArray = this.calQrcode(encoding.GetBytes(content));
            SolidBrush brush = new SolidBrush(this.qrCodeBackgroundColor);
            Bitmap image = new Bitmap((flagArray.Length * this.qrCodeScale) + 1, (flagArray.Length * this.qrCodeScale) + 1);
            Graphics graphics = Graphics.FromImage(image);
            graphics.FillRectangle(brush, new Rectangle(0, 0, image.Width, image.Height));
            brush.Color = this.qrCodeForegroundColor;
            for (int i = 0; i < flagArray.Length; i++)
            {
                for (int j = 0; j < flagArray.Length; j++)
                {
                    if (flagArray[j][i])
                    {
                        graphics.FillRectangle(brush, j * this.qrCodeScale, i * this.qrCodeScale, this.qrCodeScale, this.qrCodeScale);
                    }
                }
            }
            return image;
        }

        private static sbyte selectMask(sbyte[][] matrixContent, int maxCodewordsBitWithRemain)
        {
            int num6;
            int length = matrixContent.Length;
            int[] numArray9 = new int[8];
            int[] numArray = numArray9;
            numArray9 = new int[8];
            int[] numArray2 = numArray9;
            numArray9 = new int[8];
            int[] numArray3 = numArray9;
            numArray9 = new int[8];
            int[] numArray4 = numArray9;
            int num2 = 0;
            int num3 = 0;
            numArray9 = new int[8];
            int[] numArray5 = numArray9;
            for (int i = 0; i < length; i++)
            {
                numArray9 = new int[8];
                int[] numArray6 = numArray9;
                numArray9 = new int[8];
                int[] numArray7 = numArray9;
                bool[] flagArray3 = new bool[8];
                bool[] flagArray = flagArray3;
                flagArray3 = new bool[8];
                bool[] flagArray2 = flagArray3;
                for (int j = 0; j < length; j++)
                {
                    if ((j > 0) && (i > 0))
                    {
                        num2 = (((matrixContent[j][i] & matrixContent[j - 1][i]) & matrixContent[j][i - 1]) & matrixContent[j - 1][i - 1]) & 0xff;
                        num3 = (((matrixContent[j][i] & 0xff) | (matrixContent[j - 1][i] & 0xff)) | (matrixContent[j][i - 1] & 0xff)) | (matrixContent[j - 1][i - 1] & 0xff);
                    }
                    num6 = 0;
                    while (num6 < 8)
                    {
                        numArray6[num6] = ((numArray6[num6] & 0x3f) << 1) | (SystemUtils.URShift((int)(matrixContent[j][i] & 0xff), num6) & 1);
                        numArray7[num6] = ((numArray7[num6] & 0x3f) << 1) | (SystemUtils.URShift((int)(matrixContent[i][j] & 0xff), num6) & 1);
                        if ((matrixContent[j][i] & (((int)1) << num6)) != 0)
                        {
                            numArray5[num6]++;
                        }
                        if (numArray6[num6] == 0x5d)
                        {
                            numArray3[num6] += 40;
                        }
                        if (numArray7[num6] == 0x5d)
                        {
                            numArray3[num6] += 40;
                        }
                        if ((j > 0) && (i > 0))
                        {
                            if (((num2 & 1) != 0) || ((num3 & 1) == 0))
                            {
                                numArray2[num6] += 3;
                            }
                            num2 = num2 >> 1;
                            num3 = num3 >> 1;
                        }
                        if (((numArray6[num6] & 0x1f) == 0) || ((numArray6[num6] & 0x1f) == 0x1f))
                        {
                            if (j > 3)
                            {
                                if (flagArray[num6])
                                {
                                    numArray[num6]++;
                                }
                                else
                                {
                                    numArray[num6] += 3;
                                    flagArray[num6] = true;
                                }
                            }
                        }
                        else
                        {
                            flagArray[num6] = false;
                        }
                        if (((numArray7[num6] & 0x1f) == 0) || ((numArray7[num6] & 0x1f) == 0x1f))
                        {
                            if (j > 3)
                            {
                                if (flagArray2[num6])
                                {
                                    numArray[num6]++;
                                }
                                else
                                {
                                    numArray[num6] += 3;
                                    flagArray2[num6] = true;
                                }
                            }
                        }
                        else
                        {
                            flagArray2[num6] = false;
                        }
                        num6++;
                    }
                }
            }
            int num7 = 0;
            sbyte num8 = 0;
            int[] numArray8 = new int[] {
                90, 80, 70, 60, 50, 40, 30, 20, 10, 0, 0, 10, 20, 30, 40, 50,
                60, 70, 80, 90, 90
            };
            for (num6 = 0; num6 < 8; num6++)
            {
                numArray4[num6] = numArray8[(20 * numArray5[num6]) / maxCodewordsBitWithRemain];
                int num9 = ((numArray[num6] + numArray2[num6]) + numArray3[num6]) + numArray4[num6];
                if ((num9 < num7) || (num6 == 0))
                {
                    num8 = (sbyte)num6;
                    num7 = num9;
                }
            }
            return num8;
        }

        public virtual void setStructureappend(int m, int n, int p)
        {
            if (((((n > 1) && (n <= 0x10)) && ((m > 0) && (m <= 0x10))) && (p >= 0)) && (p <= 0xff))
            {
                this.qrcodeStructureappendM = m;
                this.qrcodeStructureappendN = n;
                this.qrcodeStructureappendParity = p;
            }
        }

        public virtual ERROR_CORRECTION QRCodeErrorCorrect
        {
            get =>
                this.qrcodeErrorCorrect;
            set =>
                this.qrcodeErrorCorrect = value;
        }

        public virtual int QRCodeVersion
        {
            get =>
                this.qrcodeVersion;
            set
            {
                if ((value >= 0) && (value <= 40))
                {
                    this.qrcodeVersion = value;
                }
            }
        }

        public virtual ENCODE_MODE QRCodeEncodeMode
        {
            get =>
                this.qrcodeEncodeMode;
            set =>
                this.qrcodeEncodeMode = value;
        }

        public virtual int QRCodeScale
        {
            get =>
                this.qrCodeScale;
            set =>
                this.qrCodeScale = value;
        }

        public virtual System.Drawing.Color QRCodeBackgroundColor
        {
            get =>
                this.qrCodeBackgroundColor;
            set =>
                this.qrCodeBackgroundColor = value;
        }

        public virtual System.Drawing.Color QRCodeForegroundColor
        {
            get =>
                this.qrCodeForegroundColor;
            set =>
                this.qrCodeForegroundColor = value;
        }

        public enum ENCODE_MODE
        {
            ALPHA_NUMERIC,
            NUMERIC,
            BYTE
        }

        public enum ERROR_CORRECTION
        {
            L,
            M,
            Q,
            H
        }
    }
}

