namespace ThoughtWorks.QRCode.Codec
{
    using System;
    using System.Collections;
    using System.Text;
    using ThoughtWorks.QRCode.Codec.Data;
    using ThoughtWorks.QRCode.Codec.Ecc;
    using ThoughtWorks.QRCode.Codec.Reader;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.ExceptionHandler;
    using ThoughtWorks.QRCode.Geom;

    public class QRCodeDecoder
    {
        internal QRCodeSymbol qrCodeSymbol;
        internal int numTryDecode = 0;
        internal ArrayList results = ArrayList.Synchronized(new ArrayList(10));
        internal ArrayList lastResults = ArrayList.Synchronized(new ArrayList(10));
        internal static DebugCanvas canvas = new DebugCanvasAdapter();
        internal QRCodeImageReader imageReader;
        internal int numLastCorrections;
        internal bool correctionSucceeded;

        internal virtual int[] correctDataBlocks(int[] blocks)
        {
            ReedSolomon solomon;
            int num8;
            int num9;
            int num10;
            int num = 0;
            int dataCapacity = this.qrCodeSymbol.DataCapacity;
            int[] numArray = new int[dataCapacity];
            int numErrorCollectionCode = this.qrCodeSymbol.NumErrorCollectionCode;
            int numRSBlocks = this.qrCodeSymbol.NumRSBlocks;
            int nPAR = numErrorCollectionCode / numRSBlocks;
            if (numRSBlocks == 1)
            {
                solomon = new ReedSolomon(blocks, nPAR);
                solomon.correct();
                num += solomon.NumCorrectedErrors;
                if (num > 0)
                {
                    canvas.println(Convert.ToString(num) + " data errors corrected.");
                }
                else
                {
                    canvas.println("No errors found.");
                }
                this.numLastCorrections = num;
                this.correctionSucceeded = solomon.CorrectionSucceeded;
                return blocks;
            }
            int num6 = dataCapacity % numRSBlocks;
            if (num6 == 0)
            {
                int num7 = dataCapacity / numRSBlocks;
                int[][] numArray2 = new int[numRSBlocks][];
                for (num8 = 0; num8 < numRSBlocks; num8++)
                {
                    numArray2[num8] = new int[num7];
                }
                int[][] numArray3 = numArray2;
                for (num8 = 0; num8 < numRSBlocks; num8++)
                {
                    num9 = 0;
                    while (num9 < num7)
                    {
                        numArray3[num8][num9] = blocks[(num9 * numRSBlocks) + num8];
                        num9++;
                    }
                    solomon = new ReedSolomon(numArray3[num8], nPAR);
                    solomon.correct();
                    num += solomon.NumCorrectedErrors;
                    this.correctionSucceeded = solomon.CorrectionSucceeded;
                }
                num10 = 0;
                for (num8 = 0; num8 < numRSBlocks; num8++)
                {
                    num9 = 0;
                    while (num9 < (num7 - nPAR))
                    {
                        numArray[num10++] = numArray3[num8][num9];
                        num9++;
                    }
                }
            }
            else
            {
                int num11 = dataCapacity / numRSBlocks;
                int num12 = (dataCapacity / numRSBlocks) + 1;
                int num13 = numRSBlocks - num6;
                int[][] numArray4 = new int[num13][];
                for (int i = 0; i < num13; i++)
                {
                    numArray4[i] = new int[num11];
                }
                int[][] numArray5 = numArray4;
                int[][] numArray6 = new int[num6][];
                for (int j = 0; j < num6; j++)
                {
                    numArray6[j] = new int[num12];
                }
                int[][] numArray7 = numArray6;
                for (num8 = 0; num8 < numRSBlocks; num8++)
                {
                    int num16;
                    if (num8 < num13)
                    {
                        num16 = 0;
                        num9 = 0;
                        while (num9 < num11)
                        {
                            if (num9 == (num11 - nPAR))
                            {
                                num16 = num6;
                            }
                            numArray5[num8][num9] = blocks[((num9 * numRSBlocks) + num8) + num16];
                            num9++;
                        }
                        solomon = new ReedSolomon(numArray5[num8], nPAR);
                        solomon.correct();
                        num += solomon.NumCorrectedErrors;
                        this.correctionSucceeded = solomon.CorrectionSucceeded;
                    }
                    else
                    {
                        num16 = 0;
                        num9 = 0;
                        while (num9 < num12)
                        {
                            if (num9 == (num11 - nPAR))
                            {
                                num16 = num13;
                            }
                            numArray7[num8 - num13][num9] = blocks[((num9 * numRSBlocks) + num8) - num16];
                            num9++;
                        }
                        solomon = new ReedSolomon(numArray7[num8 - num13], nPAR);
                        solomon.correct();
                        num += solomon.NumCorrectedErrors;
                        this.correctionSucceeded = solomon.CorrectionSucceeded;
                    }
                }
                num10 = 0;
                for (num8 = 0; num8 < numRSBlocks; num8++)
                {
                    if (num8 < num13)
                    {
                        num9 = 0;
                        while (num9 < (num11 - nPAR))
                        {
                            numArray[num10++] = numArray5[num8][num9];
                            num9++;
                        }
                    }
                    else
                    {
                        for (num9 = 0; num9 < (num12 - nPAR); num9++)
                        {
                            numArray[num10++] = numArray7[num8 - num13][num9];
                        }
                    }
                }
            }
            if (num > 0)
            {
                canvas.println(Convert.ToString(num) + " data errors corrected.");
            }
            else
            {
                canvas.println("No errors found.");
            }
            this.numLastCorrections = num;
            return numArray;
        }

