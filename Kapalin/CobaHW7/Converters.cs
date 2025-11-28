using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CobaHW7
{
    /// <summary>
    /// Converter untuk mengubah null menjadi Collapsed dan non-null menjadi Visible
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter untuk mengubah boolean true menjadi Visible dan false menjadi Collapsed
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    /// <summary>
    /// Converter untuk menginversi boolean value (true menjadi false, false menjadi true)
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }
    }

    /// <summary>
    /// Converter untuk mengubah chance of rain (int) menjadi warna (Brush)
    /// 0-30% = green, 31-60% = orange, 61-100% = red
    /// </summary>
    public class ChanceOfRainToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int chanceOfRain)
            {
                if (chanceOfRain <= 30)
                    return new SolidColorBrush(Colors.Green);
                else if (chanceOfRain <= 60)
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // Orange
                else
                    return new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
