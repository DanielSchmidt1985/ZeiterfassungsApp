using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stechuhr.UI.WindowsForms
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WorktimeProvider worktimeProvider = new WorktimeProvider();
            worktimeProvider.LoadWorktimeData(Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), "WorktimeData.json"));

            Application.Run(new frmStechuhr(worktimeProvider));

            worktimeProvider.SaveWorktimeData();
        }
    }
}
