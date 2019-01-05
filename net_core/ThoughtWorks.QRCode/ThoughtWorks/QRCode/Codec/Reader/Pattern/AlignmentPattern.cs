namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
    using System;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.ExceptionHandler;
    using ThoughtWorks.QRCode.Geom;

    public class AlignmentPattern
    {
        internal const int BOTTOM = 2;
        internal static DebugCanvas canvas = QRCodeDecoder.Canvas;
        internal Point[][] center;
        internal const int LEFT = 3;
        internal int patternDistance;
        internal const int RIGHT = 1;
        internal const int TOP = 4;

        internal AlignmentPattern(Point[][] center, int patternDistance)
        {
            this.center = center;
            this.patternDistance = patternDistance;
        }

        public static AlignmentPattern findAlignmentPattern(bool[][] image, FinderPattern finderPattern)
        {
            Point[][] logicalCenters = getLogicalCenter(finderPattern);
            int patternDistance = logicalCenters[1][0].X - logicalCenters[0][0].X;
            return new AlignmentPattern(getCenter(image, finderPattern, logicalCenters), patternDistance);
        }

        public virtual Point[][] getCenter()
        {
            return this.center;
        }

        internal static Point[][] getCenter(bool[][] image, FinderPattern finderPattern, Point[][] logicalCenters)
        {
            int modulePitch = finderPattern.getModuleSize();
            Axis axis = new Axis(finderPattern.getAngle(), modulePitch);
            int length = logicalCenters.Length;
            Point[][] pointArray = new Point[length][];
            for (int i = 0; i < length; i++)
            {
                pointArray[i] = new Point[length];
            }
            axis.Origin = finderPattern.getCenter(0);
            pointArray[0][0] = axis.translate(3, 3);
            canvas.drawCross(pointArray[0][0], Color_Fields.BLUE);
            axis.Origin = finderPattern.getCenter(1);
            pointArray[length - 1][0] = axis.translate(-3, 3);
            canvas.drawCross(pointArray[length - 1][0], Color_Fields.BLUE);
            axis.Origin = finderPattern.getCenter(2);
            pointArray[0][length - 1] = axis.translate(3, -3);
            canvas.drawCross(pointArray[0][length - 1], Color_Fields.BLUE);
            Point point = pointArray[0][0];
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < length; k++)
                {
                    if ((((k != 0) || (j != 0)) && ((k != 0) || (j != (length - 1)))) && ((k != (length - 1)) || (j != 0)))
                    {
                        Point point2 = null;
                        if (j == 0)
                        {
                            if ((k > 0) && (k < (length - 1)))
                            {
                                point2 = axis.translate(pointArray[k - 1][j], logicalCenters[k][j].X - logicalCenters[k - 1][j].X, 0);
                            }
                            pointArray[k][j] = new Point(point2.X, point2.Y);
                            canvas.drawCross(pointArray[k][j], Color_Fields.RED);
                        }
                        else if (k == 0)
                        {
                            if ((j > 0) && (j < (length - 1)))
                            {
                                point2 = axis.translate(pointArray[k][j - 1], 0, logicalCenters[k][j].Y - logicalCenters[k][j - 1].Y);
                            }
                            pointArray[k][j] = new Point(point2.X, point2.Y);
                            canvas.drawCross(pointArray[k][j], Color_Fields.RED);
                        }
                        else
                        {
                            Point point3 = axis.translate(pointArray[k - 1][j], logicalCenters[k][j].X - logicalCenters[k - 1][j].X, 0);
                            Point point4 = axis.translate(pointArray[k][j - 1], 0, logicalCenters[k][j].Y - logicalCenters[k][j - 1].Y);
                            pointArray[k][j] = new Point((point3.X + point4.X) / 2, ((point3.Y + point4.Y) / 2) + 1);
                        }
                        if (finderPattern.Version > 1)
                        {
                            Point other = getPrecisionCenter(image, pointArray[k][j]);
                            if (pointArray[k][j].distanceOf(other) < 6)
                            {
                                canvas.drawCross(pointArray[k][j], Color_Fields.RED);
                                int num6 = other.X - pointArray[k][j].X;
                                int num7 = other.Y - pointArray[k][j].Y;
                                canvas.println(string.Concat(new object[] { "Adjust AP(", k, ",", j, ") to d(", num6, ",", num7, ")" }));
                                pointArray[k][j] = other;
                            }
                        }
                        canvas.drawCross(pointArray[k][j], Color_Fields.BLUE);
                        canvas.drawLine(new Line(point, pointArray[k][j]), Color_Fields.LIGHTBLUE);
                        point = pointArray[k][j];
                    }
                }
            }
            return pointArray;
        }

        public static Point[][] getLogicalCenter(FinderPattern finderPattern)
        {
            int version = finderPattern.Version;
            Point[][] pointArray = new Point[1][];
            for (int i = 0; i < 1; i++)
            {
                pointArray[i] = new Point[1];
            }
            int[] numArray = new int[1];
            numArray = LogicalSeed.getSeed(version);
            pointArray = new Point[numArray.Length][];
            for (int j = 0; j < numArray.Length; j++)
            {
                pointArray[j] = new Point[numArray.Length];
            }
            for (int k = 0; k < pointArray.Length; k++)
            {
                for (int m = 0; m < pointArray.Length; m++)
                {
                    pointArray[m][k] = new Point(numArray[m], numArray[k]);
                }
            }
            return pointArray;
        }

        internal static Point getPrecisionCenter(bool[][] image, Point targetPoint)
        {
            int num9;
            int num10;
            int num12;
            int num13;
            int x = targetPoint.X;
            int y = targetPoint.Y;
            if (((x < 0) || (y < 0)) || ((x > (image.Length - 1)) || (y > (image[0].Length - 1))))
            {
                throw new AlignmentPatternNotFoundException("Alignment Pattern finder exceeded out of image");
            }
            if (!image[targetPoint.X][targetPoint.Y])
            {
                int num3 = 0;
                bool flag = false;
                while (!flag)
                {
                    num3++;
                    for (int i = num3; i > -num3; i--)
                    {
                        for (int j = num3; j > -num3; j--)
                        {
                            int index = targetPoint.X + j;
                            int num7 = targetPoint.Y + i;
                            if (((index < 0) || (num7 < 0)) || ((index > (image.Length - 1)) || (num7 > (image[0].Length - 1))))
                            {
                                throw new AlignmentPatternNotFoundException("Alignment Pattern finder exceeded out of image");
                            }
                            if (image[index][num7])
                            {
                                targetPoint = new Point(targetPoint.X + j, targetPoint.Y + i);
                                flag = true;
                            }
                        }
                    }
                }
            }
            int num8 = num9 = num10 = targetPoint.X;
            int num11 = num12 = num13 = targetPoint.Y;
            while ((num9 >= 1) && !targetPointOnTheCorner(image, num9, num11, num9 - 1, num11))
            {
                num9--;
            }
            while ((num10 < (image.Length - 1)) && !targetPointOnTheCorner(image, num10, num11, num10 + 1, num11))
            {
                num10++;
            }
            while ((num12 >= 1) && !targetPointOnTheCorner(image, num8, num12, num8, num12 - 1))
            {
                num12--;
            }
            while ((num13 < (image[0].Length - 1)) && !targetPointOnTheCorner(image, num8, num13, num8, num13 + 1))
            {
                num13++;
            }
            return new Point(((num9 + num10) + 1) / 2, ((num12 + num13) + 1) / 2);
        }

        public virtual void setCenter(Point[][] center)
        {
            this.center = center;
        }

        internal static bool targetPointOnTheCorner(bool[][] image, int x, int y, int nx, int ny)
        {
            if (((((x < 0) || (y < 0)) || ((nx < 0) || (ny < 0))) || (((x > image.Length) || (y > image[0].Length)) || (nx > image.Length))) || (ny > image[0].Length))
            {
                throw new AlignmentPatternNotFoundException("Alignment Pattern Finder exceeded image edge");
            }
            return (!image[x][y] && image[nx][ny]);
        }

        public virtual int LogicalDistance
        {
            get
            {
                return this.patternDistance;
            }
        }
    }
}

