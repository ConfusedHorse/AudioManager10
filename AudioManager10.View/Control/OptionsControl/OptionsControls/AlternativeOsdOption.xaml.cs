using System.Windows;
using System.Windows.Controls;
using AudioManager10.View.Module;

namespace AudioManager10.View.Control.OptionsControl.OptionsControls
{
    /// <summary>
    /// Interaction logic for AlternativeOsdOption.xaml
    /// </summary>
    public partial class AlternativeOsdOption : UserControl
    {
        public AlternativeOsdOption()
        {
            InitializeComponent();
        }

        private void AlternativeOsdRectCountSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider) sender;
            var volumeOsd = ViewModelLocator.Instance.TrayManager.GetVolumeOsd;
            if (sender == null || volumeOsd == null) return;
            volumeOsd.VolumeRectCount = (int)slider.Value;
        }
    }
}
