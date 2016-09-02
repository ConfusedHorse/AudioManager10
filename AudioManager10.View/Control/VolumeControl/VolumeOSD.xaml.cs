using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using AudioManager10.View.Module;
using AudioManager10.View.Resource.Enum;
using NAudioWrapper.Interface;

namespace AudioManager10.View.Control.VolumeControl
{
    /// <summary>
    /// Interaction logic for VolumeOSD.xaml
    /// </summary>
    public partial class VolumeOsd : Window
    {
        #region Fields
        
        private readonly DispatcherTimer _windowActivatedDispatcherTimer = new DispatcherTimer();
        private IAudioDeviceObject _dataContext;
        private IntPtr _nativeOsd;

        #endregion Fields

        #region Dependency Properties

        public static readonly DependencyProperty VolumeRectCountProperty = DependencyProperty.Register(
            "VolumeRectCount", typeof(int), typeof(VolumeOsd),
            new PropertyMetadata(ViewModelLocator.Instance.OptionsViewModel.AlternativeOsdRectCount.GetRectCount()));
        
        public int VolumeRectCount
        {
            get { return (int) GetValue(VolumeRectCountProperty); }
            set
            {
                SetValue(VolumeRectCountProperty, value.GetRectCount());
                InitializeRectangles();
                ActivateWindow();
            }
        }

        #endregion Dependency Properties

        public VolumeOsd()
        {
            ManageNativeOsd();
            InitializeComponent();
            InitializeParameters();
            InitializeRectangles();
            AdjustLocalDataContext();
        }

        private void ManageNativeOsd()
        {
            Loaded += delegate { HideNativeOsd(); };
            Closing += delegate { RestoreNativeOsd(); };
        }

        private void InitializeRectangles()
        {
            VolumeControl.Children.Clear();
            for (var i = 0; i < VolumeRectCount; i++)
            {
                VolumeControl.Children.Add(GetRect(0.99d - (double)i/ VolumeRectCount));
            }
        }

        private static VolumeRectangle GetRect(double threshold)
        {
            return new VolumeRectangle(threshold)
            {
                Width = 80,
                Height = 18,
                BorderThickness = 3,
                VisualVolumeImpact = 6
            };
        }

        private void InitializeParameters()
        {
            Loaded += delegate
            {
                Height = SystemParameters.WorkArea.Height;
                WindowState = WindowState.Minimized;
            };

            DataContextChanged += delegate
            {
                InitializeRectangles();
                AdjustLocalDataContext();
            };

            _windowActivatedDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 2);
            _windowActivatedDispatcherTimer.Tick += WindowActivatedDispatcherTimerOnTick;
        }

        private void WindowActivatedDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            BeginAnimation(OpacityProperty, null);
            _windowActivatedDispatcherTimer.Stop();
            DeactivationAnimation();
        }

        private void AdjustLocalDataContext()
        {
            _dataContext = (IAudioDeviceObject)DataContext;
            _dataContext.DefaultMasterVolumeChanged += delegate { ActivateWindow();};
        }

        private void ActivateWindow()
        {
            _windowActivatedDispatcherTimer.Stop();
            _windowActivatedDispatcherTimer.Start();
            
            Dispatcher.Invoke(() => WindowState = WindowState.Normal);
            if (Dispatcher.Invoke(() => WindowState == WindowState.Normal && Opacity > 0.25)) return;
            ActivationAnimation();
        }

        #region Comforting Animation

        private void ActivationAnimation()
        {
            Dispatcher.Invoke(() =>
            {
                //BeginAnimation(OpacityProperty, null);

                var opacityAnimation = new DoubleAnimation
                {
                    From = Opacity,
                    To = 0.85,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 3000)),
                    EasingFunction = new ExponentialEase {Exponent = 15}
                };

                BeginAnimation(OpacityProperty, opacityAnimation);
            });
        }

        private void DeactivationAnimation()
        {
            Dispatcher.Invoke(() =>
            {
                var opacityAnimation = new DoubleAnimation
                {
                    From = Opacity,
                    To = 0,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 2000)),
                    EasingFunction = new ExponentialEase {Exponent = 15}
                };
                opacityAnimation.Completed += delegate 
                {
                    Dispatcher.Invoke(() => WindowState = WindowState.Minimized);
                };

                BeginAnimation(OpacityProperty, opacityAnimation);
            });
        }

        #endregion Comforting Animation

        #region ClickThrough

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        #endregion ClickThrough

        #region ManageNativeOsd

        private void HideNativeOsd()
        {
            _nativeOsd = FindOsdWindow();
            HideOsd();
        }

        private void RestoreNativeOsd()
        {
            ShowOsd();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static IntPtr FindOsdWindow()
        {
            var hwndRet = IntPtr.Zero;
            var hwndHost = IntPtr.Zero;

            var pairCount = 0;

            while ((hwndHost = FindWindowEx(IntPtr.Zero, hwndHost, "NativeHWNDHost", "")) != IntPtr.Zero)
            {
                if (FindWindowEx(hwndHost, IntPtr.Zero, "DirectUIHWND", "") == IntPtr.Zero) continue;
                if (pairCount == 0) hwndRet = hwndHost;
                pairCount++;

                if (pairCount <= 1) continue;
                MessageBox.Show("Severe error: Multiple pairs found!", "HideVolumeOSD");
                return IntPtr.Zero;
            }
            return hwndRet;
        }

        public void HideOsd()
        {
            ShowWindow(_nativeOsd, 6); // SW_MINIMIZE
        }

        public void ShowOsd()
        {
            ShowWindow(_nativeOsd, 9); // SW_RESTORE
        }

        #endregion ManageNativeOsd
    }

    #region ClickThrough

    internal static class WindowsServices
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }

    #endregion ClickThrough
}
