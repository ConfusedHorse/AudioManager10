using System;
using System.Windows.Data;

namespace AudioManager10.View.Resource.Converter.Converters
{
    public class PercentualValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var percentage = (int)values[0];
                var value = (double)values[1];
                var aditionalPartition = double.Parse(parameter.ToString());
                var result = value * percentage / 100 / aditionalPartition;
                return result < 0 ? 0 : result;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}