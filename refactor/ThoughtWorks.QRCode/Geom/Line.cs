namespace ThoughtWorks.QRCode.Geom
{
    using System;
    using ThoughtWorks.QRCode.Codec.Util;

    public class Line
    {
        internal int x1;
        internal int y1;
        internal int x2;
        internal int y2;

        public Line()
        {
            this.x1 = this.y1 = this.x2 = this.y2 = 0;
        }

        public Line(Point p1, Point p2)
        {
            this.x1 = p1.X;
            this.y1 = p1.Y;
            this.x2 = p2.X;
            this.y2 = p2.Y;
        }

        public Line(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public static Line getLongest(Line[] lines)
        {
            Line line = new Line();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > line.Length)
                {
                    line = lines[i];
                }
            }
            return line;
        }

        public virtual Point getP1() => 
            new Point(this.x1, this.y1);

        public virtual Point getP2() => 
            new Point(this.x2, this.y2);

        public static bool isCross(Line line1, Line line2)
        {
            if (line1.Horizontal && line2.Vertical)
            {
                if ((((line1.getP1().Y > line2.getP1().Y) && (line1.getP1().Y < line2.getP2().Y)) && (line2.getP1().X > line1.getP1().X)) && (line2.getP1().X < line1.getP2().X))
                {
                    return true;
                }
            }
            else if ((line1.Vertical && line2.Horizontal) && ((((line1.getP1().X > line2.getP1().X) && (line1.getP1().X < line2.getP2().X)) && (line2.getP1().Y > line1.getP1().Y)) && (line2.getP1().Y < line1.getP2().Y)))
            {
                return true;
            }
            return false;
        }

        public static bool isNeighbor(Line line1, Line line2) => 
            (((Math.Abs((int) (line1.getP1().X - line2.getP1().X)) < 2) && (Math.Abs((int) (line1.getP1().Y - line2.getP1().Y)) < 2)) && ((Math.Abs((int) (line1.getP2().X - line2.getP2().X)) < 2) && (Math.Abs((int) (line1.getP2().Y - line2.getP2().Y)) < 2)));

        public virtual void setLine(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public virtual void setP1(Point p1)
        {
            this.x1 = p1.X;
            this.y1 = p1.Y;
        }

        public virtual void setP1(int x1, int y1)
        {
            this.x1 = x1;
            this.y1 = y1;
        }

        public virtual void setP2(Point p2)
        {
            this.x2 = p2.X;
            this.y2 = p2.Y;
        }

        public virtual void setP2(int x2, int y2)
        {
            this.x2 = x2;
            this.y2 = y2;
        }

        public override string ToString() => 
            ("(" + Convert.ToString(this.x1) + "," + Convert.ToString(this.y1) + ")-(" + Convert.ToString(this.x2) + "," + Convert.ToString(this.y2) + ")");

        public virtual void translate(int dx, int dy)
        {
            this.x1 += dx;
            this.y1 += dy;
            this.x2 += dx;
            this.y2 += dy;
        }

        public virtual bool Horizontal =>
            (this.y1 == this.y2);

        public virtual bool Vertical =>
            (this.x1 == this.x2);

        public virtual Point Center
        {
            get
            {
                int x = (this.x1 + this.x2) / 2;
                return new Point(x, (this.y1 + this.y2) / 2);
            }
        }

        public virtual int Length
        {
            get
            {
                int num = Math.Abs((int) (this.x2 - this.x1));
                int num2 = Math.Abs((int) (this.y2 - this.y1));
                return QRCodeUtility.sqrt((num * num) + (num2 * num2));
            }
        }
    }
}

