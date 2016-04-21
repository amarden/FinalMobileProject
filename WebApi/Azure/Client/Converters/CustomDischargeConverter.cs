using Client.ClientObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Client
{
    public class CustomDischargeConverter : IValueConverter
    {
        public CustomDischargeConverter()
        {
        }

        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            var role = (string)value;
            if (role != "Physician")
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return null;
        }
    }
}
