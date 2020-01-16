namespace ThoughtWorks.QRCode.Codec.Reader
{
    using System;
    using System.Collections;
    using ThoughtWorks.QRCode.Codec.Data;
    using ThoughtWorks.QRCode.Codec.Reader.Pattern;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.ExceptionHandler;
    using ThoughtWorks.QRCode.Geom;

    public class QRCodeImageReader
    {
        public const bool POINT_DARK = true;
        public const bool POINT_LIGHT = false;
        internal DebugCanvas canvas = QRCodeDecoder.Canvas;
        public static int DECIMAL_POINT = 0x15;
        internal SamplingGrid samplingGrid;
        internal bool[][] bitmap;

        internal virtual bool[][] applyCrossMaskingMedianFilter(bool[][] image, int threshold)
        {
            bool[][] flagArray = new bool[image.Length][];
            for (int i = 0; i < image.Length; i++)
            {
                flagArray[i] = new bool[image[0].Length];
            }
            for (int j = 2; j < (image[0].Length - 2); j++)
            {
                for (int k = 2; k < (image.Length - 2); k++)
                {
                    int num2 = 0;
                    for (int m = -2; m < 3; m++)
                    {
                        if (image[k + m][j])
                        {
                            num2++;
                        }
                        if (image[k][j + m])
                        {
                            num2++;
                        }
                    }
                    if (num2 > threshold)
                    {
                        flagArray[k][j] = true;
                    }
                }
            }
            return flagArray;
        }

        internal virtual bool[][] applyMedianFilter(bool[][] image, int threshold)
        {
            bool[][] flagArray = new bool[image.Length][];
            for (int i = 0; i < image.Length; i++)
            {
                flagArray[i] = new bool[image[0].Length];
            }
            for (int j = 1; j < (image[0].Length - 1); j++)
            {
                for (int k = 1; k < (image.Length - 1); k++)
                {
                    int num2 = 0;
                    for (int m = -1; m < 2; m++)
                    {
                        for (int n = -1; n < 2; n++)
                        {
                            if (image[k + n][j + m])
                            {
                                num2++;
                            }
                        }
                    }
                    if (num2 > threshold)
                    {
                        flagArray[k][j] = true;
                    }
                }
            }
            return flagArray;
        }

        internal virtual bool[][] filterImage(int[][] image)
        {
            this.imageToGrayScale(image);
            return this.grayScaleToBitmap(image);
        }

        internal virtual int getAreaModulePitch(Point start, Point end, int logicalDistance)
        {
            Line line = new Line(start, end);
            return ((line.Length << DECIMAL_POINT) / logicalDistance);
        }

        internal virtual int[][] getMiddleBrightnessPerArea(int[][] image)
        {
            int num6;
            int num7;
            int num = 4;
            int num2 = image.Length / num;
            int num3 = image[0].Length / num;
            int[][][] numArray = new int[num][][];
            for (int i = 0; i < num; i++)
            {
                numArray[i] = new int[num][];
                for (int k = 0; k < num; k++)
                {
                    numArray[i][k] = new int[2];
                }
            }
            for (num6 = 0; num6 < num; num6++)
            {
                num7 = 0;
                while (num7 < num)
                {
                    numArray[num7][num6][0] = 0xff;
                    for (int k = 0; k < num3; k++)
                    {
                        for (int m = 0; m < num2; m++)
                        {
                            int num10 = image[(num2 * num7) + m][(num3 * num6) + k];
                            if (num10 < numArray[num7][num6][0])
                            {
                                numArray[num7][num6][0] = num10;
                            }
                            if (num10 > numArray[num7][num6][1])
                            {
                                numArray[num7][num6][1] = num10;
                            }
                        }
                    }
                    num7++;
                }
            }
            int[][] numArray2 = new int[num][];
            for (int j = 0; j < num; j++)
            {
                numArray2[j] = new int[num];
            }
            for (num6 = 0; num6 < num; num6++)
            {
                for (num7 = 0; num7 < num; num7++)
                {
                    numArray2[num7][num6] = (numArray[num7][num6][0] + numArray[num7][num6][1]) / 2;
                }
            }
            return numArray2;
        }

        internal virtual bool[][] getQRCodeMatrix(bool[][] image, SamplingGrid gridLines)
        {
            int totalWidth = gridLines.TotalWidth;
            this.canvas.println("gridSize=" + totalWidth);
            Point point = null;
            bool[][] flagArray = new bool[totalWidth][];
            for (int i = 0; i < totalWidth; i++)
            {
                flagArray[i] = new bool[totalWidth];
            }
            for (int j = 0; j < gridLines.getHeight(); j++)
            {
                for (int k = 0; k < gridLines.getWidth(); k++)
                {
                    ArrayList list = ArrayList.Synchronized(new ArrayList(10));
                    for (int m = 0; m < gridLines.getHeight(k, j); m++)
                    {
                        for (int n = 0; n < gridLines.getWidth(k, j); n++)
                        {
                            int x = gridLines.getXLine(k, j, n).getP1().X;
                            int y = gridLines.getXLine(k, j, n).getP1().Y;
                            int num9 = gridLines.getXLine(k, j, n).getP2().X;
                            int num10 = gridLines.getXLine(k, j, n).getP2().Y;
                            int num11 = gridLines.getYLine(k, j, m).getP1().X;
                            int num12 = gridLines.getYLine(k, j, m).getP1().Y;
                            int num13 = gridLines.getYLine(k, j, m).getP2().X;
                            int num14 = gridLines.getYLine(k, j, m).getP2().Y;
                            int num15 = ((num10 - y) * (num11 - num13)) - ((num14 - num12) * (x - num9));
                            int num16 = (((x * num10) - (num9 * y)) * (num11 - num13)) - (((num11 * num14) - (num13 * num12)) * (x - num9));
                            int num17 = (((num11 * num14) - (num13 * num12)) * (num10 - y)) - (((x * num10) - (num9 * y)) * (num14 - num12));
                            flagArray[gridLines.getX(k, n)][gridLines.getY(j, m)] = image[num16 / num15][num17 / num15];
                            if ((((j == (gridLines.getHeight() - 1)) && (k == (gridLines.getWidth() - 1))) && (m == (gridLines.getHeight(k, j) - 1))) && (n == (gridLines.getWidth(k, j) - 1)))
                            {
                                point = new Point(num16 / num15, num17 / num15);
                            }
                        }
                    }
                }
            }
            if ((point.X > (image.Length - 1)) || (point.Y > (image[0].Length - 1)))
            {
                throw new IndexOutOfRangeException("Sampling grid pointed out of image");
            }
            this.canvas.drawPoint(point, Color_Fields.BLUE);
            return flagArray;
        }

        public virtual QRCodeSymbol getQRCodeSymbol(int[][] image)
        {
            int num = (image.Length < image[0].Length) ? image[0].Length : image.Length;
            DECIMAL_POINT = 0x17 - QRCodeUtility.sqrt(num / 0x100);
            this.bitmap = this.filterImage(image);
            this.canvas.println("Drawing matrix.");
            this.canvas.drawMatrix(this.bitmap);
            this.canvas.println("Scanning Finder Pattern.");
            FinderPattern finderPattern = null;
            try
            {
                finderPattern = FinderPattern.findFinderPattern(this.bitmap);
            }
            catch (FinderPatternNotFoundException)
            {
                this.canvas.println("Not found, now retrying...");
                this.bitmap = this.applyCrossMaskingMedianFilter(this.bitmap, 5);
                this.canvas.drawMatrix(this.bitmap);
                for (int j = 0; j < 0x3b9aca00; j++)
                {
                }
                try
                {
                    finderPattern = FinderPattern.findFinderPattern(this.bitmap);
                }
                catch (FinderPatternNotFoundException exception2)
                {
                    throw new SymbolNotFoundException(exception2.Message);
                }
                catch (VersionInformationException exception3)
                {
                    throw new SymbolNotFoundException(exception3.Message);
                }
            }
            catch (VersionInformationException exception4)
            {
                throw new SymbolNotFoundException(exception4.Message);
            }
            this.canvas.println("FinderPattern at");
            string str = finderPattern.getCenter(0).ToString() + finderPattern.getCenter(1).ToString() + finderPattern.getCenter(2).ToString();
            this.canvas.println(str);
            int[] numArray = finderPattern.getAngle();
            this.canvas.println("Angle*4098: Sin " + Convert.ToString(numArray[0]) + "  Cos " + Convert.ToString(numArray[1]));
            int version = finderPattern.Version;
            this.canvas.println("Version: " + Convert.ToString(version));
            if ((version < 1) || (version > 40))
            {
                throw new InvalidVersionException("Invalid version: " + version);
            }
            AlignmentPattern alignmentPattern = null;
            try
            {
                alignmentPattern = AlignmentPattern.findAlignmentPattern(this.bitmap, finderPattern);
            }
            catch (AlignmentPatternNotFoundException exception5)
            {
                throw new SymbolNotFoundException(exception5.Message);
            }
            int length = alignmentPattern.getCenter().Length;
            this.canvas.println("AlignmentPatterns at");
            for (int i = 0; i < length; i++)
            {
                string str2 = "";
                for (int j = 0; j < length; j++)
                {
                    str2 = str2 + alignmentPattern.getCenter()[j][i].ToString();
                }
                this.canvas.println(str2);
            }
            this.canvas.println("Creating sampling grid.");
            this.samplingGrid = this.getSamplingGrid(finderPattern, alignmentPattern);
            this.canvas.println("Reading grid.");
            bool[][] moduleMatrix = null;
            try
            {
                moduleMatrix = this.getQRCodeMatrix(this.bitmap, this.samplingGrid);
            }
            catch (IndexOutOfRangeException)
            {
                throw new SymbolNotFoundException("Sampling grid exceeded image boundary");
            }
            return new QRCodeSymbol(moduleMatrix);
        }

        public virtual QRCodeSymbol getQRCodeSymbolWithAdjustedGrid(Point adjust)
        {
            if ((this.bitmap == null) || (this.samplingGrid == null))
            {
                throw new SystemException("This method must be called after QRCodeImageReader.getQRCodeSymbol() called");
            }
            this.samplingGrid.adjust(adjust);
            this.canvas.println(string.Concat(new object[] { "Sampling grid adjusted d(", adjust.X, ",", adjust.Y, ")" }));
            bool[][] moduleMatrix = null;
            try
            {
                moduleMatrix = this.getQRCodeMatrix(this.bitmap, this.samplingGrid);
            }
            catch (IndexOutOfRangeException)
            {
                throw new SymbolNotFoundException("Sampling grid exceeded image boundary");
            }
            return new QRCodeSymbol(moduleMatrix);
        }

        internal virtual SamplingGrid getSamplingGrid(FinderPattern finderPattern, AlignmentPattern alignmentPattern)
        {
            Point[][] pointArray = alignmentPattern.getCenter();
            int version = finderPattern.Version;
            int num2 = (version / 7) + 2;
            pointArray[0][0] = finderPattern.getCenter(0);
            pointArray[num2 - 1][0] = finderPattern.getCenter(1);
            pointArray[0][num2 - 1] = finderPattern.getCenter(2);
            int sqrtNumArea = num2 - 1;
            SamplingGrid grid = new SamplingGrid(sqrtNumArea);
            Axis axis = new Axis(finderPattern.getAngle(), finderPattern.getModuleSize());
            for (int i = 0; i < sqrtNumArea; i++)
            {
                for (int j = 0; j < sqrtNumArea; j++)
                {
                    ModulePitch pitch = new ModulePitch(this);
                    Line line = new Line();
                    Line line2 = new Line();
                    axis.ModulePitch = finderPattern.getModuleSize();
                    Point[][] pointArray2 = AlignmentPattern.getLogicalCenter(finderPattern);
                    Point origin = pointArray[j][i];
                    Point point2 = pointArray[j + 1][i];
                    Point point3 = pointArray[j][i + 1];
                    Point point4 = pointArray[j + 1][i + 1];
                    Point point5 = pointArray2[j][i];
                    Point point6 = pointArray2[j + 1][i];
                    Point point7 = pointArray2[j][i + 1];
                    Point point8 = pointArray2[j + 1][i + 1];
                    if ((j == 0) && (i == 0))
                    {
                        if (sqrtNumArea == 1)
                        {
                            origin = axis.translate(origin, -3, -3);
                            point2 = axis.translate(point2, 3, -3);
                            point3 = axis.translate(point3, -3, 3);
                            point4 = axis.translate(point4, 6, 6);
                            point5.translate(-6, -6);
                            point6.translate(3, -3);
                            point7.translate(-3, 3);
                            point8.translate(6, 6);
                        }
                        else
                        {
                            origin = axis.translate(origin, -3, -3);
                            point2 = axis.translate(point2, 0, -6);
                            point3 = axis.translate(point3, -6, 0);
                            point5.translate(-6, -6);
                            point6.translate(0, -6);
                            point7.translate(-6, 0);
                        }
                    }
                    else if ((j == 0) && (i == (sqrtNumArea - 1)))
                    {
                        origin = axis.translate(origin, -6, 0);
                        point3 = axis.translate(point3, -3, 3);
                        point4 = axis.translate(point4, 0, 6);
                        point5.translate(-6, 0);
                        point7.translate(-6, 6);
                        point8.translate(0, 6);
                    }
                    else if ((j == (sqrtNumArea - 1)) && (i == 0))
                    {
                        origin = axis.translate(origin, 0, -6);
                        point2 = axis.translate(point2, 3, -3);
                        point4 = axis.translate(point4, 6, 0);
                        point5.translate(0, -6);
                        point6.translate(6, -6);
                        point8.translate(6, 0);
                    }
                    else if ((j == (sqrtNumArea - 1)) && (i == (sqrtNumArea - 1)))
                    {
                        point3 = axis.translate(point3, 0, 6);
                        point2 = axis.translate(point2, 6, 0);
                        point4 = axis.translate(point4, 6, 6);
                        point7.translate(0, 6);
                        point6.translate(6, 0);
                        point8.translate(6, 6);
                    }
                    else if (j == 0)
                    {
                        origin = axis.translate(origin, -6, 0);
                        point3 = axis.translate(point3, -6, 0);
                        point5.translate(-6, 0);
                        point7.translate(-6, 0);
                    }
                    else if (j == (sqrtNumArea - 1))
                    {
                        point2 = axis.translate(point2, 6, 0);
                        point4 = axis.translate(point4, 6, 0);
                        point6.translate(6, 0);
                        point8.translate(6, 0);
                    }
                    else if (i == 0)
                    {
                        origin = axis.translate(origin, 0, -6);
                        point2 = axis.translate(point2, 0, -6);
                        point5.translate(0, -6);
                        point6.translate(0, -6);
                    }
                    else if (i == (sqrtNumArea - 1))
                    {
                        point3 = axis.translate(point3, 0, 6);
                        point4 = axis.translate(point4, 0, 6);
                        point7.translate(0, 6);
                        point8.translate(0, 6);
                    }
                    if (j == 0)
                    {
                        point6.translate(1, 0);
                        point8.translate(1, 0);
                    }
                    else
                    {
                        point5.translate(-1, 0);
                        point7.translate(-1, 0);
                    }
                    if (i == 0)
                    {
                        point7.translate(0, 1);
                        point8.translate(0, 1);
                    }
                    else
                    {
                        point5.translate(0, -1);
                        point6.translate(0, -1);
                    }
                    int width = point6.X - point5.X;
                    int height = point7.Y - point5.Y;
                    if (version < 7)
                    {
                        width += 3;
                        height += 3;
                    }
                    pitch.top = this.getAreaModulePitch(origin, point2, width - 1);
                    pitch.left = this.getAreaModulePitch(origin, point3, height - 1);
                    pitch.bottom = this.getAreaModulePitch(point3, point4, width - 1);
                    pitch.right = this.getAreaModulePitch(point2, point4, height - 1);
                    line.setP1(origin);
                    line2.setP1(origin);
                    line.setP2(point3);
                    line2.setP2(point2);
                    grid.initGrid(j, i, width, height);
                    int moveX = 0;
                    while (moveX < width)
                    {
                        Line line3 = new Line(line.getP1(), line.getP2());
                        axis.Origin = line3.getP1();
                        axis.ModulePitch = pitch.top;
                        line3.setP1(axis.translate(moveX, 0));
                        axis.Origin = line3.getP2();
                        axis.ModulePitch = pitch.bottom;
                        line3.setP2(axis.translate(moveX, 0));
                        grid.setXLine(j, i, moveX, line3);
                        moveX++;
                    }
                    for (moveX = 0; moveX < height; moveX++)
                    {
                        Line line4 = new Line(line2.getP1(), line2.getP2());
                        axis.Origin = line4.getP1();
                        axis.ModulePitch = pitch.left;
                        line4.setP1(axis.translate(0, moveX));
                        axis.Origin = line4.getP2();
                        axis.ModulePitch = pitch.right;
                        line4.setP2(axis.translate(0, moveX));
                        grid.setYLine(j, i, moveX, line4);
                    }
                }
            }
            return grid;
        }

        internal virtual bool[][] grayScaleToBitmap(int[][] grayScale)
        {
            int[][] numArray = this.getMiddleBrightnessPerArea(grayScale);
            int length = numArray.Length;
            int num2 = grayScale.Length / length;
            int num3 = grayScale[0].Length / length;
            bool[][] flagArray = new bool[grayScale.Length][];
            for (int i = 0; i < grayScale.Length; i++)
            {
                flagArray[i] = new bool[grayScale[0].Length];
            }
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < length; k++)
                {
                    for (int m = 0; m < num3; m++)
                    {
                        for (int n = 0; n < num2; n++)
                        {
                            flagArray[(num2 * k) + n][(num3 * j) + m] = grayScale[(num2 * k) + n][(num3 * j) + m] < numArray[k][j];
                        }
                    }
                }
            }
            return flagArray;
        }

        internal virtual void imageToGrayScale(int[][] image)
        {
            for (int i = 0; i < image[0].Length; i++)
            {
                for (int j = 0; j < image.Length; j++)
                {
                    int num3 = (image[j][i] >> 0x10) & 0xff;
                    int num4 = (image[j][i] >> 8) & 0xff;
                    int num5 = image[j][i] & 0xff;
                    int num6 = (((num3 * 30) + (num4 * 0x3b)) + (num5 * 11)) / 100;
                    image[j][i] = num6;
                }
            }
        }

        private class ModulePitch
        {
            public int top;
            public int left;
            public int bottom;
            public int right;
            private QRCodeImageReader enclosingInstance;

            public ModulePitch(QRCodeImageReader enclosingInstance)
            {
                this.InitBlock(enclosingInstance);
            }

            private void InitBlock(QRCodeImageReader enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public QRCodeImageReader Enclosing_Instance =>
                this.enclosingInstance;
        }
    }
}

