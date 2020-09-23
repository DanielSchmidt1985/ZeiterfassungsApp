using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stechuhr
{
    public partial class MainForm : Form
    {

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenu1;
        private ToolStripMenuItem menuItemBeenden;

        private bool AllowClosing { get; set; }

        public MainForm()
        {
            InitializeComponent();

            this.contextMenu1 = new ContextMenuStrip();
            this.menuItemBeenden = new ToolStripMenuItem();

            // Initialize contextMenu1
            this.contextMenu1.Items.AddRange(new ToolStripMenuItem[] { this.menuItemBeenden });

            // Initialize menuItem1
            this.menuItemBeenden.Text = "Beenden";
            this.menuItemBeenden.Click += new EventHandler(this.menuItemBeenden_Click);

            // Set up how the form should be displayed.
            this.ClientSize = new Size(292, 266);
            this.Text = "Notify Icon Example";

            // Create the NotifyIcon.
            this.notifyIcon1 = new NotifyIcon();

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon1.Icon = new Icon("appicon.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon1.ContextMenuStrip = this.contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "Form1 (NotifyIcon example)";
            notifyIcon1.Visible = true;

            // Handle the DoubleClick event to activate the form.
            notifyIcon1.MouseClick += NotifyIcon1_MouseClick;

            // Form Closing Event
            this.FormClosing += MainForm_FormClosing;
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.AllowClosing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void menuItemBeenden_Click(object Sender, EventArgs e)
        {
            this.AllowClosing = true;
            this.Close();
        }
    }
}
