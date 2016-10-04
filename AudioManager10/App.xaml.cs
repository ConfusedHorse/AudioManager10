using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AudioManager10.Properties;
using AudioManager10.View.Control.TrayControl;
using AudioManager10.View.Module;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;

namespace AudioManager10
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Mutex Mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            // check target OS version
            if (!IsWindows10() && !Settings.Default.WarningDialogAccepted)
            {
                var alreadyRunningMessageBoxResult =
                BlurryMessageBox.Show(
                    View.Properties.Resources.NotWindows10Description,
                    View.Properties.Resources.NotWindows10,
                    BlurryDialogButton.OkCancel,
                    BlurryDialogIcon.Warning, 0.5
                );
                if (alreadyRunningMessageBoxResult == BlurryDialogResult.Cancel ||
                    alreadyRunningMessageBoxResult == BlurryDialogResult.None) Current.Shutdown();
                else Settings.Default.WarningDialogAccepted = true;
            }

            // Single instance magic
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                // Start application on Windows startup
                InstallMeOnStartUp();
                base.OnStartup(e);

                // Start application in system tray
                ViewModelLocator.Instance.TrayManager.Initialize();
            }
            else
            {
                var alreadyRunningMessageBoxResult =
                BlurryMessageBox.Show(
                    View.Properties.Resources.AlreadyRunningDescription,
                    View.Properties.Resources.AlreadyRunning,
                    BlurryDialogButton.Ok,
                    BlurryDialogIcon.Error, 0.5
                );
                Current.Shutdown();
            }

            //copy EndPointController to C:\temp\EndPointController
            var exeBytes = AudioManager10.Properties.Resources.EndPointController;
            const string exeToRun = @"C:\temp\EndPointController.exe";
            if (!File.Exists(exeToRun))
            using (var exeFile = new FileStream(exeToRun, FileMode.CreateNew))
            {
                exeFile.Write(exeBytes, 0, exeBytes.Length);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ViewModelLocator.Instance.TrayManager?.Terminate();
            try { Mutex.ReleaseMutex(); } catch (Exception) { /* bla */ }

            Settings.Default.Save();
            base.OnExit(e);
        }

        private static void InstallMeOnStartUp()
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

        private static bool IsWindows10()
        {
            var name =
                (from x in
                    new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get()
                        .Cast<ManagementObject>()
                    select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null && name.ToString().Contains("Microsoft Windows 10");
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionManagementMessageBoxResult =
            BlurryMessageBox.Show(
                View.Properties.Resources.ErrorDescription + "\r\n" + e.Exception,
                View.Properties.Resources.Error,
                BlurryDialogButton.Ok,
                BlurryDialogIcon.Error, 0.5
            );
            Current.Shutdown();
        }
    }
}
