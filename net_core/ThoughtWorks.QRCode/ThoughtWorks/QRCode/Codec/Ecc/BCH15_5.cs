namespace ThoughtWorks.QRCode.Codec.Ecc
{
    using System;

    public class BCH15_5
    {
        internal static string[] bitName = new string[] { "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "d0", "d1", "d2", "d3", "d4" };
        internal int[][] gf16;
        internal int numCorrectedError;
        internal bool[] recieveData;

        public BCH15_5(bool[] source)
        {
            this.gf16 = this.createGF16();
            this.recieveData = source;
        }

        internal virtual int addGF(int arg1, int arg2)
        {
            int[] x = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int num2 = ((arg1 < 0) || (arg1 >= 15)) ? 0 : this.gf16[arg1][i];
                int num3 = ((arg2 < 0) || (arg2 >= 15)) ? 0 : this.gf16[arg2][i];
                x[i] = (num2 + num3) % 2;
            }
            return this.searchElement(x);
        }

        internal virtual int[] calcErrorPositionVariable(int[] s)
        {
            int[] numArray = new int[4];
            numArray[0] = s[0];
            int num = (s[0] + s[1]) % 15;
            int num2 = this.addGF(s[2], num);
            num2 = (num2 >= 15) ? -1 : num2;
            num = (s[2] + s[1]) % 15;
            int num3 = this.addGF(s[4], num);
            num3 = (num3 >= 15) ? -1 : num3;
            numArray[1] = ((num3 < 0) && (num2 < 0)) ? -1 : (((num3 - num2) + 15) % 15);
            num = (s[1] + numArray[0]) % 15;
            int num4 = this.addGF(s[2], num);
            num = (s[0] + numArray[1]) % 15;
            numArray[2] = this.addGF(num4, num);
            return numArray;
        }

        internal virtual int[] calcSyndrome(bool[] y)
        {
            int num;
            int num2;
            int[] numArray = new int[5];
            int[] x = new int[4];
            for (num = 0; num < 15; num++)
            {
                if (y[num])
                {
                    num2 = 0;
                    while (num2 < 4)
                    {
                        x[num2] = (x[num2] + this.gf16[num][num2]) % 2;
                        num2++;
                    }
                }
            }
            num = this.searchElement(x);
            numArray[0] = (num >= 15) ? -1 : num;
            x = new int[4];
            for (num = 0; num < 15; num++)
            {
                if (y[num])
                {
                    num2 = 0;
                    while (num2 < 4)
                    {
                        x[num2] = (x[num2] + this.gf16[(num * 3) % 15][num2]) % 2;
                        num2++;
                    }
                }
            }
            num = this.searchElement(x);
            numArray[2] = (num >= 15) ? -1 : num;
            x = new int[4];
            for (num = 0; num < 15; num++)
            {
                if (y[num])
                {
                    for (num2 = 0; num2 < 4; num2++)
                    {
                        x[num2] = (x[num2] + this.gf16[(num * 5) % 15][num2]) % 2;
                    }
                }
            }
            num = this.searchElement(x);
            numArray[4] = (num >= 15) ? -1 : num;
            return numArray;
        }

        public virtual bool[] correct()
        {
            int[] s = this.calcSyndrome(this.recieveData);
            int[] errorPos = this.detectErrorBitPosition(s);
            return this.correctErrorBit(this.recieveData, errorPos);
        }

        internal virtual bool[] correctErrorBit(bool[] y, int[] errorPos)
        {
            for (int i = 1; i <= errorPos[0]; i++)
            {
                y[errorPos[i]] = !y[errorPos[i]];
            }
            this.numCorrectedError = errorPos[0];
            return y;
        }

        internal virtual int[][] createGF16()
        {
            int num;
            this.gf16 = new int[0x10][];
            for (num = 0; num < 0x10; num++)
            {
                this.gf16[num] = new int[4];
            }
            int[] numArray3 = new int[4];
            numArray3[0] = 1;
            numArray3[1] = 1;
            int[] numArray = numArray3;
            for (num = 0; num < 4; num++)
            {
                this.gf16[num][num] = 1;
            }
            for (num = 0; num < 4; num++)
            {
                this.gf16[4][num] = numArray[num];
            }
            for (num = 5; num < 0x10; num++)
            {
                int index = 1;
                while (index < 4)
                {
                    this.gf16[num][index] = this.gf16[num - 1][index - 1];
                    index++;
                }
                if (this.gf16[num - 1][3] == 1)
                {
                    for (index = 0; index < 4; index++)
                    {
                        this.gf16[num][index] = (this.gf16[num][index] + numArray[index]) % 2;
                    }
                }
            }
            return this.gf16;
        }

        internal virtual int[] detectErrorBitPosition(int[] s)
        {
            int[] numArray = this.calcErrorPositionVariable(s);
            int[] numArray2 = new int[4];
            if (numArray[0] != -1)
            {
                if (numArray[1] == -1)
                {
                    numArray2[0] = 1;
                    numArray2[1] = numArray[0];
                    return numArray2;
                }
                for (int i = 0; i < 15; i++)
                {
                    int num = (i * 3) % 15;
                    int num2 = (i * 2) % 15;
                    int num3 = i;
                    int num4 = (numArray[0] + num2) % 15;
                    int num5 = this.addGF(num, num4);
                    num4 = (numArray[1] + num3) % 15;
                    int num6 = this.addGF(num4, numArray[2]);
                    if (this.addGF(num5, num6) >= 15)
                    {
                        numArray2[0]++;
                        numArray2[numArray2[0]] = i;
                    }
                }
            }
            return numArray2;
        }

        internal virtual int[] getCode(int input)
        {
            int[] numArray = new int[15];
            int[] numArray2 = new int[8];
            for (int i = 0; i < 15; i++)
            {
                int num3;
                int num4;
                int num2 = numArray2[7];
                if (i < 7)
                {
                    num4 = (input >> (6 - i)) % 2;
                    num3 = (num4 + num2) % 2;
                }
                else
                {
                    num4 = num2;
                    num3 = 0;
                }
                numArray2[7] = (numArray2[6] + num3) % 2;
                numArray2[6] = (numArray2[5] + num3) % 2;
                numArray2[5] = numArray2[4];
                numArray2[4] = (numArray2[3] + num3) % 2;
                numArray2[3] = numArray2[2];
                numArray2[2] = numArray2[1];
                numArray2[1] = numArray2[0];
                numArray2[0] = num3;
                numArray[14 - i] = num4;
            }
            return numArray;
        }

        internal virtual int searchElement(int[] x)
        {
            int index = 0;
            while (index < 15)
            {
                if ((((x[0] == this.gf16[index][0]) && (x[1] == this.gf16[index][1])) && (x[2] == this.gf16[index][2])) && (x[3] == this.gf16[index][3]))
                {
                    return index;
                }
                index++;
            }
            return index;
        }

        public virtual int NumCorrectedError
        {
            get
            {
                return this.numCorrectedError;
            }
        }
    }
}

