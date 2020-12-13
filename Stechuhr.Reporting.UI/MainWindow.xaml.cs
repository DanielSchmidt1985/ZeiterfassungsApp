using Newtonsoft.Json;
using Stechuhr.Settings;
using Stechuhr.Views;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Xps;

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

        private DayView SelectedDay = null;

        public MainWindow()
        {
            InitializeComponent();

            wtProvider = new WorktimeProvider();
            dvProvider = new DayViewProvider(wtProvider, new WorktimeSettings());

            MonthPicker.SelectedDate = DateTime.Today;
            MonthPicker.SelectedDatesChanged += MonthPicker_SelectedDatesChanged;
            MonthPicker.DisplayDateChanged += (s, e) => MonthPicker.SelectedDate = e.AddedDate;

            dgDayView.SelectionChanged += DgDayView_SelectionChanged;

            MonthPicker_SelectedDatesChanged(null, null);
        }

        private void DgDayView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDay = null;
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is DayView)
            {
                SelectedDay = e.AddedItems[0] as DayView;
            }
        }

        private void MonthPicker_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Days = dvProvider.CreateMonthView(MonthPicker.SelectedDate.Value);
            Days.ForEach(t => t.PropertyChanged += (s, e) => Update());
            Update();
        }

        private void Update()
        {
            TimeSpan otMonth = dvProvider.GetOvertime(Days);
            TimeSpan otTotal = dvProvider.GetOvertime(dvProvider.CreateOverallView());

            lblOtMonth.Content = otMonth.Format();
            lblOtOverall.Content = otTotal.Format();

            dgDayView.ItemsSource = null;
            dgDayView.ItemsSource = Days;
        }

        private void mnuEditDay_Clicked(object sender, RoutedEventArgs e)
        {
            if (SelectedDay == null) return;
            WorktimeItem nItem = new WorktimeItem();
            nItem.StartTime = DateTime.Parse(SelectedDay.Date.ToString());
            nItem.EndTime = nItem.StartTime;
            WorktimeItem wtItem = SelectedDay.wtItem == null ? nItem : SelectedDay.wtItem;
            JsonSerializerSettings jsonSerializerOptions = new JsonSerializerSettings() { Formatting = Formatting.Indented };
            string Data = JsonConvert.SerializeObject(wtItem, jsonSerializerOptions);
            TextEditor txtEditor = new TextEditor();
            txtEditor.Text = Data;
            txtEditor.ShowDialog();
            Data = txtEditor.Text;
            try
            {
                int index = wtProvider.Worktimes.FindIndex(t => t.id == wtItem.id);
                wtItem = JsonConvert.DeserializeObject<WorktimeItem>(Data);
                if (index == -1)
                {
                    wtProvider.Worktimes.Add(wtItem);
                }
                else
                {
                    wtProvider.Worktimes[index] = wtItem;
                }
                wtProvider.SaveWorktimeData();
                SelectedDay.FromWorktimeItem(wtItem);
                SelectedDay.NotifyPropertyChanged();
            }
            catch (Exception)
            {
                MessageBox.Show("Der Text konnte nicht Deserialisiert werden.");
                throw;
            }
        }
    }
}
