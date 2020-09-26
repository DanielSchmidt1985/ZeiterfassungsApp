using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Stechuhr.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WorktimeProvider worktimeProvider { get; private set; }
        public DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            Closing += (s, e) =>
            {
                if (worktimeProvider.Status == WorktimeStatus.Working)
                {
                    Stempeln(btnStempeln, null);
                }
            };

            worktimeProvider = new WorktimeProvider();
            worktimeProvider.LoadWorktimeData();

            lblUhrzeit.Content = DateTime.Now.ToLongTimeString();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_RefreshTimes;
            timer.Start();

            RefreshLayout(btnStempeln, WorktimeStatus.NotWorking);
        }

        private void Timer_RefreshTimes(object sender, EventArgs e)
        {
            lblUhrzeit.Content = DateTime.Now.ToLongTimeString();

            DateTime startWorking = worktimeProvider.TodayWorktimeStart;
            lblStartWorking.Content = startWorking == DateTime.MinValue ? "" : startWorking.ToLongTimeString();

            lblEndWorking.Content = worktimeProvider.TodayWorktimeEnd.ToLongTimeString();

            lblWorkingTime.Content = worktimeProvider.TodayWorktimeSpan.ToString(@"hh\:mm\:ss");
            lblPauseTime.Content = worktimeProvider.TodayPauseSpan.ToString(@"hh\:mm\:ss");
        }

        public void InitializeWorktimeProvider(WorktimeProvider worktimeProvider)
        {
            this.worktimeProvider = worktimeProvider;
        }

        private void Stempeln(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshLayout(sender, worktimeProvider.Stamping());
            }
            catch (Exception)
            {
                lblStatus.Text = "Ungültige Operation";
            }
        }

        private void RefreshLayout(object sender, WorktimeStatus worktimeCommandResult)
        {
            if (worktimeCommandResult == WorktimeStatus.Working)
            {
                btnStempeln.Background = Brushes.Tomato;
                btnStempeln.Content = "Gehen";
                lblStatus.Text = "Arbeitet ...";
            }
            else
            {
                btnStempeln.Background = Brushes.LightGreen;
                btnStempeln.Content = "Kommen";
                lblStatus.Text = "Pausiert ...";
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
