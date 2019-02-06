namespace ThoughtWorks.QRCode.Codec.Ecc
{
    using System;

    public class ReedSolomon
    {
        internal int[] y;
        internal int[] gexp = new int[0x200];
        internal int[] glog = new int[0x100];
        internal int NPAR;
        internal int MAXDEG;
        internal int[] synBytes;
        internal int[] Lambda;
        internal int[] Omega;
        internal int[] ErrorLocs = new int[0x100];
        internal int NErrors;
        internal int[] ErasureLocs = new int[0x100];
        internal int NErasures = 0;
        internal bool correctionSucceeded = true;

        public ReedSolomon(int[] source, int NPAR)
        {
            this.initializeGaloisTables();
            this.y = source;
            this.NPAR = NPAR;
            this.MAXDEG = NPAR * 2;
            this.synBytes = new int[this.MAXDEG];
            this.Lambda = new int[this.MAXDEG];
            this.Omega = new int[this.MAXDEG];
        }

        internal virtual void add_polys(int[] dst, int[] src)
        {
            for (int i = 0; i < this.MAXDEG; i++)
            {
                dst[i] ^= src[i];
            }
        }

        internal virtual int compute_discrepancy(int[] lambda, int[] S, int L, int n)
        {
            int num2 = 0;
            for (int i = 0; i <= L; i++)
            {
                num2 ^= this.gmult(lambda[i], S[n - i]);
            }
            return num2;
        }

        internal virtual void compute_modified_omega()
        {
            int[] dst = new int[this.MAXDEG * 2];
            this.mult_polys(dst, this.Lambda, this.synBytes);
            this.zero_poly(this.Omega);
            for (int i = 0; i < this.NPAR; i++)
            {
                this.Omega[i] = dst[i];
            }
        }

        internal virtual void compute_next_omega(int d, int[] A, int[] dst, int[] src)
        {
            for (int i = 0; i < this.MAXDEG; i++)
            {
                dst[i] = src[i] ^ this.gmult(d, A[i]);
            }
        }

        internal virtual void copy_poly(int[] dst, int[] src)
        {
            for (int i = 0; i < this.MAXDEG; i++)
            {
                dst[i] = src[i];
            }
        }

        public virtual void correct()
        {
            this.decode_data(this.y);
            this.correctionSucceeded = true;
            bool flag = false;
            for (int i = 0; i < this.synBytes.Length; i++)
            {
                if (this.synBytes[i] != 0)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                this.correctionSucceeded = this.correct_errors_erasures(this.y, this.y.Length, 0, new int[1]);
            }
        }

        internal virtual bool correct_errors_erasures(int[] codeword, int csize, int nerasures, int[] erasures)
        {
            this.NErasures = nerasures;
            int index = 0;
            while (index < this.NErasures)
            {
                this.ErasureLocs[index] = erasures[index];
                index++;
            }
            this.Modified_Berlekamp_Massey();
            this.Find_Roots();
            if ((this.NErrors <= this.NPAR) || (this.NErrors > 0))
            {
                int num;
                for (num = 0; num < this.NErrors; num++)
                {
                    if (this.ErrorLocs[num] >= csize)
                    {
                        return false;
                    }
                }
                for (num = 0; num < this.NErrors; num++)
                {
                    index = this.ErrorLocs[num];
                    int a = 0;
                    int num3 = 0;
                    while (num3 < this.MAXDEG)
                    {
                        a ^= this.gmult(this.Omega[num3], this.gexp[((0xff - index) * num3) % 0xff]);
                        num3++;
                    }
                    int elt = 0;
                    for (num3 = 1; num3 < this.MAXDEG; num3 += 2)
                    {
                        elt ^= this.gmult(this.Lambda[num3], this.gexp[((0xff - index) * (num3 - 1)) % 0xff]);
                    }
                    int num4 = this.gmult(a, this.ginv(elt));
                    codeword[(csize - index) - 1] ^= num4;
                }
                return true;
            }
            return false;
        }

        internal virtual void decode_data(int[] data)
        {
            for (int i = 0; i < this.MAXDEG; i++)
            {
                int b = 0;
                for (int j = 0; j < data.Length; j++)
                {
                    b = data[j] ^ this.gmult(this.gexp[i + 1], b);
                }
                this.synBytes[i] = b;
            }
        }

        internal virtual void Find_Roots()
        {
            this.NErrors = 0;
            for (int i = 1; i < 0x100; i++)
            {
                int num = 0;
                for (int j = 0; j < (this.NPAR + 1); j++)
                {
                    num ^= this.gmult(this.gexp[(j * i) % 0xff], this.Lambda[j]);
                }
                if (num == 0)
                {
                    this.ErrorLocs[this.NErrors] = 0xff - i;
                    this.NErrors++;
                }
            }
        }

        internal virtual int ginv(int elt) => 
            this.gexp[0xff - this.glog[elt]];

        internal virtual int gmult(int a, int b)
        {
            if ((a == 0) || (b == 0))
            {
                return 0;
            }
            int num = this.glog[a];
            int num2 = this.glog[b];
            return this.gexp[num + num2];
        }

        internal virtual void init_gamma(int[] gamma)
        {
            int[] poly = new int[this.MAXDEG];
            this.zero_poly(gamma);
            this.zero_poly(poly);
            gamma[0] = 1;
            for (int i = 0; i < this.NErasures; i++)
            {
                this.copy_poly(poly, gamma);
                this.scale_poly(this.gexp[this.ErasureLocs[i]], poly);
                this.mul_z_poly(poly);
                this.add_polys(gamma, poly);
            }
        }

        internal virtual void initializeGaloisTables()
        {
            int num;
            int num5;
            int num6;
            int num7;
            int num8;
            int num9;
            int num10;
            int num11;
            int num3 = num5 = num6 = num7 = num8 = num9 = num10 = num11 = 0;
            int num4 = 1;
            this.gexp[0] = 1;
            this.gexp[0xff] = this.gexp[0];
            this.glog[0] = 0;
            for (num = 1; num < 0x100; num++)
            {
                num3 = num11;
                num11 = num10;
                num10 = num9;
                num9 = num8;
                num8 = num7 ^ num3;
                num7 = num6 ^ num3;
                num6 = num5 ^ num3;
                num5 = num4;
                num4 = num3;
                this.gexp[num] = ((((((num4 + (num5 * 2)) + (num6 * 4)) + (num7 * 8)) + (num8 * 0x10)) + (num9 * 0x20)) + (num10 * 0x40)) + (num11 * 0x80);
                this.gexp[num + 0xff] = this.gexp[num];
            }
            for (num = 1; num < 0x100; num++)
            {
                for (int i = 0; i < 0x100; i++)
                {
                    if (this.gexp[i] == num)
                    {
                        this.glog[num] = i;
                        break;
                    }
                }
            }
        }

        internal virtual void Modified_Berlekamp_Massey()
        {
            int num6;
            int[] dst = new int[this.MAXDEG];
            int[] numArray2 = new int[this.MAXDEG];
            int[] numArray3 = new int[this.MAXDEG];
            int[] gamma = new int[this.MAXDEG];
            this.init_gamma(gamma);
            this.copy_poly(numArray3, gamma);
            this.mul_z_poly(numArray3);
            this.copy_poly(dst, gamma);
            int num4 = -1;
            int nErasures = this.NErasures;
            for (int i = this.NErasures; i < 8; i++)
            {
                int a = this.compute_discrepancy(dst, this.synBytes, nErasures, i);
                if (a != 0)
                {
                    num6 = 0;
                    while (num6 < this.MAXDEG)
                    {
                        numArray2[num6] = dst[num6] ^ this.gmult(a, numArray3[num6]);
                        num6++;
                    }
                    if (nErasures < (i - num4))
                    {
                        int num3 = i - num4;
                        num4 = i - nErasures;
                        num6 = 0;
                        while (num6 < this.MAXDEG)
                        {
                            numArray3[num6] = this.gmult(dst[num6], this.ginv(a));
                            num6++;
                        }
                        nErasures = num3;
                    }
                    num6 = 0;
                    while (num6 < this.MAXDEG)
                    {
                        dst[num6] = numArray2[num6];
                        num6++;
                    }
                }
                this.mul_z_poly(numArray3);
            }
            for (num6 = 0; num6 < this.MAXDEG; num6++)
            {
                this.Lambda[num6] = dst[num6];
            }
            this.compute_modified_omega();
        }

        internal virtual void mul_z_poly(int[] src)
        {
            for (int i = this.MAXDEG - 1; i > 0; i--)
            {
                src[i] = src[i - 1];
            }
            src[0] = 0;
        }

        internal virtual void mult_polys(int[] dst, int[] p1, int[] p2)
        {
            int num;
            int[] numArray = new int[this.MAXDEG * 2];
            for (num = 0; num < (this.MAXDEG * 2); num++)
            {
                dst[num] = 0;
            }
            for (num = 0; num < this.MAXDEG; num++)
            {
                int mAXDEG = this.MAXDEG;
                while (mAXDEG < (this.MAXDEG * 2))
                {
                    numArray[mAXDEG] = 0;
                    mAXDEG++;
                }
                mAXDEG = 0;
                while (mAXDEG < this.MAXDEG)
                {
                    numArray[mAXDEG] = this.gmult(p2[mAXDEG], p1[num]);
                    mAXDEG++;
                }
                mAXDEG = (this.MAXDEG * 2) - 1;
                while (mAXDEG >= num)
                {
                    numArray[mAXDEG] = numArray[mAXDEG - num];
                    mAXDEG--;
                }
                mAXDEG = 0;
                while (mAXDEG < num)
                {
                    numArray[mAXDEG] = 0;
                    mAXDEG++;
                }
                for (mAXDEG = 0; mAXDEG < (this.MAXDEG * 2); mAXDEG++)
                {
                    dst[mAXDEG] ^= numArray[mAXDEG];
                }
            }
        }

        internal virtual void scale_poly(int k, int[] poly)
        {
            for (int i = 0; i < this.MAXDEG; i++)
            {
                poly[i] = this.gmult(k, poly[i]);
            }
        }

        internal virtual void zero_poly(int[] poly)
        {
            for (int i = 0; i < this.MAXDEG; i++)
            {
                poly[i] = 0;
            }
        }

        public virtual bool CorrectionSucceeded =>
            this.correctionSucceeded;

        public virtual int NumCorrectedErrors =>
            this.NErrors;
    }
}

