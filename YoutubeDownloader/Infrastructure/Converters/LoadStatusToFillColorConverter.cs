using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace YoutubeDownloader;

public class LoadStatusToFillColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LoadStatus s)
        {
            switch (s)
            {
                case LoadStatus.None:
                    return Brushes.Transparent;

                case LoadStatus.Success:
                    return new SolidColorBrush(Color.FromRgb(80, 200, 120)); // light green 

                case LoadStatus.Downloading:
                    return Brushes.Silver;

                case LoadStatus.Converting:
                    return Brushes.Silver;

                case LoadStatus.HasErrors:
                    return new SolidColorBrush(Color.FromRgb(220, 20, 60)); // red (Crimson)
            }
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
