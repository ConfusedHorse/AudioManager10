using System.ComponentModel;
using AudioManager10.DependencyInjector;
using AudioManager10.Settings;
using AudioManager10.ViewModel.ViewModel;
using Ninject;

namespace AudioManager10.View.Module
{
    public class ViewModelLocator
    {
        #region Singleton

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return null;
                return _instance ?? (_instance = new ViewModelLocator());
            }
        }

        #endregion Singleton

        public TrayIconViewModel TrayIconViewModel => NinjectKernel.StandardKernel.Get<TrayIconViewModel>();
        public AudioDevicesViewModel AudioDevicesViewModel => NinjectKernel.StandardKernel.Get<AudioDevicesViewModel>();
        public AudioSettings AudioSettings => NinjectKernel.StandardKernel.Get<AudioSettings>();
        public OptionsViewModel OptionsViewModel => NinjectKernel.StandardKernel.Get<OptionsViewModel>();

        public static void Cleanup()
        {

        }
    }
}