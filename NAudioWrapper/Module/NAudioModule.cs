using NAudioWrapper.Helper;
using NAudioWrapper.Interface;
using NAudioWrapper.Model;
using Ninject.Modules;

namespace NAudioWrapper.Module
{
    public class NAudioModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAudioDeviceObject>().To<AudioDeviceObject>();
            Bind<AudioEventHelper>().ToSelf().InSingletonScope();
        }
    }
}
