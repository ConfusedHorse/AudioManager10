using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using EndPointControllerWrapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudioWrapper.Extentions;
using NAudioWrapper.Helper;
using NAudioWrapper.Interface;

namespace NAudioWrapper.Model
{
    internal class AudioDeviceObject : ViewModelBase, IAudioDeviceObject
    {
        #region Fields

        private MMDevice _actualDevice;
        private bool _muted;
        private int _percentualLeftPeak;
        private int _percentualRightPeak;
        private int _percentualLeftPeakDelay;
        private int _percentualRightPeakDelay;
        private readonly DispatcherTimer _peakTimer = new DispatcherTimer();

        #endregion Fields

        internal AudioDeviceObject(MMDevice audioDevice, DataFlow flow, Role role = Role.Multimedia)
        {
            _actualDevice = audioDevice;
            Flow = flow;
            Role = role;
            try
            {
                Init();
            }
            catch (Exception)
            {
                Debug.WriteLine("Initialize AudioDeviceObject failed. Could not fetch audio device data.");
            }
        }

        #region PrivateMethods

        private void Init()
        {
            Muted = _actualDevice.AudioEndpointVolume.Mute;

            _actualDevice.AudioEndpointVolume.OnVolumeNotification += AudioEndpointVolume_OnVolumeNotification;

            InitCommands();

            _peakTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); //meh
            _peakTimer.Tick += PeakTimerOnTick;
            _peakTimer.Start();
        }

        private void PeakTimerOnTick(object sender, EventArgs eventArgs)
        {
            _peakTimer.Stop();
            _peakTimer.Start();

            var leftPeak = _actualDevice.AudioMeterInformation.PeakValues[0] * 100;
            var rightPeak = 0d;
            if (_actualDevice.AudioMeterInformation.PeakValues.Count > 1)
                rightPeak = _actualDevice.AudioMeterInformation.PeakValues[1] * 100;

            if (leftPeak < _percentualLeftPeakDelay) PercentualLeftPeakDelay -= 1;
            else PercentualLeftPeakDelay = (int)leftPeak;
            if (rightPeak < _percentualRightPeakDelay) PercentualRightPeakDelay -= 1;
            else PercentualRightPeakDelay = (int)rightPeak;

            PercentualLeftPeak = (int)leftPeak;
            PercentualRightPeak = (int)rightPeak;
        }

        private void SetAsDefaultOutputMultimediaAudioDevice()
        {
            var actualEnumerator =
                InvokeEndPointController.GetDevices().FirstOrDefault(i => i.Item2 == _actualDevice.FriendlyName);
            if (actualEnumerator == null) return;
            var deviceEnumerator = actualEnumerator.Item1.ToString();
            AudioAccessHelper.SetDefaultDevice(deviceEnumerator);
        }

        #endregion PrivateMethods

        #region PublicMethods

        public bool StartRecording()
        {
            try
            {
                var loopback = _actualDevice.GetLoopback();
                loopback.DataAvailable += WaveInOnDataAvailableIntern;
                loopback.StartRecording();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool StopRecording()
        {
            try
            {
                var loopback = _actualDevice.GetLoopback();
                loopback.DataAvailable -= WaveInOnDataAvailableIntern;
                loopback.StopRecording();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion PublicMethods

        #region Properties

        public MMDevice ActualDevice
        {
            get { return _actualDevice; }
            set
            {
                _actualDevice = value;
                RaisePropertyChanged(() => ActualDevice);
            }
        }

        public bool Muted
        {
            get { return _muted; }
            set
            {
                _muted = value;
                _actualDevice.AudioEndpointVolume.Mute = Muted;
                RaisePropertyChanged(() => Muted);
            }
        }

        public DataFlow Flow { get; }
        public Role Role { get; }

        public bool IsDefault => _actualDevice.IsDefault(Flow, Role);

        public int PercentualLeftPeak
        {
            get { return _percentualLeftPeak; }
            set
            {
                _percentualLeftPeak = value;
                RaisePropertyChanged(() => PercentualLeftPeak);
            }
        }

        public int PercentualLeftPeakDelay
        {
            get { return _percentualLeftPeakDelay; }
            set
            {
                _percentualLeftPeakDelay = value;
                RaisePropertyChanged(() => PercentualLeftPeakDelay);
            }
        }

        public int PercentualRightPeak
        {
            get { return _percentualRightPeak; }
            set
            {
                _percentualRightPeak = value;
                RaisePropertyChanged(() => PercentualRightPeak);
            }
        }

        public int PercentualRightPeakDelay
        {
            get { return _percentualRightPeakDelay; }
            set
            {
                _percentualRightPeakDelay = value;
                RaisePropertyChanged(() => PercentualRightPeakDelay);
            }
        }

        #endregion Properties

        #region EventHandler

        public event WaveDataEventHandler WaveInOnDataAvailable;
        public event VolumeChangedEventHandler DefaultMasterVolumeChanged;

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            RaisePropertyChanged(() => ActualDevice);

            if (Flow != DataFlow.Render || Role != Role.Multimedia || !IsDefault) return;
            var args = new VolumeChangedEventArgs()
            {
                FriendlyName = _actualDevice.FriendlyName,
                NewVolume = (float)Math.Round(data.MasterVolume * 100)
            };
            var handler = DefaultMasterVolumeChanged;
            handler?.Invoke(this, args);
        }

        private void WaveInOnDataAvailableIntern(object sender, WaveInEventArgs args)
        {
            var handler = WaveInOnDataAvailable;
            handler?.Invoke(this, args);
        }

        #endregion EventHandler

        #region Commands

        public ICommand InvertMuteCommand { get; internal set; }
        public ICommand SetAsDefaultOutputDeviceCommand { get; internal set; }

        private void InitCommands()
        {
            InvertMuteCommand = new RelayCommand<object>(InvertMuteCommandExcecute, InvertMuteCommandCanExcecute);
            SetAsDefaultOutputDeviceCommand = new RelayCommand<object>(SetAsDefaultOutputMultimediaDeviceCommandExcecute,
                SetAsDefaultDeviceCommandCanExcecute);
        }

        private void InvertMuteCommandExcecute(object o)
        {
            Muted = !_muted;
        }

        private void SetAsDefaultOutputMultimediaDeviceCommandExcecute(object o)
        {
            SetAsDefaultOutputMultimediaAudioDevice();
        }

        private static bool InvertMuteCommandCanExcecute(object o)
        {
            return true;
        }

        private bool SetAsDefaultDeviceCommandCanExcecute(object o)
        {
            return AudioAccessHelper.GetDefaultOutputDevice(Role.Multimedia).DeviceFriendlyName !=
                   _actualDevice.DeviceFriendlyName;
        }

        #endregion Commands
    }
}