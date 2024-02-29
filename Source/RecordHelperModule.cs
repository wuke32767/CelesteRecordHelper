using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.RecordHelper
{
    public class RecordHelperModule : EverestModule
    {
        public static RecordHelperModule Instance { get; private set; }

        public override Type SettingsType => typeof(RecordHelperModuleSettings);
        public static RecordHelperModuleSettings Settings => (RecordHelperModuleSettings)Instance._Settings;

        public override Type SessionType => typeof(RecordHelperModuleSession);
        public static RecordHelperModuleSession Session => (RecordHelperModuleSession)Instance._Session;

        public override Type SaveDataType => typeof(RecordHelperModuleSaveData); 
        public static RecordHelperModuleSaveData SaveData => (RecordHelperModuleSaveData)Instance._SaveData;

        public RecordHelperModule()
        {
            Instance = this;
#if DEBUG
            // debug builds use verbose logging
            Logger.SetLogLevel(nameof(RecordHelperModule), LogLevel.Verbose);
#else
            // release builds use info logging to reduce spam in log files
            Logger.SetLogLevel(nameof(RecordHelperModule), LogLevel.Info);
#endif
        }

        public override void Load()
        {
            // TODO: apply any hooks that should always be active
        }

        public override void Unload()
        {
            // TODO: unapply any hooks applied in Load()
        }
    }
}
