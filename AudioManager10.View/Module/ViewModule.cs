using AudioManager10.View.Control.TrayControl;
using Ninject.Modules;

namespace AudioManager10.View.Module
{
    public class ViewModule : NinjectModule
    {
        public override void Load()
        {
            Bind<TrayManager>().ToSelf().InSingletonScope();
        }
    }
}
