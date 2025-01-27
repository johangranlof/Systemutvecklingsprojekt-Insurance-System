using System.Globalization;
using System.Windows.Data;

namespace Presentation.Helpers
{
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? $"{value} %" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = string.Empty;

            if (value is string inputValue)
            {
                strValue = inputValue;

                if (strValue.EndsWith(" %"))
                {
                    strValue = strValue.Substring(0, strValue.Length - 2);
                }
            }

            double result;
            if (double.TryParse(strValue, out result))
            {
                return result;
            }
            return 0;
        }
    }
}
