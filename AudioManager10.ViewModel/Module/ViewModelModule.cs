using AudioManager10.ViewModel.ViewModel;
using Ninject.Modules;

namespace AudioManager10.ViewModel.Module
{
    public class ViewModelModule : NinjectModule
    {
        public override void Load()
        {
            Bind<TrayIconViewModel>().ToSelf().InSingletonScope();
            Bind<AudioDevicesViewModel>().ToSelf().InSingletonScope();
            Bind<OptionsViewModel>().ToSelf().InSingletonScope();
        }
    }
}
