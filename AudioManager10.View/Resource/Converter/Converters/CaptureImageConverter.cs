using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AudioManager10.View.Resource.Converter.Converters
{
    public class CaptureImageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var muted = (bool)values[0];
            return FromIcoToBitmapImage(!muted);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage FromIcoToBitmapImage(bool on)
        {
            return IcoImageSource.Where(mic => mic.Key == on)
                .Select(i => i.Value)
                .FirstOrDefault();
        }

        private static readonly Dictionary<bool, BitmapImage> IcoImageSource = new Dictionary<bool, BitmapImage>
        {
            {
                false,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/mic_off.png"))
            },
            {
                true,
                new BitmapImage(
                    new Uri("pack://application:,,,/AudioManager10.View;component/Resource/Icons/064/mic_on.png"))
            }
        };
    }
}