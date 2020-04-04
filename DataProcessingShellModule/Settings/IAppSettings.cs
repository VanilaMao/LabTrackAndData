

namespace Lab.ShellModule.Settings
{
    public interface IAppSettings
    {
        Settings LoadSettings(string name);
        void SaveSettings(Settings settings);

        T LoadSettings<T>() where T : Settings;

        void RegisterSettings(string name);

        void Update(Settings settings);
    }
}
