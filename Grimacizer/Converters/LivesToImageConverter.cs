using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Grimacizer7.Converters
{
    public class LivesToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var lives = (int)value;

            if (lives == 0)
                return new BitmapImage(new Uri("/Assets/Application/heart_broken.png", UriKind.RelativeOrAbsolute));

            return new BitmapImage(new Uri("/Assets/Application/heart.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
