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

        public virtual int getPixel(int x, int y)
        {
            return this.image.GetPixel(x, y).ToArgb();
        }

        public virtual int Height
        {
            get
            {
                return this.image.Height;
            }
        }

        public virtual int Width
        {
            get
            {
                return this.image.Width;
            }
        }
    }
}

