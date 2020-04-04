using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Lab.ShellModule.Events;
using Lab.ShellModule.Extensions;
using Lab.ShellModule.ViewModelModels;
using Prism.Events;

namespace Lab.ShellModule.Settings
{
    public class AppSettings :IAppSettings
    {
        private readonly IEventAggregator _eventAggregator;

        private static readonly string FolderName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                           $"\\ShawnLab\\Settings";
        public AppSettings(AppSettingsModel model, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Settingses = new Dictionary<string, Settings>();
            DefaultSettingses=new Dictionary<string, Settings>();
            Model = model;
            Id= new Random().Next();
            model.AppSettingModelChangedEventHandler += (o, e) => { SaveAllSettings(model); };
            model.SetItemPropertyChanged((o, e) => SaveAllSettings(model));
            Directory.CreateDirectory(FolderName);
        }

        public int Id { get; }

        private AppSettingsModel Model { get; set; }

        private Dictionary<string, Settings> DefaultSettingses { get; set; }
        private Dictionary<string,Settings> Settingses { get; }

        //must happen after LoadAllSettings get called, that means it can't be called in module initlized
        // or any module register
        // https://stackoverflow.com/questions/34316613/cannot-serialize-member-of-type-system-collections-generic-dictionary2-beca
        // https://stackoverflow.com/questions/7620718/datacontractserializer-and-dictionarystring-object-fails-when-reading
        private Settings LoadSettingsInternal(string name)  
        {
            var defaultSettings = DefaultSettingses[name];          
            var file = GetConfigName(name);
            if (!File.Exists(file)) return defaultSettings;

            DataContractSerializer serializer = new DataContractSerializer(defaultSettings.GetType());

            var reader = XmlReader.Create(file);
            var settings = serializer.ReadObject(reader) as Settings;
            if (settings != null)
            {
                settings.Name = name;
            }

            reader.Close();
            return settings;

        }

        public Settings LoadSettings(string name)
        {
            return Settingses[name];
        }



        public void LoadAllSettings()
        {
            var instances = typeof(Settings).GetAllSubClasses()
                .Select(x=>Activator.CreateInstance(x) as Settings).ToList();
            DefaultSettingses = instances.ToDictionary(x => x.Name, y => y);

            var keyList = Settingses.Keys.ToList();
            //load necessary settings
            foreach (var key in keyList)
            {
                Settingses[key] = LoadSettingsInternal(key);
            }

            foreach (var settingsPair in DefaultSettingses)
            {
                var name = settingsPair.Key;
                var settings = Settingses.ContainsKey(name) ?
                    Settingses[name] : settingsPair.Value;
                settings.MapToModel(Model,()=>SaveAllSettings(Model));
            }
        }

        private void SaveAllSettings(AppSettingsModel appSettingsModel)
        {
            foreach (var settingsPair in Settingses)
            {
                var name = settingsPair.Key;
                var settings = Settingses[name];
                if (!settings.IsNotify) continue;
                settings.MapModelToSettings(appSettingsModel);
                //setting name must equal keyname
                SaveSettings(settings);
                _eventAggregator.GetEvent<SettingChangedEvent>().Publish(settings.Name);
            }          
        }

        public void SaveSettings(Settings settings)
        {
            var file = GetConfigName(settings.Name);

            DataContractSerializer serializer = new DataContractSerializer(settings.GetType());
            using (var sw = XmlWriter.Create(file))
            {
                serializer.WriteObject(sw, settings);
            } 
        }

        public T LoadSettings<T>() where T : Settings
        {
            var settings = Settingses.FirstOrDefault(x => x.Value.GetType() == typeof(T));
            return (T)settings.Value;
        }

        private static string GetConfigName(string file)
        {
            return FolderName+$"\\{file}.xml";
        }


        // probabaly register it use type
        public void RegisterSettings(string name)
        {
            if (!Settingses.ContainsKey(name))
            {
                Settingses.Add(name,null);
            }
        }

        public void Update(Settings settings)
        {
            settings?.MapToModel(Model);
            // need to create a inherit expandobject to track dirty
            SaveAllSettings(Model);
        }
        
    }
}
