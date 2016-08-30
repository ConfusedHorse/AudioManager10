using System.Windows.Input;
using NAudio.CoreAudioApi;
using NAudioWrapper.Helper;
using NAudioWrapper.Model;

namespace NAudioWrapper.Interface
{
    public interface IAudioDeviceObject
    {
        event WaveDataEventHandler WaveInOnDataAvailable;
        event VolumeChangedEventHandler DefaultMasterVolumeChanged;

        bool StartRecording();
        bool StopRecording();

        DataFlow Flow { get; }
        Role Role { get; }
        bool IsDefault { get; }
        bool Muted { get; set; }
        int PercentualLeftPeak { get; set; }
        int PercentualRightPeak { get; set; }
        int PercentualLeftPeakDelay { get; set; }
        int PercentualRightPeakDelay { get; set; }
        MMDevice ActualDevice { get; }
        ICommand InvertMuteCommand { get; }
        ICommand SetAsDefaultOutputDeviceCommand { get; }
    }
}