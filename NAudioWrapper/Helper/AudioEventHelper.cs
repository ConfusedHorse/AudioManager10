﻿using System;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace NAudioWrapper.Helper
{
    #region Event Handlers

    public delegate void DeviceStateChangedEventHandler(object sender, DeviceStateChangedEventArgs e);
    public delegate void DeviceAddedEventHandler(object sender, DeviceAddedEventArgs e);
    public delegate void DeviceRemovedEventHandler(object sender, DeviceRemovedEventArgs e);
    public delegate void DefaultDeviceChangedEventHandler(object sender, DefaultDeviceChangedEventArgs e);
    public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);
    public delegate void VolumeChangedEventHandler(object sender, VolumeChangedEventArgs e);

    #endregion Event Handlers

    public class AudioEventHelper : IMMNotificationClient
    {
        private readonly MMDeviceEnumerator _realEnumerator;

        #region Event Handlers

        public event DeviceStateChangedEventHandler DeviceStateChanged;
        public event DeviceAddedEventHandler DeviceAdded;
        public event DeviceRemovedEventHandler DeviceRemoved;
        public event DefaultDeviceChangedEventHandler DefaultDeviceChanged;
        public event PropertyValueChangedEventHandler PropertyValueChanged;

        #endregion Event Handlers

        #region Constructor

        public AudioEventHelper()
        {
            _realEnumerator = new MMDeviceEnumerator();
            RegisterEndpointNotificationCallback(this);
        }

        ~AudioEventHelper()
        {
            UnRegisterEndpointNotificationCallback(this);
        }

        #endregion Constructor

        internal int RegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return _realEnumerator.RegisterEndpointNotificationCallback(client);
        }
        internal int UnRegisterEndpointNotificationCallback([In] [MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
        {
            return _realEnumerator.UnregisterEndpointNotificationCallback(client);
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            var args = new DeviceStateChangedEventArgs
            {
                DeviceId = deviceId,
                NewState = newState
            };
            DeviceStateChanged?.Invoke(this, args);
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            var args = new DeviceAddedEventArgs
            {
                DeviceFriendlyName = pwstrDeviceId.ToFriendlyName()
            };
            DeviceAdded?.Invoke(this, args);
        }

        public void OnDeviceRemoved(string deviceId)
        {
            var args = new DeviceRemovedEventArgs
            {
                DeviceId = deviceId.ToFriendlyName()
            };
            DeviceRemoved?.Invoke(this, args);
        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            var args = new DefaultDeviceChangedEventArgs
            {
                Flow = flow,
                Role = role,
                DeviceId = defaultDeviceId
            };
            DefaultDeviceChanged?.Invoke(this, args);
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            var args = new PropertyValueChangedEventArgs
            {
                DeviceFriendlyName = pwstrDeviceId.ToFriendlyName(),
                Key = key
            };
            PropertyValueChanged?.Invoke(this, args);
        }
    }

    public class DeviceStateChangedEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
        public DeviceState NewState { get; set; }
    }

    public class DeviceAddedEventArgs : EventArgs
    {
        public string DeviceFriendlyName { get; set; }
    }

    public class DeviceRemovedEventArgs : EventArgs
    {
        public string DeviceId { get; set; }
    }

    public class DefaultDeviceChangedEventArgs : EventArgs
    {
        public DataFlow Flow { get; set; }
        public Role Role { get; set; }
        public string DeviceId { get; set; }
    }

    public class PropertyValueChangedEventArgs : EventArgs
    {
        public string DeviceFriendlyName { get; set; }
        public PropertyKey Key { get; set; }
    }

    public class VolumeChangedEventArgs : EventArgs
    {
        public string FriendlyName { get; set; }
        public float NewVolume { get; set; }
    }
}