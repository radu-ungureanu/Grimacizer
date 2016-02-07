using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Grimacizer7.Converters
{
    public class StarsToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var stars = (int)value;
            var param = int.Parse(parameter as string);

            if (stars >= param)
                return new BitmapImage(new Uri("/Assets/Application/star.png", UriKind.RelativeOrAbsolute));

            return new BitmapImage(new Uri("/Assets/Application/star_bw.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
