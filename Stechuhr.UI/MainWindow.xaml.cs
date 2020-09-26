using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace Stechuhr.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        private bool AllowClosing = false;

        public WorktimeProvider worktimeProvider { get; private set; }
        public DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.MouseClick += (s, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Show();
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    ContextMenuStrip contextMenu = new ContextMenuStrip();
                    var mnuExit = contextMenu.Items.Add("Beenden");
                    contextMenu.Show(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
                    mnuExit.Click += (s, e) =>
                    {
                        notifyIcon.Dispose();
                        AllowClosing = true;
                        Close();
                    };
                }
            };
            notifyIcon.Icon = new System.Drawing.Icon("Stechuhr.ico");
            
            
            Closing += (s, e) =>
            {
                if (!AllowClosing)
                {
                    e.Cancel = true;
                    Hide();
                }
                else
                {
                    if (worktimeProvider.Status == WorktimeStatus.Working)
                    {
                        Stempeln(btnStempeln, null);
                    }
                }
            };

            Loaded += (s, e) =>
            {
                notifyIcon.Visible = true;
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

    }
}
