using System.Windows;
using System.Windows.Controls;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;

namespace AudioManager10.View.Control
{
    /// <summary>
    /// Interaction logic for OptionsControl.xaml
    /// </summary>
    public partial class OptionsControl : UserControl
    {
        public OptionsControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var notImplementedDialogResult = 
                BlurryMessageBox.Show(
                    Properties.Resources.NotImplementedDescription,
                    Properties.Resources.NotImplemented, 
                    BlurryDialogButton.Ok, 0.5
                );
        }
    }
}
