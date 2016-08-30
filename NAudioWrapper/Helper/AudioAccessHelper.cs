using System;
using System.Diagnostics;
using EndPointControllerWrapper;
using NAudio.CoreAudioApi;

namespace NAudioWrapper.Helper
{
    public static class AudioAccessHelper
    {
        #region public methods

        public static MMDeviceCollection GetActiveOutoutDevices()
        {
            return GetActiveDevices(DataFlow.Render);
        }

        public static MMDeviceCollection GetActiveInputDevices()
        {
            return GetActiveDevices(DataFlow.Capture);
        }

        public static MMDevice GetDefaultOutputDevice(Role role)
        {
            return GetDefaultDevice(DataFlow.Render, role);
        }

        public static MMDevice GetDefaultInputDevice(Role role)
        {
            return GetDefaultDevice(DataFlow.Capture, role);
        }

        public static bool IsDefault(this MMDevice device, DataFlow flow = DataFlow.Render, Role role = Role.Multimedia)
        {
            return GetDefaultDevice(flow, role).FriendlyName == device.FriendlyName; //cannot access id
        }

        public static bool SetDefaultDevice(string id)
        {
            if (GetDefaultOutputDevice(Role.Multimedia).ID == id) return false;
            InvokeEndPointController.SelectDevice(id);
            return true;
        }

        public static string ToFriendlyName(this string pwstrDeviceId)
        {
            try
            {
                var mmde = new MMDeviceEnumerator();
                var devCol = mmde.GetDevice(pwstrDeviceId);
                return devCol.DeviceFriendlyName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not enumerate devices due to an excepion: " + ex.Message);
                return null;
            }
        }

        private static MMDeviceCollection GetActiveDevices(DataFlow kind)
        {
            try
            {
                var mmde = new MMDeviceEnumerator();
                var devCol = mmde.EnumerateAudioEndPoints(kind, DeviceState.Active);
                return devCol;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not enumerate devices due to an excepion: " + ex.Message);
                return null;
            }
        }

        public static void SetGlobalVolume(int level)
        {
            try
            {
                var mmde = new MMDeviceEnumerator();
                var devCol = mmde.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All);
                foreach (var dev in devCol)
                {
                    try
                    {
                        if (dev.State == DeviceState.Active)
                        {
                            var newVolume = Math.Max(Math.Min(level, 100), 0) / (float)100;

                            dev.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume;

                            dev.AudioEndpointVolume.Mute = level == 0;

                            Debug.WriteLine("Volume of " + dev.FriendlyName + " is " + dev.AudioEndpointVolume.MasterVolumeLevelScalar);
                        }
                        else
                        {
                            Debug.WriteLine("Ignoring device " + dev.FriendlyName + " with state " + dev.State);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(dev.FriendlyName + " could not be muted with error " + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not enumerate devices due to an excepion: " + ex.Message);
            }
        }

        #endregion public methods

        #region private methods

        private static MMDevice GetDefaultDevice(DataFlow kind, Role role = Role.Multimedia)
        {
            try
            {
                var mmde = new MMDeviceEnumerator();
                var dev = mmde.GetDefaultAudioEndpoint(kind, role);
                return dev;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not aquire default device due to an excepion: " + ex.Message);
                return null;
            }
        }

        #endregion private methods
    }
}
