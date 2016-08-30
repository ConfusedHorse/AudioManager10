using Ninject.Modules;

namespace AudioManager10.Settings
{
    public class SettingsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AudioSettings>().ToSelf().InSingletonScope();
        }
    }
}