using CobaHW7.Services;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Globalization; 
using System.Threading;     

namespace CobaHW7
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // --- KODE BARU DIMULAI DI SINI ---

            // 1. Buat settingan bahasa Indonesia
            var culture = new CultureInfo("id-ID");

            // 2. Terapkan ke format angka & tanggal aplikasi
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // 3. Terapkan ke komponen WPF (XAML) agar Binding membacanya
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

            // --- KODE BARU SELESAI ---
        }
    }

}
