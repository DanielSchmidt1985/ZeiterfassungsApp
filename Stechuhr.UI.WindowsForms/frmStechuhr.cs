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
        private bool AllowClosing { get; set; } = false;

        public WorktimeProvider WorktimeProvider { get; }

        public frmStechuhr(WorktimeProvider worktimeProvider)
        {
            InitializeComponent();

            this.MaximizeBox = false;

            WorktimeProvider = worktimeProvider;
            stechuhrPanel.InitializeWorktimeProvider(worktimeProvider);

            FormClosing += FrmStechuhr_FormClosing;

            // StechuhrPanel raised Closing
            this.stechuhrPanel.OnClose += StechuhrPanel_OnClose;
        }

        private void FrmStechuhr_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AllowClosing == false)
            {
                e.Cancel = true;
            }
        }

        private void StechuhrPanel_OnClose(object sender, StechuhrPanelEventArgs args)
        {
            AllowClosing = true;
            Close();
        }
    }
}
