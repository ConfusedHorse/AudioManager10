using System.Collections.ObjectModel;
using System.Linq;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudioWrapper.Interface;
using NAudioWrapper.Model;

namespace NAudioWrapper.Extentions
{
    public static class NAudioExtentions
    {
        public static ObservableCollection<IAudioDeviceObject> ToViewModels(this MMDeviceCollection models, DataFlow flow, Role role = Role.Multimedia)
        {
            return new ObservableCollection<IAudioDeviceObject>(models.Select(model => new AudioDeviceObject(model, flow, role)));
        }

        public static WasapiLoopbackCapture GetLoopback(this MMDevice device)
        {
            return new WasapiLoopbackCapture(device);
        }
         
    }
}