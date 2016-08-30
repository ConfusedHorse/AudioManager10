using System;
using System.Windows.Data;

namespace AudioManager10.View.Resource.Converter.Converters
{
    public class DoublePercentageConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var value = (float) values[0];
                var result = (int) Math.Round(value*100);
                return result.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}