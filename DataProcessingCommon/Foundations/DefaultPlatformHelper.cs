using System;
using System.Windows.Input;

namespace Lab.Common.Foundations
{
    public class DefaultPlatformHelper : IPlatformHelper
    {
        public event EventHandler RequerySuggested
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
