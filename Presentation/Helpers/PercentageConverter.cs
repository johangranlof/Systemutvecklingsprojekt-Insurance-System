using System.Globalization;
using System.Windows.Data;

namespace Presentation.Helpers
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return $"{intValue} %";

            }
            else if (value is string strValue)
            {
                if (int.TryParse(strValue.TrimEnd('%'), out int parsedValue))
                {
                    return $"{parsedValue} %";
                }
            }
            return "0 %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue && strValue.EndsWith("%") && int.TryParse(strValue.TrimEnd('%'), out int result))
            {
                return result;
            }
            return 0;
        }
    }

}
