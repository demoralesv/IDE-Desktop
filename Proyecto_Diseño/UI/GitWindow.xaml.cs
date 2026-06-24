using Microsoft.Win32;
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
        GitController GitManager = new GitController();
        public GitWindow()
        {
            InitializeComponent();
        }



        private void ChangeDir(object sender, RoutedEventArgs e)
        {
            using (var newDir = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = newDir.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string gitresult = GitManager.setDirectory(newDir.SelectedPath);
                    LogGit.Document.Blocks.Add(new Paragraph(new Run(gitresult)));
                    LogGit.ScrollToEnd();

                }
                else
                {
                    MessageBox.Show("Sin directorio especificado");
                }
            }
        }

        private void CommitGit(object sender, RoutedEventArgs e)
        {
            
            string result = GitManager.commit(CommitMessage.Text);
            LogGit.Document.Blocks.Add(new Paragraph(new Run(result)));
            LogGit.ScrollToEnd();

        }

        private void Gitpull(object sender, RoutedEventArgs e)
        {
            string result = GitManager.pull();
            LogGit.Document.Blocks.Add(new Paragraph(new Run(result)));
            LogGit.ScrollToEnd();

        }

        private void CloreGit(object sender, RoutedEventArgs e)
        {
            string result = GitManager.clone(Repoclone.Text);
            LogGit.Document.Blocks.Add(new Paragraph(new Run(result)));
            LogGit.ScrollToEnd();
    

            result = GitManager.setorigin(Repoclone.Text);
            LogGit.Document.Blocks.Add(new Paragraph(new Run(result)));
            LogGit.ScrollToEnd();

        }

        private void pushgit(object sender, RoutedEventArgs e)
        {
            string result = GitManager.push();
            LogGit.Document.Blocks.Add(new Paragraph(new Run(result)));
            LogGit.ScrollToEnd();

        }
    }
}
