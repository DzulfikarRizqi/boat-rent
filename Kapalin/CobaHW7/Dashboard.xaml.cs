using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CobaHW7.ViewModels;
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();

            // Hubungkan dengan ViewModel
            this.DataContext = new DashboardUserViewModel();
        }
    }
}
