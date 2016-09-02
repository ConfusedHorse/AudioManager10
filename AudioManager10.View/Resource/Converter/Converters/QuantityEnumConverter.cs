using System;
using System.Globalization;
using System.Windows.Data;
using AudioManager10.View.Properties;
using AudioManager10.View.Resource.Enum;

namespace AudioManager10.View.Resource.Converter.Converters
{
    public class QuantityEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int)(double)value;
            var enumValue = (Quantity)index;
            var result = Resources.ResourceManager.GetString(enumValue.ToString());
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}