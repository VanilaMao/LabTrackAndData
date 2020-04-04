using System;
using System.Runtime.Serialization;
using Lab.ShellModule.ViewModelModels;

namespace Lab.ShellModule.Settings
{
    // use datacontract to serilize and deserilize
    [DataContract]
    public abstract class ModuleSettings :Settings
    {
        private bool _isNotify;
        //each module settings has its on setting groupbox in ribbon
        //and the software has its own settings tab, dynamic to create
        // control for every settings also can do somehing like previous job did,
        // use dayamic object and create template by its data type
        //refactor it in the future

        //dynamic object list here

        //maybe responsive reactive framwork (obserable/observe) pattern here

        //currently, cannot decide which pattern is going to be used

        //automapper?
        [IgnoreDataMember]
        public override bool IsNotify => _isNotify;

        public override void MapToModel(AppSettingsModel appSettingsModel, Action changeAction=null)
        {
            _isNotify = false;
            MapToModelInternal(appSettingsModel, changeAction);
            _isNotify = true;
        }

        
        protected abstract void MapToModelInternal(AppSettingsModel appSettingsModel, Action changeAction);

    }
}
