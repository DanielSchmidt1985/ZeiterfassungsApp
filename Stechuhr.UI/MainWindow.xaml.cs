using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Stechuhr.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum CloseReason
        {
            EndTask,
            Logoff,
            User
        }
        /// <summary>
        /// Must be reset to CloseReason.EndTask if closing is canceled
        /// </summary>
        private CloseReason _closeReason;

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
                if (_closeReason == CloseReason.Logoff || AllowClosing)
                {
                    if (worktimeProvider.Status == WorktimeStatus.Working)
                    {
                        Stempeln(btnStempeln, null);
                    }
                }
                else 
                {
                    e.Cancel = true;
                    _closeReason = CloseReason.EndTask;
                    Hide();
                }
            };

            Loaded += (s, e) =>
            {
                ((HwndSource)PresentationSource.FromDependencyObject(this)).AddHook(WndProc);
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

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x11:
                case 0x16:
                    _closeReason = CloseReason.Logoff;
                    break;

                case 0x112:
                    if (((ushort)wParam & 0xfff0) == 0xf060)
                        _closeReason = CloseReason.User;
                    break;

                    // CloseReason.EndTask gets a 0x10 windows message which is got by CloseReason.User too,
                    // so we have no way to identify it,
                    // except knowing that we did not got any of the specific messages of the other CloseReasons
            }
            return IntPtr.Zero;
        }
    }
}
