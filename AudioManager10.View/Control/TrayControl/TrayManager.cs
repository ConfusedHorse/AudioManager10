using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using AudioManager10.View.Module;
using AudioManager10.View.Resource.Enum;
using AudioManager10.ViewModel.ViewModel;
using Hardcodet.Wpf.TaskbarNotification;
using NAudioWrapper.Helper;
using NAudioWrapper.Interface;

namespace AudioManager10.View.Control.TrayControl
{
    public class TrayManager
    {
        private TaskbarIcon _notifyIcon;
        private TrayIconViewModel _instance;
        private TrayWindow _trayWindow;
        private readonly Icon[] _icos = new Icon[5];

        public TrayWindow TrayWindow => _trayWindow ?? (_trayWindow = new TrayWindow(){ });

        public void Initialize()
        {
            _instance = ViewModelLocator.Instance.TrayIconViewModel;
            _notifyIcon = (TaskbarIcon) Application.Current.FindResource("NotifyIcon");
            
            InitializeIcos();
            InitializeEventHandler();
            InitializeTrayAppearance();
        }

        public void Terminate()
        {
            _notifyIcon.Dispose();
        }

        private void InitializeTrayAppearance()
        {
            var defaultMultimediaRenderDevice = ViewModelLocator.Instance.AudioDevicesViewModel.DefaultMultimediaRenderDevice;
            var args = new VolumeChangedEventArgs
            {
                FriendlyName = defaultMultimediaRenderDevice.ActualDevice.FriendlyName,
                NewVolume = (float)Math.Round(defaultMultimediaRenderDevice.ActualDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100)
            };

            SelectIconFromMasterVolume(defaultMultimediaRenderDevice, args);
        }

        private void InitializeEventHandler()
        {
            _instance.ShowTrayWindowDemanded += InstanceOnShowTrayWindowDemanded;
            _instance.CloseTrayWindowDemanded += InstanceOnCloseTrayWindowDemanded;
            ViewModelLocator.Instance.AudioDevicesViewModel.DefaultMasterVolumeChanged += SelectIconFromMasterVolume;
        }

        private void InitializeIcos()
        {
            var soundMuteIcoStream =
                Application.GetResourceStream(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_mute.ico"));
            var soundOffIcoStream =
                Application.GetResourceStream(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_off.ico"));
            var soundLowIcoStream =
                Application.GetResourceStream(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_low.ico"));
            var soundMediumIcoStream =
                Application.GetResourceStream(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_medium.ico"));
            var soundHighIcoStream =
                Application.GetResourceStream(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_high.ico"));

            if (soundMuteIcoStream != null) _icos[0] = new Icon(soundMuteIcoStream.Stream);
            if (soundOffIcoStream != null) _icos[1] = new Icon(soundOffIcoStream.Stream);
            if (soundLowIcoStream != null) _icos[2] = new Icon(soundLowIcoStream.Stream);
            if (soundMediumIcoStream != null) _icos[3] = new Icon(soundMediumIcoStream.Stream);
            if (soundHighIcoStream != null) _icos[4] = new Icon(soundHighIcoStream.Stream);
        }

        private void SelectIconFromMasterVolume(object sender, VolumeChangedEventArgs volumeChangedEventArgs)
        {
            Thread thread;
            var audioDeviceObject = sender as IAudioDeviceObject;
            if (audioDeviceObject != null && audioDeviceObject.Muted) thread = new Thread(() => SetTrayIconIco(IconIco.SoundMute));
            else if (volumeChangedEventArgs.NewVolume >= 66) thread = new Thread(() => SetTrayIconIco(IconIco.SoundHigh));
            else if (volumeChangedEventArgs.NewVolume >= 33) thread = new Thread(() => SetTrayIconIco(IconIco.SoundMedium));
            else if (volumeChangedEventArgs.NewVolume >= 1) thread = new Thread(() => SetTrayIconIco(IconIco.SoundLow));
            else thread = new Thread(() => SetTrayIconIco());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void SetTrayIconIco(IconIco ico = IconIco.SoundOff)
        {
            _notifyIcon.Icon = _icos[(int)ico];
        }

        private void InstanceOnShowTrayWindowDemanded(object sender, EventArgs eventArgs)
        {
            TrayWindow.Show();
            _trayWindow.Closed += delegate { _trayWindow = null; };
        }

        private void InstanceOnCloseTrayWindowDemanded(object sender, EventArgs eventArgs)
        {
            TrayWindow.Close();
        }
    }
}