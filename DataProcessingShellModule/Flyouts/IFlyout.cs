using System.Windows.Input;
using Lab.UICommon.Positions;

namespace Lab.ShellModule.Flyouts
{
    public interface IFlyout
    {
        string DisplayName { get; }
        DockPosition SlideDirection { get; }
        bool IsFlyoutOpen { get; set; }

        ICommand CancelCommand { get; }

        ICommand SaveCommand { get; }

        ICommand CloseCommand { get; }

        Length Length { get; }

    }
}
