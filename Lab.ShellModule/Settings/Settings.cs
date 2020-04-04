using System;
using System.Runtime.Serialization;
using Lab.ShellModule.ViewModelModels;

namespace Lab.ShellModule.Settings
{
    [DataContract]
    public abstract class Settings
    {
        [IgnoreDataMember]
        public abstract bool IsNotify { get; }

        public string Name { get; set; }

        // add a roll back settings inside, all as Nullable property, when save settings, check roll back object, if not null, will use value from roll back
        public virtual void RollBack(){}

        public abstract void MapToModel(AppSettingsModel appSettingsModel, Action changeAction=null);

        public abstract void MapModelToSettings(AppSettingsModel appSettingsModel);
    }
}
