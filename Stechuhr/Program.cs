using System;
using System.Windows.Forms;
using System.Drawing;

namespace Stechuhr
{
    public class StechuhrApp
    {

        [STAThread]
        static void Main()
        {
            WorktimeProvider wtProvider = new WorktimeProvider();
            
            WorktimeItemCollection wtData = wtProvider.LoadWorktimeData();

            Application.Run(new MainForm());

            wtProvider.SaveWorktimeData(wtData);
        }

     

    }
}


//*****************************************************************************