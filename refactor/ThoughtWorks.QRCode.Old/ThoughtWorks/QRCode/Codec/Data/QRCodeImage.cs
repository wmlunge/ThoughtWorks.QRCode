namespace ThoughtWorks.QRCode.Codec.Data
{
    using System;

    public interface QRCodeImage
    {
        int getPixel(int x, int y);

        int Width { get; }

        int Height { get; }
    }
}

