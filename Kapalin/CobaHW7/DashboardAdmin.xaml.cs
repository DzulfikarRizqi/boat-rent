using CobaHW7.Class;
using CobaHW7.ViewModels;
//using CobaHW7.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CobaHW7
{
    public partial class DashboardAdmin : Window
    {
        public DashboardAdmin()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Cek apakah mouse sedang berada di atas tabel atau tidak
            // Jika TIDAK (!), maka kita hapus seleksinya
            if (!BoatsDataGrid.IsMouseOver)
            {
                BoatsDataGrid.UnselectAll();
            }
        }
    }
}
