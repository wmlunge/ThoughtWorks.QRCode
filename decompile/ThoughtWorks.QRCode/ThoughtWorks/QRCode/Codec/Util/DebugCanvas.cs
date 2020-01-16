namespace ThoughtWorks.QRCode.Codec.Util
{
    using System;
    using ThoughtWorks.QRCode.Geom;

    public interface DebugCanvas
    {
        void drawCross(Point point, int color);
        void drawLine(Line line, int color);
        void drawLines(Line[] lines, int color);
        void drawMatrix(bool[][] matrix);
        void drawPoint(Point point, int color);
        void drawPoints(Point[] points, int color);
        void drawPolygon(Point[] points, int color);
        void println(string str);
    }
}

