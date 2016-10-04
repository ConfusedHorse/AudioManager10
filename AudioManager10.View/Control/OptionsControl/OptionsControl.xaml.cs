using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace AudioManager10.View.Control.OptionsControl
{
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : UserControl
    {
        private bool _isExpanded;
        private double _optionsHeight;

        public OptionsControl()
        {
            InitializeComponent();
            Loaded += delegate { InitializeOptionsHeight(); };
            }

        private void InitializeOptionsHeight()
        {
            _optionsHeight = OptionsPanel.Children.Cast<FrameworkElement>().Sum(userControl => userControl.ActualHeight);
            _isExpanded = false;

            OptionsPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowOptionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isExpanded) Minimize();
            else Maximize();
        }

        private void Maximize()
        {
            _isExpanded = true;
            OptionsPanel.Visibility = Visibility.Visible;

            Dispatcher.Invoke(() =>
            {
                var heightAnimation = new DoubleAnimation
                {
                    From = 32,
                    To = _optionsHeight + 32,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500)),
                    EasingFunction = new ExponentialEase() { Exponent = 15 }
                };
                BeginAnimation(HeightProperty, heightAnimation);
            });
        }

        private void Minimize()
        {
            _isExpanded = false;
            OptionsPanel.Visibility = Visibility.Collapsed;

            Dispatcher.Invoke(() =>
            {
                var heightAnimation = new DoubleAnimation
                {
                    From = _optionsHeight + 32,
                    To = 32,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300)),
                    EasingFunction = new ExponentialEase() { Exponent = 15 }
                };
                BeginAnimation(HeightProperty, heightAnimation);
            });
        }
    }
}
