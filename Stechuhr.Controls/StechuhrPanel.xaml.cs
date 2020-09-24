using System;
using System.Collections.Generic;
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

namespace Stechuhr.Controls
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class StechuhrPanel : UserControl
    {
        public WorktimeProvider worktimeProvider { get; private set; }

        public StechuhrPanel()
        {
            InitializeComponent();
        }

        public void InitializeWorktimeProvider(WorktimeProvider worktimeProvider)
        {
            this.worktimeProvider = worktimeProvider;
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
           
        }
        private void Stempeln(object sender, RoutedEventArgs e)
        {

        }
    }
}
