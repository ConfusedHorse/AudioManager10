using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace AudioManager10.ViewModel.ViewModel
{
    public class TrayIconViewModel : ViewModelBase
    {
        #region Fields

        private bool _isBusy;
        private bool _trayWindowIsOpened;
        private string _iconToolTipText;
        private const string DefaultIconToolTipText = "AudioManager10";

        #endregion

        public TrayIconViewModel()
        {
            Init();
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

        public bool TrayWindowIsOpened
        {
            get { return _trayWindowIsOpened; }
            set
            {
                _trayWindowIsOpened = value;
                RaisePropertyChanged(() => TrayWindowIsOpened);
            }
        }

        public string IconToolTipText
        {
            get { return _iconToolTipText; }
            set
            {
                _iconToolTipText = value;
                RaisePropertyChanged(() => IconToolTipText);
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            InitCommands();

            IconToolTipText = DefaultIconToolTipText;
            RefreshCommand.Execute(null);
        }

        #endregion

        #region Events
        
        public event EventHandler ShowTrayWindowDemanded;
        public event EventHandler CloseTrayWindowDemanded;

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; internal set; }
        public ICommand ShowWindowCommand { get; internal set; }
        public ICommand ShowHideWindowCommand { get; internal set; }
        public ICommand HideWindowCommand { get; internal set; }
        public ICommand ExitApplicationCommand { get; internal set; }

        private void InitCommands()
        {
            RefreshCommand = new RelayCommand<object>(RefreshCommandExcecute, RefreshCommandCanExcecute);
            ShowWindowCommand = new RelayCommand<object>(ShowWindowCommandExcecute, ShowWindowCommandCanExcecute);
            ShowHideWindowCommand = new RelayCommand<object>(ShowHideWindowCommandExcecute, ShowHideWindowCommandCanExcecute);
            HideWindowCommand = new RelayCommand<object>(HideWindowCommandExcecute, HideWindowCommandCanExcecute);
            ExitApplicationCommand = new RelayCommand<object>(ExitApplicationCommandExcecute, ExitApplicationCommandCanExcecute);
        }

        private void RefreshCommandExcecute(object o)
        {
            
        }

        private void ShowWindowCommandExcecute(object o)
        {
            ShowTrayWindowDemanded?.Invoke(this, null);
        }

        private void ShowHideWindowCommandExcecute(object o)
        {
            if (_trayWindowIsOpened)
                CloseTrayWindowDemanded?.Invoke(this, null);
            else
                ShowTrayWindowDemanded?.Invoke(this, null);
        }

        private void HideWindowCommandExcecute(object o)
        {
            CloseTrayWindowDemanded?.Invoke(this, null);
        }

        private static void ExitApplicationCommandExcecute(object o)
        {
            Application.Current.Shutdown();
        }

        private bool RefreshCommandCanExcecute(object o)
        {
            return !_isBusy;
        }

        private bool ShowWindowCommandCanExcecute(object o)
        {
            return !_trayWindowIsOpened;
        }

        private static bool ShowHideWindowCommandCanExcecute(object o)
        {
            return true;
        }

        private bool HideWindowCommandCanExcecute(object o)
        {
            return _trayWindowIsOpened;
        }

        private static bool ExitApplicationCommandCanExcecute(object o)
        {
            return true;
        }

        #endregion

    }
}
