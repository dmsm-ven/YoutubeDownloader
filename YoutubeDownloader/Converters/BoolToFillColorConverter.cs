using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace YoutubeDownloader;

public class LoadStatusToFillColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is LoadStatus s)
        {
            switch (s)
            {
                case LoadStatus.None:
                    return Brushes.Transparent;

                case LoadStatus.Success: 
                    return Brushes.GreenYellow;

                case LoadStatus.InProgress:
                    return Brushes.Silver;

                case LoadStatus.HasErrors:
                    return Brushes.Red;
            }
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