        public virtual string decode(QRCodeImage qrCodeImage)
        {
            sbyte[] src = this.decodeBytes(qrCodeImage);
            byte[] dst = new byte[src.Length];
            Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
            return Encoding.UTF8.GetString(dst);
        }

        public virtual string decode(QRCodeImage qrCodeImage, Encoding encoding)
        {
            sbyte[] src = this.decodeBytes(qrCodeImage);
            byte[] dst = new byte[src.Length];
            Buffer.BlockCopy(src, 0, dst, 0, dst.Length);
            return encoding.GetString(dst);
        }

        internal virtual DecodeResult decode(QRCodeImage qrCodeImage, Point adjust)
        {
            DecodeResult result;
            try
            {
                if (this.numTryDecode == 0)
                {
                    canvas.println("Decoding started");
                    int[][] image = this.imageToIntArray(qrCodeImage);
                    this.imageReader = new QRCodeImageReader();
                    this.qrCodeSymbol = this.imageReader.getQRCodeSymbol(image);
                }
                else
                {
                    canvas.println("--");
                    canvas.println("Decoding restarted #" + this.numTryDecode);
                    this.qrCodeSymbol = this.imageReader.getQRCodeSymbolWithAdjustedGrid(adjust);
                }
            }
            catch (SymbolNotFoundException exception)
            {
                throw new DecodingFailedException(exception.Message);
            }
            canvas.println("Created QRCode symbol.");
            canvas.println("Reading symbol.");
            canvas.println("Version: " + this.qrCodeSymbol.VersionReference);
            canvas.println("Mask pattern: " + this.qrCodeSymbol.MaskPatternRefererAsString);
            int[] blocks = this.qrCodeSymbol.Blocks;
            canvas.println("Correcting data errors.");
            blocks = this.correctDataBlocks(blocks);
            try
            {
                sbyte[] decodedBytes = this.getDecodedByteArray(blocks, this.qrCodeSymbol.Version, this.qrCodeSymbol.NumErrorCollectionCode);
                result = new DecodeResult(this, decodedBytes, this.numLastCorrections, this.correctionSucceeded);
            }
            catch (InvalidDataBlockException exception2)
            {
                canvas.println(exception2.Message);
                throw new DecodingFailedException(exception2.Message);
            }
            return result;
        }

