namespace ThoughtWorks.QRCode.Codec.Data
{
    using System;
    using System.Drawing;

    public class QRCodeBitmapImage : QRCodeImage
    {
        private Bitmap image;

        public QRCodeBitmapImage(Bitmap image)
        {
            this.image = image;
        }

        public virtual int getPixel(int x, int y) => 
            this.image.GetPixel(x, y).ToArgb();

        public virtual int Width =>
            this.image.Width;

        public virtual int Height =>
            this.image.Height;
    }
}

