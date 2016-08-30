﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using NAudio.CoreAudioApi;
using NAudioWrapper.Extentions;
using NAudioWrapper.Helper;
using NAudioWrapper.Interface;

namespace AudioManager10.ViewModel.ViewModel
{
    public class AudioDevicesViewModel : ViewModelBase
    {
        #region Fields

        private bool _isBusy;
        private ObservableCollection<IAudioDeviceObject> _activeOutputDeviceList;
        private ObservableCollection<IAudioDeviceObject> _activeInputDeviceList;
        private readonly AudioEventHelper _audioEventHelper;
        private DispatcherTimer _delayMasterVolumeEventTimer;
        private IAudioDeviceObject _defaultMultimediaRenderDevice;

        #endregion Fields

        public AudioDevicesViewModel(AudioEventHelper audioEventHelper)
        {
            _audioEventHelper = audioEventHelper;
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

        public ObservableCollection<IAudioDeviceObject> ActiveOutputDeviceList
        {
            get { return _activeOutputDeviceList; }
            set
            {
                _activeOutputDeviceList = value;
                RaisePropertyChanged(() => ActiveOutputDeviceList);
            }
        }

        public ObservableCollection<IAudioDeviceObject> ActiveInputDeviceList
        {
            get { return _activeInputDeviceList; }
            set
            {
                _activeInputDeviceList = value;
                RaisePropertyChanged(() => ActiveOutputDeviceList);
            }
        }

        public IAudioDeviceObject DefaultMultimediaRenderDevice
        {
            get { return _defaultMultimediaRenderDevice; }
            set
            {
                _defaultMultimediaRenderDevice = value;
                RaisePropertyChanged(() => DefaultMultimediaRenderDevice);
            }
        }

        #endregion Properties

        #region Private Methods

        private void Init()
        {
            InitCommands();
            InitEvents();
            RefreshOutputDevicesCommand.Execute(null);
            RefreshInputDevicesCommand.Execute(null);
        }

        private void InitEvents()
        {
            _audioEventHelper.DeviceStateChanged += AudioEventHelperOnDeviceStateChanged;
            _audioEventHelper.DeviceAdded += AudioEventHelperOnDeviceAdded;
            _audioEventHelper.DeviceRemoved += AudioEventHelperOnDeviceRemoved;
            _audioEventHelper.DefaultDeviceChanged += AudioEventHelperOnDefaultDeviceChanged;
            _audioEventHelper.PropertyValueChanged += AudioEventHelperOnPropertyValueChanged;

            _delayMasterVolumeEventTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 666)};
            _delayMasterVolumeEventTimer.Tick += DelayMasterVolumeEventTimerOnTick;
        }

        private void DelayMasterVolumeEventTimerOnTick(object sender, EventArgs eventArgs)
        {
            _delayMasterVolumeEventTimer.Stop();
            var tag = _delayMasterVolumeEventTimer.Tag as Tuple<object, VolumeChangedEventArgs>;
            var handler = DefaultMasterVolumeChanged;
            if (tag != null) handler?.Invoke(tag.Item1, tag.Item2);
            _delayMasterVolumeEventTimer.Tag = null;
        }

        private static void AudioEventHelperOnDeviceStateChanged(object sender, DeviceStateChangedEventArgs args)
        {
            Debug.WriteLine("DeviceState changed: " + args.DeviceId.ToFriendlyName() + " (" + args.NewState + ")");
        }

        private void AudioEventHelperOnDeviceRemoved(object sender, DeviceRemovedEventArgs args)
        {
            var oldDevice =
                _activeOutputDeviceList.FirstOrDefault(d => d.ActualDevice.ID == args.DeviceId);

            RefreshOutputDevicesCommand.Execute(null);

            Debug.WriteLine("Device removed: " + oldDevice);
        }

        private void AudioEventHelperOnDeviceAdded(object sender, DeviceAddedEventArgs args)
        {
            RefreshOutputDevicesCommand.Execute(null);

            var newDevice =
                _activeOutputDeviceList.FirstOrDefault(d => d.ActualDevice.ID == args.DeviceFriendlyName);

            Debug.WriteLine("Device added: " + newDevice);
        }

        private static void AudioEventHelperOnDefaultDeviceChanged(object sender, DefaultDeviceChangedEventArgs args)
        {
            Debug.WriteLine("DefaultDevice changed: " + args.DeviceId.ToFriendlyName() + " (" + args.Flow + ": " + args.Role + ")");
        }

        private static void AudioEventHelperOnPropertyValueChanged(object sender, PropertyValueChangedEventArgs args)
        {
            Debug.WriteLine("PropertyValue changed: " + args.DeviceFriendlyName + " (" + args.Key.formatId + ": " + args.Key.propertyId + ")");
        }
        
        private void AudioDeviceObjectOnVolumeChanged(object sender, VolumeChangedEventArgs args)
        {
            _delayMasterVolumeEventTimer.Stop();
            _delayMasterVolumeEventTimer.Tag = new Tuple<object, VolumeChangedEventArgs>(sender, args);
            _delayMasterVolumeEventTimer.Start();
        }

        #endregion Private Methods

        #region Events

        public event VolumeChangedEventHandler DefaultMasterVolumeChanged;

        #endregion Events

        #region Commands

        public ICommand RefreshOutputDevicesCommand { get; internal set; }
        public ICommand RefreshInputDevicesCommand { get; internal set; }

        private void InitCommands()
        {
            RefreshOutputDevicesCommand = new RelayCommand<object>(RefreshOutputDevicesCommandExcecute, RefreshOutputDevicesCommandCanExcecute);
            RefreshInputDevicesCommand = new RelayCommand<object>(RefreshInputDevicesCommandExcecute, RefreshInputDevicesCommandCanExcecute);
        }

        private void RefreshOutputDevicesCommandExcecute(object o)
        {
            IsBusy = true;

            ActiveOutputDeviceList = AudioAccessHelper.GetActiveOutoutDevices().ToViewModels(DataFlow.Render);

            foreach (var audioDeviceObject in ActiveOutputDeviceList)
            {
                audioDeviceObject.DefaultMasterVolumeChanged += AudioDeviceObjectOnVolumeChanged;
            }

            var outputDevice = AudioAccessHelper.GetDefaultOutputDevice(Role.Multimedia);
            DefaultMultimediaRenderDevice = ActiveOutputDeviceList.FirstOrDefault(od => od.ActualDevice.FriendlyName == outputDevice.FriendlyName);

            IsBusy = false;
        }

        private void RefreshInputDevicesCommandExcecute(object o)
        {
            IsBusy = true;
            
            ActiveInputDeviceList = AudioAccessHelper.GetActiveInputDevices().ToViewModels(DataFlow.Capture);

            IsBusy = false;
        }

        private bool RefreshOutputDevicesCommandCanExcecute(object o)
        {
            return !_isBusy;
        }

        private bool RefreshInputDevicesCommandCanExcecute(object o)
        {
            return !_isBusy;
        }

        #endregion

    }
}
