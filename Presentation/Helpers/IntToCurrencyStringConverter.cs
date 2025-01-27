using System.Globalization;
using System.Windows.Data;


namespace Presentation.Helpers
{
    public class IntToCurrencyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int amount)
            {
                return $"{amount:N0} kr";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                strValue = strValue.Replace("kr", "").Trim().Replace(",", "");

                if (int.TryParse(strValue, NumberStyles.AllowThousands, culture, out int result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}