        public virtual sbyte[] decodeBytes(QRCodeImage qrCodeImage)
        {
            DecodeResult result;
            Point[] adjustPoints = this.AdjustPoints;
            ArrayList list = ArrayList.Synchronized(new ArrayList(10));
            while (this.numTryDecode < adjustPoints.Length)
            {
                try
                {
                    result = this.decode(qrCodeImage, adjustPoints[this.numTryDecode]);
                    if (result.CorrectionSucceeded)
                    {
                        return result.DecodedBytes;
                    }
                    list.Add(result);
                    canvas.println("Decoding succeeded but could not correct");
                    canvas.println("all errors. Retrying..");
                }
                catch (DecodingFailedException exception)
                {
                    if (exception.Message.IndexOf("Finder Pattern") >= 0)
                    {
                        throw exception;
                    }
                }
                finally
                {
                    this.numTryDecode++;
                }
            }
            if (list.Count == 0)
            {
                throw new DecodingFailedException("Give up decoding");
            }
            int num = -1;
            int numErrors = 0x7fffffff;
            for (int i = 0; i < list.Count; i++)
            {
                result = (DecodeResult)list[i];
                if (result.NumErrors < numErrors)
                {
                    numErrors = result.NumErrors;
                    num = i;
                }
            }
            canvas.println("All trials need for correct error");
            canvas.println("Reporting #" + num + " that,");
            canvas.println("corrected minimum errors (" + numErrors + ")");
            canvas.println("Decoding finished.");
            return ((DecodeResult)list[num]).DecodedBytes;
        }

        internal virtual sbyte[] getDecodedByteArray(int[] blocks, int version, int numErrorCorrectionCode)
        {
            sbyte[] dataByte;
            QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
            try
            {
                dataByte = reader.DataByte;
            }
            catch (InvalidDataBlockException exception)
            {
                throw exception;
            }
            return dataByte;
        }

        internal virtual string getDecodedString(int[] blocks, int version, int numErrorCorrectionCode)
        {
            string dataString = null;
            QRCodeDataBlockReader reader = new QRCodeDataBlockReader(blocks, version, numErrorCorrectionCode);
            try
            {
                dataString = reader.DataString;
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new InvalidDataBlockException(exception.Message);
            }
            return dataString;
        }

        internal virtual int[][] imageToIntArray(QRCodeImage image)
        {
            int width = image.Width;
            int height = image.Height;
            int[][] numArray = new int[width][];
            for (int i = 0; i < width; i++)
            {
                numArray[i] = new int[height];
            }
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    numArray[k][j] = image.getPixel(k, j);
                }
            }
            return numArray;
        }

        public static DebugCanvas Canvas
        {
            get =>
                canvas;
            set =>
                canvas = value;
        }

        internal virtual Point[] AdjustPoints
        {
            get
            {
                ArrayList list = ArrayList.Synchronized(new ArrayList(10));
                for (int i = 0; i < 4; i++)
                {
                    list.Add(new Point(1, 1));
                }
                int num2 = 0;
                int num3 = 0;
                for (int j = 0; j > -4; j--)
                {
                    for (int m = 0; m > -4; m--)
                    {
                        if ((m != j) && (((m + j) % 2) == 0))
                        {
                            list.Add(new Point(m - num2, j - num3));
                            num2 = m;
                            num3 = j;
                        }
                    }
                }
                Point[] pointArray = new Point[list.Count];
                for (int k = 0; k < pointArray.Length; k++)
                {
                    pointArray[k] = (Point)list[k];
                }
                return pointArray;
            }
        }

        internal class DecodeResult
        {
            internal int numCorrections;
            internal bool correctionSucceeded;
            internal sbyte[] decodedBytes;
            private QRCodeDecoder enclosingInstance;

            public DecodeResult(QRCodeDecoder enclosingInstance, sbyte[] decodedBytes, int numErrors, bool correctionSucceeded)
            {
                this.InitBlock(enclosingInstance);
                this.decodedBytes = decodedBytes;
                this.numCorrections = numErrors;
                this.correctionSucceeded = correctionSucceeded;
            }

            private void InitBlock(QRCodeDecoder enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public virtual sbyte[] DecodedBytes =>
                this.decodedBytes;

            public virtual int NumErrors =>
                this.numCorrections;

            public virtual bool CorrectionSucceeded =>
                this.correctionSucceeded;

            public QRCodeDecoder Enclosing_Instance =>
                this.enclosingInstance;
        }
    }
}

