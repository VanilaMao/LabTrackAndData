using System.Windows.Media;
using RandomColorGenerator;

namespace Lab.UICommon.Utilities
{
    public static class ColorfulHelper
    { 

        public static Color[] GetRandomColor()
        {
            return RandomColor.GetColors(ColorScheme.Random, Luminosity.Random,20);
        }
    }
}
