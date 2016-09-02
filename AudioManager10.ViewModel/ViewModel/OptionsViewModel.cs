using System;
using System.Windows.Input;
using AudioManager10.View.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace AudioManager10.ViewModel.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        #region Fields

        private bool _isBusy;
        private bool _showAlternativeOsd;
        private int _alternativeOsdRectCount;

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

        public bool ShowAlternativeOsd
        {
            get { return _showAlternativeOsd; }
            set
            {
                _showAlternativeOsd = value;
                Settings.Default.ShowAlternativeVolume = value;

                if (value) ShowAlternativeOsdDemanded?.Invoke(this, null); 
                else CloseAlternativeOsdDemanded?.Invoke(this, null);

                RaisePropertyChanged(() => ShowAlternativeOsd);
            }
        }

        public int AlternativeOsdRectCount
        {
            get { return _alternativeOsdRectCount; }
            set
            {
                _alternativeOsdRectCount = value;
                Settings.Default.AlternativeVolumeRectCount = value;
                RaisePropertyChanged(() => AlternativeOsdRectCount);
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
            ShowAlternativeOsd = Settings.Default.ShowAlternativeVolume;
            AlternativeOsdRectCount = Settings.Default.AlternativeVolumeRectCount;
        }

        #endregion

        #region Events

        public event EventHandler ShowAlternativeOsdDemanded;
        public event EventHandler CloseAlternativeOsdDemanded;

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