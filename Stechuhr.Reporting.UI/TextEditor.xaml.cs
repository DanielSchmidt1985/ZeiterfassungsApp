using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Stechuhr.Reporting.UI
{
    /// <summary>
    /// Interaktionslogik für TextEditor.xaml
    /// </summary>
    public partial class TextEditor : Window
    {
        private string initialText;

        public TextEditor()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => txt.Text;
            set 
            {
                initialText = value;
                txt.Text = value; 
            }
        }

        private void cmdAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            txt.Text = initialText;
            Close();
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
