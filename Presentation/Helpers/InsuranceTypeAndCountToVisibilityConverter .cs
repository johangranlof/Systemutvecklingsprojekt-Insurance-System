using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Presentation.Helpers
{
    public class InsuranceTypeAndCountToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string insuranceType && values[1] != null && parameter is string expectedInsuranceType)
            {
                bool hasItems = false;

                if (values[1] is bool boolValue)
                {
                    hasItems = boolValue;
                }
                else if (values[1] is int intValue)
                {
                    hasItems = intValue > 0;
                }
                else if (values[1] is Visibility visibilityValue)
                {
                    hasItems = visibilityValue == Visibility.Visible;
                }
                if (insuranceType == expectedInsuranceType && hasItems)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
