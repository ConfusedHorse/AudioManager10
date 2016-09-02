using System.ComponentModel;
using AudioManager10.View.Control.TrayControl;
using AudioManager10.ViewModel.Module;
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

        // ViewModel
        public TrayIconViewModel TrayIconViewModel => NinjectKernel.StandardKernel.Get<TrayIconViewModel>();
        public AudioDevicesViewModel AudioDevicesViewModel => NinjectKernel.StandardKernel.Get<AudioDevicesViewModel>();
        public OptionsViewModel OptionsViewModel => NinjectKernel.StandardKernel.Get<OptionsViewModel>();

        // View
        public TrayManager TrayManager => NinjectKernel.StandardKernel.Get<TrayManager>();

        public static void Cleanup()
        {

        }
    }
}