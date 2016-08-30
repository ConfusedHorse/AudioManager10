using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using AudioManager10.View.Resource.Enum;

namespace AudioManager10.View.Resource.Converter.Converters
{
    public class VolumeImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var muted = (bool)values[0];
            var value = (float)values[1] * 100;
            if (muted) return FromIcoToBitmapImage(IconIco.SoundMute);
            if (value >= 66) return FromIcoToBitmapImage(IconIco.SoundHigh);
            if (value >= 33) return FromIcoToBitmapImage(IconIco.SoundMedium);
            if (value >= 1) return FromIcoToBitmapImage(IconIco.SoundLow);
            return FromIcoToBitmapImage(IconIco.SoundOff);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage FromIcoToBitmapImage(IconIco iconIco)
        {
            return IcoImageSource.Where(ico => ico.Key == iconIco)
                .Select(i => i.Value)
                .FirstOrDefault();
        }

        private static readonly Dictionary<IconIco, BitmapImage> IcoImageSource = new Dictionary<IconIco, BitmapImage>
        {
            {
                IconIco.SoundMute,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_mute.png"))
            },
            {
                IconIco.SoundOff,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_off.png"))
            },
            {
                IconIco.SoundLow,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_low.png"))
            },
            {
                IconIco.SoundMedium,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_medium.png"))
            },
            {
                IconIco.SoundHigh,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/sound_high.png"))
            }
        };
    }
}