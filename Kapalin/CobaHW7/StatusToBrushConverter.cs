using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CobaHW7
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (value?.ToString() ?? "").ToLowerInvariant();
            return s switch
            {
                "pending" => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB300")),
                "confirmed" => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E88E5")),
                "cancelled" => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828")),
                // dukung label Indonesia juga
                "dipesan" => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E88E5")),
                "selesai" => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32")),
                "dibatalkan" => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828")),
                _ => (Brush)new SolidColorBrush((Color)ColorConverter.ConvertFromString("#607D8B"))
            };
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
