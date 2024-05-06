using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Nelder_Mead_App.Converters;

public class VisibilityToTextConverter : IValueConverter
{
    public string VisibleValue { get; set; }
    public string HiddenValue { get; set; }
    public string CollapsedValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if(value is not null)
        { 
            var visibility = (Visibility)value;
            if(visibility == Visibility.Visible)
            {
                return VisibleValue;
            }
            if (visibility == Visibility.Hidden)
            {
                return HiddenValue;
            }
            if (visibility == Visibility.Collapsed)
            {
                return CollapsedValue;
            }
        }

        return CollapsedValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
