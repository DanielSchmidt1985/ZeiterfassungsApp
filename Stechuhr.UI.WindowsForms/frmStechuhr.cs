using Stechuhr.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stechuhr.UI.WindowsForms
{
    public partial class frmStechuhr : Form
    {
        private bool AllowClosing { get; set; }

        public WorktimeProvider WorktimeProvider { get; }

        public frmStechuhr(WorktimeProvider worktimeProvider)
        {
            InitializeComponent();
        
            WorktimeProvider = worktimeProvider;
            stechuhrPanel.InitializeWorktimeProvider(worktimeProvider);

            // Form Closing Event
            this.FormClosing += MainForm_FormClosing;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.AllowClosing && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

    }
}
