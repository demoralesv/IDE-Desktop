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
using System.Windows.Shapes;

namespace Proyecto_Diseño.UI
{
    /// <summary>
    /// Interaction logic for GitWindow.xaml
    /// </summary>
    public partial class GitWindow : Window
    {
        TerminalManager GitManager = new TerminalManager();
        public GitWindow()
        {
            InitializeComponent();
        }

        private void GitCMD(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextRange range = new TextRange(GitTerminal.Document.ContentStart, GitTerminal.Document.ContentEnd);
                string result = GitManager.Gitcommand(range.Text);
                LogGit.Document.Blocks.Add(new Paragraph(new Run(result)));
                LogGit.ScrollToEnd();
                GitTerminal.Document.Blocks.Clear();
            }
        }
    }
}
