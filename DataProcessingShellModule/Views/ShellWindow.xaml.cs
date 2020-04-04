using Fluent;
using Lab.Common.ApplicationHelpers;

namespace Lab.ShellModule.Views
{
    /// <summary>
    /// Interaction logic for RibbonWindow.xaml
    /// </summary>
    public partial class ShellWindow  : RibbonWindow
    {
        public ShellWindow()
        {       
            InitializeComponent();
            Title += "  Version: (" + ApplicationInfo.Version + ")";
        }
    }
}
