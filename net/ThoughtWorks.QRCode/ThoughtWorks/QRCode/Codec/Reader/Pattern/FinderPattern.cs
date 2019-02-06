namespace ThoughtWorks.QRCode.Codec.Reader.Pattern
{
    using System;
    using System.Collections;
    using ThoughtWorks.QRCode.Codec.Reader;
    using ThoughtWorks.QRCode.Codec.Util;
    using ThoughtWorks.QRCode.ExceptionHandler;
    using ThoughtWorks.QRCode.Geom;

    public class FinderPattern
    {
        public const int UL = 0;
        public const int UR = 1;
        public const int DL = 2;
        internal static readonly int[] VersionInfoBit = new int[] { 
            0x7c94, 0x85bc, 0x9a99, 0xa4d3, 0xbbf6, 0xc762, 0xd847, 0xe60d, 0xf928, 0x10b78, 0x1145d, 0x12a17, 0x13532, 0x149a6, 0x15683, 0x168c9,
            0x177ec, 0x18ec4, 0x191e1, 0x1afab, 0x1b08e, 0x1cc1a, 0x1d33f, 0x1ed75, 0x1f250, 0x209d5, 0x216f0, 0x228ba, 0x2379f, 0x24b0b, 0x2542e, 0x26a64,
            0x27541, 0x28c69
        };
        internal static DebugCanvas canvas = QRCodeDecoder.Canvas;
        internal Point[] center;
        internal int version;
        internal int[] sincos;
        internal int[] width;
        internal int[] moduleSize;

        internal FinderPattern(Point[] center, int version, int[] sincos, int[] width, int[] moduleSize)
        {
            this.center = center;
            this.version = version;
            this.sincos = sincos;
            this.width = width;
            this.moduleSize = moduleSize;
        }

        internal static int calcExactVersion(Point[] centers, int[] angle, int[] moduleSize, bool[][] image)
        {
            Point point;
            int num2;
            bool[] target = new bool[0x12];
            Point[] points = new Point[0x12];
            Axis axis = new Axis(angle, moduleSize[1]) {
                Origin = centers[1]
            };
            int num = 0;
            while (num < 6)
            {
                for (num2 = 0; num2 < 3; num2++)
                {
                    point = axis.translate((int) (num2 - 7), (int) (num - 3));
                    target[num2 + (num * 3)] = image[point.X][point.Y];
                    points[num2 + (num * 3)] = point;
                }
                num++;
            }
            canvas.drawPoints(points, Color_Fields.RED);
            int num3 = 0;
            try
            {
                num3 = checkVersionInfo(target);
            }
            catch (InvalidVersionInfoException)
            {
                canvas.println("Version info error. now retry with other place one.");
                axis.Origin = centers[2];
                axis.ModulePitch = moduleSize[2];
                for (num2 = 0; num2 < 6; num2++)
                {
                    for (num = 0; num < 3; num++)
                    {
                        point = axis.translate((int) (num2 - 3), (int) (num - 7));
                        target[num + (num2 * 3)] = image[point.X][point.Y];
                        points[num2 + (num * 3)] = point;
                    }
                }
                canvas.drawPoints(points, Color_Fields.RED);
                try
                {
                    num3 = checkVersionInfo(target);
                }
                catch (VersionInformationException exception2)
                {
                    throw exception2;
                }
            }
            return num3;
        }

        internal static int calcRoughVersion(Point[] center, int[] width)
        {
            int num = QRCodeImageReader.DECIMAL_POINT;
            int num2 = new Line(center[0], center[1]).Length << num;
            int num3 = ((width[0] + width[1]) << num) / 14;
            int num4 = ((num2 / num3) - 10) / 4;
            if ((((num2 / num3) - 10) % 4) >= 2)
            {
                num4++;
            }
            return num4;
        }

        internal static bool cantNeighbor(Line line1, Line line2)
        {
            if (Line.isCross(line1, line2))
            {
                return true;
            }
            if (line1.Horizontal)
            {
                return (Math.Abs((int) (line1.getP1().Y - line2.getP1().Y)) > 1);
            }
            return (Math.Abs((int) (line1.getP1().X - line2.getP1().X)) > 1);
        }

        internal static bool checkPattern(int[] buffer, int pointer)
        {
            int[] numArray = new int[] { 1, 1, 3, 1, 1 };
            int num = 0;
            for (int i = 0; i < 5; i++)
            {
                num += buffer[i];
            }
            num = num << QRCodeImageReader.DECIMAL_POINT;
            num /= 7;
            for (int j = 0; j < 5; j++)
            {
                int num4 = (num * numArray[j]) - (num / 2);
                int num5 = (num * numArray[j]) + (num / 2);
                int num6 = buffer[((pointer + j) + 1) % 5] << QRCodeImageReader.DECIMAL_POINT;
                if ((num6 < num4) || (num6 > num5))
                {
                    return false;
                }
            }
            return true;
        }

        internal static int checkVersionInfo(bool[] target)
        {
            int num = 0;
            int index = 0;
            while (index < VersionInfoBit.Length)
            {
                num = 0;
                for (int i = 0; i < 0x12; i++)
                {
                    if (target[i] ^ (((VersionInfoBit[index] >> i) % 2) == 1))
                    {
                        num++;
                    }
                }
                if (num <= 3)
                {
                    break;
                }
                index++;
            }
            if (num > 3)
            {
                throw new InvalidVersionInfoException("Too many errors in version information");
            }
            return (7 + index);
        }

        public static FinderPattern findFinderPattern(bool[][] image)
        {
            Line[] crossLines = findLineCross(findLineAcross(image));
            Point[] centers = null;
            try
            {
                centers = getCenter(crossLines);
            }
            catch (FinderPatternNotFoundException exception)
            {
                throw exception;
            }
            int[] angle = getAngle(centers);
            centers = sort(centers, angle);
            int[] width = getWidth(image, centers, angle);
            int[] moduleSize = new int[] { (width[0] << QRCodeImageReader.DECIMAL_POINT) / 7, (width[1] << QRCodeImageReader.DECIMAL_POINT) / 7, (width[2] << QRCodeImageReader.DECIMAL_POINT) / 7 };
            int version = calcRoughVersion(centers, width);
            if (version > 6)
            {
                try
                {
                    version = calcExactVersion(centers, angle, moduleSize, image);
                }
                catch (VersionInformationException)
                {
                }
            }
            return new FinderPattern(centers, version, angle, width, moduleSize);
        }

        internal static Line[] findLineAcross(bool[][] image)
        {
            int num = 0;
            int num2 = 1;
            int length = image.Length;
            int num4 = image[0].Length;
            Point point = new Point();
            ArrayList list = ArrayList.Synchronized(new ArrayList(10));
            int[] buffer = new int[5];
            int index = 0;
            int num6 = num;
            bool flag = false;
            while (true)
            {
                bool flag2 = image[point.X][point.Y];
                if (flag2 == flag)
                {
                    buffer[index]++;
                }
                else
                {
                    if (!flag2 && checkPattern(buffer, index))
                    {
                        int x;
                        int y;
                        int num9;
                        int num10;
                        int num11;
                        if (num6 == num)
                        {
                            x = point.X;
                            for (num11 = 0; num11 < 5; num11++)
                            {
                                x -= buffer[num11];
                            }
                            num9 = point.X - 1;
                            y = num10 = point.Y;
                        }
                        else
                        {
                            x = num9 = point.X;
                            y = point.Y;
                            for (num11 = 0; num11 < 5; num11++)
                            {
                                y -= buffer[num11];
                            }
                            num10 = point.Y - 1;
                        }
                        list.Add(new Line(x, y, num9, num10));
                    }
                    index = (index + 1) % 5;
                    buffer[index] = 1;
                    flag = !flag;
                }
                if (num6 == num)
                {
                    if (point.X < (length - 1))
                    {
                        point.translate(1, 0);
                    }
                    else if (point.Y < (num4 - 1))
                    {
                        point.set_Renamed(0, point.Y + 1);
                        buffer = new int[5];
                    }
                    else
                    {
                        point.set_Renamed(0, 0);
                        buffer = new int[5];
                        num6 = num2;
                    }
                }
                else if (point.Y < (num4 - 1))
                {
                    point.translate(0, 1);
                }
                else if (point.X < (length - 1))
                {
                    point.set_Renamed(point.X + 1, 0);
                    buffer = new int[5];
                }
                else
                {
                    Line[] lines = new Line[list.Count];
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = (Line) list[i];
                    }
                    canvas.drawLines(lines, Color_Fields.LIGHTGREEN);
                    return lines;
                }
            }
        }

        internal static Line[] findLineCross(Line[] lineAcross)
        {
            int num;
            ArrayList list = ArrayList.Synchronized(new ArrayList(10));
            ArrayList list2 = ArrayList.Synchronized(new ArrayList(10));
            ArrayList list3 = ArrayList.Synchronized(new ArrayList(10));
            for (num = 0; num < lineAcross.Length; num++)
            {
                list3.Add(lineAcross[num]);
            }
            for (num = 0; num < (list3.Count - 1); num++)
            {
                list2.Clear();
                list2.Add(list3[num]);
                for (int i = num + 1; i < list3.Count; i++)
                {
                    Line line;
                    int num3;
                    if (Line.isNeighbor((Line) list2[list2.Count - 1], (Line) list3[i]))
                    {
                        list2.Add(list3[i]);
                        line = (Line) list2[list2.Count - 1];
                        if (((list2.Count * 5) > line.Length) && (i == (list3.Count - 1)))
                        {
                            list.Add(list2[list2.Count / 2]);
                            num3 = 0;
                            while (num3 < list2.Count)
                            {
                                list3.Remove(list2[num3]);
                                num3++;
                            }
                        }
                    }
                    else if (cantNeighbor((Line) list2[list2.Count - 1], (Line) list3[i]) || (i == (list3.Count - 1)))
                    {
                        line = (Line) list2[list2.Count - 1];
                        if ((list2.Count * 6) > line.Length)
                        {
                            list.Add(list2[list2.Count / 2]);
                            for (num3 = 0; num3 < list2.Count; num3++)
                            {
                                list3.Remove(list2[num3]);
                            }
                        }
                        break;
                    }
                }
            }
            Line[] lineArray = new Line[list.Count];
            for (num = 0; num < lineArray.Length; num++)
            {
                lineArray[num] = (Line) list[num];
            }
            return lineArray;
        }

        public virtual int[] getAngle() => 
            this.sincos;

        internal static int[] getAngle(Point[] centers)
        {
            int num;
            Line[] lines = new Line[3];
            for (num = 0; num < lines.Length; num++)
            {
                lines[num] = new Line(centers[num], centers[(num + 1) % lines.Length]);
            }
            Line line = Line.getLongest(lines);
            Point point = new Point();
            for (num = 0; num < centers.Length; num++)
            {
                if (!(line.getP1().equals(centers[num]) || line.getP2().equals(centers[num])))
                {
                    point = centers[num];
                    break;
                }
            }
            canvas.println("originPoint is: " + point);
            Point point2 = new Point();
            if ((point.Y <= line.getP1().Y) & (point.Y <= line.getP2().Y))
            {
                if (line.getP1().X < line.getP2().X)
                {
                    point2 = line.getP2();
                }
                else
                {
                    point2 = line.getP1();
                }
            }
            else if ((point.X >= line.getP1().X) & (point.X >= line.getP2().X))
            {
                if (line.getP1().Y < line.getP2().Y)
                {
                    point2 = line.getP2();
                }
                else
                {
                    point2 = line.getP1();
                }
            }
            else if ((point.Y >= line.getP1().Y) & (point.Y >= line.getP2().Y))
            {
                if (line.getP1().X < line.getP2().X)
                {
                    point2 = line.getP1();
                }
                else
                {
                    point2 = line.getP2();
                }
            }
            else if (line.getP1().Y < line.getP2().Y)
            {
                point2 = line.getP1();
            }
            else
            {
                point2 = line.getP2();
            }
            int length = new Line(point, point2).Length;
            return new int[] { (((point2.Y - point.Y) << QRCodeImageReader.DECIMAL_POINT) / length), (((point2.X - point.X) << QRCodeImageReader.DECIMAL_POINT) / length) };
        }

        public virtual Point[] getCenter() => 
            this.center;

        public virtual Point getCenter(int position)
        {
            if ((position >= 0) && (position <= 2))
            {
                return this.center[position];
            }
            return null;
        }

        internal static Point[] getCenter(Line[] crossLines)
        {
            int num;
            ArrayList list = ArrayList.Synchronized(new ArrayList(10));
            for (num = 0; num < (crossLines.Length - 1); num++)
            {
                Line line = crossLines[num];
                for (int i = num + 1; i < crossLines.Length; i++)
                {
                    Line line2 = crossLines[i];
                    if (Line.isCross(line, line2))
                    {
                        int x = 0;
                        int y = 0;
                        if (line.Horizontal)
                        {
                            x = line.Center.X;
                            y = line2.Center.Y;
                        }
                        else
                        {
                            x = line2.Center.X;
                            y = line.Center.Y;
                        }
                        list.Add(new Point(x, y));
                    }
                }
            }
            Point[] points = new Point[list.Count];
            for (num = 0; num < points.Length; num++)
            {
                points[num] = (Point) list[num];
            }
            if (points.Length != 3)
            {
                throw new FinderPatternNotFoundException("Invalid number of Finder Pattern detected");
            }
            canvas.drawPolygon(points, Color_Fields.RED);
            return points;
        }

        public virtual int getModuleSize() => 
            this.moduleSize[0];

        public virtual int getModuleSize(int place) => 
            this.moduleSize[place];

        internal static Point getPointAtSide(Point[] points, int side1, int side2)
        {
            Point point = new Point();
            int x = ((side1 == 1) || (side2 == 1)) ? 0 : 0x7fffffff;
            int y = ((side1 == 2) || (side2 == 2)) ? 0 : 0x7fffffff;
            point = new Point(x, y);
            for (int i = 0; i < points.Length; i++)
            {
                switch (side1)
                {
                    case 1:
                    {
                        if (point.X >= points[i].X)
                        {
                            break;
                        }
                        point = points[i];
                        continue;
                    }
                    case 2:
                    {
                        if (point.Y >= points[i].Y)
                        {
                            goto Label_0115;
                        }
                        point = points[i];
                        continue;
                    }
                    case 3:
                    {
                        continue;
                    }
                    case 4:
                    {
                        if (point.X <= points[i].X)
                        {
                            goto Label_01A7;
                        }
                        point = points[i];
                        continue;
                    }
                    case 8:
                    {
                        if (point.Y > points[i].Y)
                        {
                            point = points[i];
                        }
                        else if (point.Y == points[i].Y)
                        {
                            if (side2 != 1)
                            {
                                goto Label_0283;
                            }
                            if (point.X < points[i].X)
                            {
                                point = points[i];
                            }
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                if (point.X == points[i].X)
                {
                    if (side2 == 2)
                    {
                        if (point.Y < points[i].Y)
                        {
                            point = points[i];
                        }
                    }
                    else if (point.Y > points[i].Y)
                    {
                        point = points[i];
                    }
                }
                continue;
            Label_0115:
                if (point.Y == points[i].Y)
                {
                    if (side2 == 1)
                    {
                        if (point.X < points[i].X)
                        {
                            point = points[i];
                        }
                    }
                    else if (point.X > points[i].X)
                    {
                        point = points[i];
                    }
                }
                continue;
            Label_01A7:
                if (point.X == points[i].X)
                {
                    if (side2 == 2)
                    {
                        if (point.Y < points[i].Y)
                        {
                            point = points[i];
                        }
                    }
                    else if (point.Y > points[i].Y)
                    {
                        point = points[i];
                    }
                }
                continue;
            Label_0283:
                if (point.X > points[i].X)
                {
                    point = points[i];
                }
            }
            return point;
        }

        internal static int getURQuadant(int[] angle)
        {
            int num = angle[0];
            int num2 = angle[1];
            if ((num >= 0) && (num2 > 0))
            {
                return 1;
            }
            if ((num > 0) && (num2 <= 0))
            {
                return 2;
            }
            if ((num <= 0) && (num2 < 0))
            {
                return 3;
            }
            if ((num < 0) && (num2 >= 0))
            {
                return 4;
            }
            return 0;
        }

        public virtual int getWidth(int position) => 
            this.width[position];

        internal static int[] getWidth(bool[][] image, Point[] centers, int[] sincos)
        {
            int[] numArray = new int[3];
            for (int i = 0; i < 3; i++)
            {
                bool flag = false;
                int y = centers[i].Y;
                int x = centers[i].X;
                while (x > 0)
                {
                    if (image[x][y] && !image[x - 1][y])
                    {
                        if (flag)
                        {
                            break;
                        }
                        flag = true;
                    }
                    x--;
                }
                flag = false;
                int index = centers[i].X;
                while (index < image.Length)
                {
                    if (image[index][y] && !image[index + 1][y])
                    {
                        if (flag)
                        {
                            break;
                        }
                        flag = true;
                    }
                    index++;
                }
                numArray[i] = (index - x) + 1;
            }
            return numArray;
        }

        internal static Point[] sort(Point[] centers, int[] angle)
        {
            Point[] pointArray = new Point[3];
            switch (getURQuadant(angle))
            {
                case 1:
                    pointArray[1] = getPointAtSide(centers, 1, 2);
                    pointArray[2] = getPointAtSide(centers, 2, 4);
                    break;

                case 2:
                    pointArray[1] = getPointAtSide(centers, 2, 4);
                    pointArray[2] = getPointAtSide(centers, 8, 4);
                    break;

                case 3:
                    pointArray[1] = getPointAtSide(centers, 4, 8);
                    pointArray[2] = getPointAtSide(centers, 1, 8);
                    break;

                case 4:
                    pointArray[1] = getPointAtSide(centers, 8, 1);
                    pointArray[2] = getPointAtSide(centers, 2, 1);
                    break;
            }
            for (int i = 0; i < centers.Length; i++)
            {
                if (!(centers[i].equals(pointArray[1]) || centers[i].equals(pointArray[2])))
                {
                    pointArray[0] = centers[i];
                }
            }
            return pointArray;
        }

        public virtual int Version =>
            this.version;

        public virtual int SqrtNumModules =>
            (0x11 + (4 * this.version));
    }
}

