using System;
using System.Windows.Media.Imaging;

namespace Tracking.ProcessModule.Models
{
    public class ProcessResult
    {
        public bool ShowSplitImage { get; set; }
        public BitmapSource Source { get; set; }
        public BitmapSource Source1 { get; set; }
        public BitmapSource Source2 { get; set; }

        // seconds
        public TimeSpan TimeLeft { get; set; }
        public int Counts { get; set; }

        public double CalibrateFactor { get; set; }
    }
}
