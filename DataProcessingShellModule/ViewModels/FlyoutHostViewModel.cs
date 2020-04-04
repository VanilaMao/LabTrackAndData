using Lab.ShellModule.Events;
using Lab.ShellModule.Flyouts;
using Prism.Events;
using Prism.Mvvm;

namespace Lab.ShellModule.ViewModels
{
    public class FlyoutHostViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private IFlyout _flyout;

        public FlyoutHostViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SubScribeEvents();
        }

        public IFlyout Flyout
        {
            get => _flyout;
            set => SetProperty(ref _flyout ,value , nameof(Flyout));
        }

        private void SubScribeEvents()
        {
            _eventAggregator.GetEvent<ShowFlyoutEvent>().Subscribe(
                flyout =>
                {
                    Flyout = flyout;
                    Flyout.IsFlyoutOpen = true;                   
                }, ThreadOption.UIThread);
        }
    }
}
