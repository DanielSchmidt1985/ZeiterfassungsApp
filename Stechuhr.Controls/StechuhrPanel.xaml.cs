using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Stechuhr.Controls
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class StechuhrPanel : UserControl
    {
        public delegate void StechuhrPanelEventHandler(object sender, StechuhrPanelEventArgs args);
        public event StechuhrPanelEventHandler OnClose;

        public WorktimeProvider worktimeProvider { get; private set; }
        public DispatcherTimer timer = new DispatcherTimer();

        public StechuhrPanel()
        {
            InitializeComponent();

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
                lblStatus.Content = "Ungültige Operation";
            }
        }

        private void RefreshLayout(object sender, WorktimeStatus worktimeCommandResult)
        {
            if (worktimeCommandResult == WorktimeStatus.Working)
            {
                btnStempeln.Background = Brushes.Tomato;
                btnStempeln.Content = "Gehen";
                lblStatus.Content = "Arbeitet ...";
            }
            else
            {
                btnStempeln.Background = Brushes.LightGreen;
                btnStempeln.Content = "Kommen";
                lblStatus.Content = "Pausiert ...";
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (worktimeProvider.Status == WorktimeStatus.Working)
            {
                Stempeln(btnStempeln, null);
            }
            OnClose(this, new StechuhrPanelEventArgs());
        }
    }
}
