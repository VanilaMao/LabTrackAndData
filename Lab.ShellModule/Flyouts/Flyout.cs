using System.Windows.Input;
using Lab.UICommon.Positions;
using Prism.Commands;
using Prism.Mvvm;

namespace Lab.ShellModule.Flyouts
{
    public abstract class Flyout : BindableBase, IFlyout
    {
        private bool _isFlyoutOpen;

        protected Flyout()
        {
            CancelCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(SaveExit);
            CloseCommand = new DelegateCommand(Close);
        }

        public virtual string DisplayName { get; set; }


        public virtual DockPosition SlideDirection => DockPosition.Right;

        public bool IsFlyoutOpen
        {
            get => _isFlyoutOpen;
            set => SetProperty(ref _isFlyoutOpen, value, nameof(IsFlyoutOpen));
        }

        private void SaveExit()
        {
            Close();
            Save();
        }

        protected virtual void Save()
        {
            //do nothing
        }

        protected virtual void OnDeactivated()
        {
        }

        protected void Cancel()
        {
            Close();
        }

        private  void Close()
        {
            OnDeactivated();
            IsFlyoutOpen = false;
        }

        public ICommand CancelCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand CloseCommand { get; }

        public virtual Length Length => Length.Auto;
    }
}
