using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stechuhr.Reporting.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MonthPicker.DisplayModeChanged += MonthPicker_DisplayModeChanged;
            //MonthPicker.SelectedDate = DateTime.Today;
        }

        private void MonthPicker_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            MonthPicker.DisplayMode = CalendarMode.Month;
        }
    }
}
