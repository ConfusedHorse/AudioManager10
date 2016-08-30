using System;
using System.Windows;
using AudioManager10.ViewModel.ViewModel;
using BlurryControls.Windows;

namespace AudioManager10.View.Control.TrayControl
{
    /// <summary>
    /// Interaction logic for TrayWindow.xaml
    /// </summary>
    public partial class TrayWindow : BlurryTrayBase
    {
        private TrayIconViewModel _instance;

        public TrayWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            _instance = DataContext as TrayIconViewModel;
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _instance.TrayWindowIsOpened = true;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            _instance.TrayWindowIsOpened = false;
        }

        private void Content_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Height = AudioDevicesControl.ActualHeight + OptionsControl.ActualHeight;
        }
    }
}
