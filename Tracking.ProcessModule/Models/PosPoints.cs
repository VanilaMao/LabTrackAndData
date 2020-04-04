
namespace Tracking.ProcessModule.Models
{
    public struct PosPoints
    {
        public PosPoints(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
    }
}
