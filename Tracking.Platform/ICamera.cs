namespace Tracking.Platform
{
    public delegate void ImageCapturedEventHandler(object sender, ImageCapturedEvent e);
    public interface ICamera
    {
        bool Open();
        bool Close();
        bool Stop();
        bool Start();
        bool IsOpened { get; }

        event ImageCapturedEventHandler ImageCaptured;
    }
}
