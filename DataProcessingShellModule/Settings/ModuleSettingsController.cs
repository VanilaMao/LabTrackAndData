using System;

namespace Lab.ShellModule.Settings
{
    public abstract class ModuleSettingsController<T> where T:ModuleSettings
    {
        private readonly Lazy<T> _lazySettings;
        private T _settings;

        protected ModuleSettingsController(IAppSettings appSettings)
        {
            _lazySettings = new Lazy<T>(
                appSettings.LoadSettings<T>);
        }
        public T Settings
        {
            get
            {
                if (_settings == null)
                {
                    return _lazySettings.Value;
                }

                return _settings;
            }
            set => _settings = value;
        }
    }
}
