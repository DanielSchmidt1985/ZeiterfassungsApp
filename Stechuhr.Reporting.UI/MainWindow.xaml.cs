using Stechuhr.Settings;
using Stechuhr.Views;
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
        private List<DayView> Days;

        private WorktimeProvider wtProvider;
        private DayViewProvider dvProvider;

        public MainWindow()
        {
            InitializeComponent();

            wtProvider = new WorktimeProvider();
            dvProvider = new DayViewProvider(wtProvider, new WorktimeSettings());

            MonthPicker.SelectedDate = DateTime.Today;
            MonthPicker.SelectedDatesChanged += UpdateDataGrid;
            MonthPicker.DisplayDateChanged += (s, e) => MonthPicker.SelectedDate = e.AddedDate; ;

            UpdateDataGrid(null, null);
        }

        private void UpdateDataGrid(object sender, SelectionChangedEventArgs e)
        {
            Days = dvProvider.CreateMonthView(MonthPicker.SelectedDate.Value);
            Days.ForEach(t => t.PropertyChanged += (s, e) =>
            {
                dgDayView.ItemsSource = null;
                dgDayView.ItemsSource = Days;
            });
            dgDayView.ItemsSource = Days;
        }

    }
}
