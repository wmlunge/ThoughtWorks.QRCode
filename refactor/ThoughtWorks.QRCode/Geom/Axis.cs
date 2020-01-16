namespace ThoughtWorks.QRCode.Geom
{
    using System;
    using ThoughtWorks.QRCode.Codec.Reader;

    public class Axis
    {
        internal int sin;
        internal int cos;
        internal int modulePitch;
        internal Point origin;

        public Axis(int[] angle, int modulePitch)
        {
            this.sin = angle[0];
            this.cos = angle[1];
            this.modulePitch = modulePitch;
            this.origin = new Point();
        }

        public virtual Point translate(Point offset)
        {
            int x = offset.X;
            int y = offset.Y;
            return this.translate(x, y);
        }

        public virtual Point translate(int moveX, int moveY)
        {
            long num = QRCodeImageReader.DECIMAL_POINT;
            Point point = new Point();
            int num2 = (moveX == 0) ? 0 : ((this.modulePitch * moveX) >> ((int)num));
            int num3 = (moveY == 0) ? 0 : ((this.modulePitch * moveY) >> ((int)num));
            point.translate(((num2 * this.cos) - (num3 * this.sin)) >> ((int)num), ((num2 * this.sin) + (num3 * this.cos)) >> ((int)num));
            point.translate(this.origin.X, this.origin.Y);
            return point;
        }

        public virtual Point translate(Point origin, Point offset)
        {
            this.Origin = origin;
            int x = offset.X;
            int y = offset.Y;
            return this.translate(x, y);
        }

        public virtual Point translate(Point origin, int moveX, int moveY)
        {
            this.Origin = origin;
            return this.translate(moveX, moveY);
        }

        public virtual Point translate(Point origin, int modulePitch, int moveX, int moveY)
        {
            this.Origin = origin;
            this.modulePitch = modulePitch;
            return this.translate(moveX, moveY);
        }

        public virtual Point Origin
        {
            set =>
                this.origin = value;
        }

        public virtual int ModulePitch
        {
            set =>
                this.modulePitch = value;
        }
    }
}

