using System;

namespace Tracking.Platform
{
    public class ImageCapturedEvent:EventArgs
    {
        public ImageCapturedEvent(ushort[] data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }

        public ushort[] Data { get; }
        public int Width { get; }
        public int Height { get; }
    }
}
