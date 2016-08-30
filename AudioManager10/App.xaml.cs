using System.Reflection;
using System.Windows;
using AudioManager10.View.Control.TrayControl;

namespace AudioManager10
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TrayManager _trayManager;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            InstallMeOnStartUp();
            base.OnStartup(e);

            //start application in system tray
            _trayManager = new TrayManager();
            _trayManager.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //the icon would clean up automatically, but this is cleaner
            _trayManager.Terminate();

            base.OnExit(e);
        }

        private void InstallMeOnStartUp()
        {
            try
            {
                var key =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
            }
            catch { }
        }
    }
}
