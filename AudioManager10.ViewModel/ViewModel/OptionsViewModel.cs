using System.Windows.Input;
using AudioManager10.ViewModel.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace AudioManager10.ViewModel.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        #region Fields

        private bool _isBusy;
        private bool _startOnWindowsStartup;

        #endregion

        public OptionsViewModel()
        {
            Init();
        }

        ~OptionsViewModel()
        {
            Settings.Default.Save();
        }

        #region Properties

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public bool StartOnWindowsStartup
        {
            get { return _startOnWindowsStartup; }
            set
            {
                _startOnWindowsStartup = value;
                Settings.Default.StartOnWindowsStartup = value;
                Settings.Default.Save();

                RaisePropertyChanged(() => StartOnWindowsStartup);
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            InitCommands();

            RefreshCommand.Execute(null);
        }

        private void InitializeSettings()
        {
            StartOnWindowsStartup = Settings.Default.StartOnWindowsStartup;
        }

        #endregion

        #region Events

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; internal set; }

        private void InitCommands()
        {
            RefreshCommand = new RelayCommand<object>(RefreshCommandExcecute, RefreshCommandCanExcecute);
        }

        private void RefreshCommandExcecute(object o)
        {
            InitializeSettings();
        }

        private bool RefreshCommandCanExcecute(object o)
        {
            return !_isBusy;
        }

        #endregion

    }
}