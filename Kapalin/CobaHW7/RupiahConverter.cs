using System;
using System.Globalization;
using System.Windows.Data;

namespace CobaHW7
{
    public class RupiahConverter : IValueConverter
    {
        private static readonly CultureInfo Id = new("id-ID");
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IFormattable f) return string.Format(Id, "Rp {0:N0}", f);
            return "Rp 0";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
