using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace AudioManager10.ViewModel.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        #region Fields

        private bool _isBusy;

        #endregion

        public OptionsViewModel()
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

        #endregion

        #region Private Methods

        private void Init()
        {
            InitCommands();
            
            RefreshCommand.Execute(null);
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
            
        }

        private bool RefreshCommandCanExcecute(object o)
        {
            return !_isBusy;
        }

        #endregion

    }
}
