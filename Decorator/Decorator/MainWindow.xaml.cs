using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Decorator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ScriptInt script;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadScript(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
            bool? success = file.ShowDialog();
            if (success == true)
            {
                script = new Script(file.FileName);
                Texto.Document.Blocks.Clear();
                Texto.IsUndoEnabled = false;
                Texto.AppendText(script.GetCurrentFileContent());
                Texto.IsUndoEnabled = true;
                return;
            }
            return;
        }

        private void Sign(object sender, RoutedEventArgs e)
        {
           SignedScript script = new SignedScript(this.script);
           script.SignScript();
           MessageBox.Show("Script Firmado");
        }

        private void CompareFirm(object sender, RoutedEventArgs e)
        {
            SignedScript script = new SignedScript(this.script);
            if (script.verificarfirma())
            {
                MessageBox.Show("Firmas iguales");
            }
            else{
                MessageBox.Show("Firmas diferentes");
            }
        }

        private void FormText(object sender, RoutedEventArgs e)
        {
            ScriptFormat script = new ScriptFormat(this.script);
            string TF = script.GetFormattedText();
            Texto.Document.Blocks.Clear();
            Texto.IsUndoEnabled = false;
            string pattern = @"(\[[YGPB]\]|\[/[YGBP]\])";
            string[] parts = Regex.Split(TF, pattern);
            Paragraph Newtext = new Paragraph();
            Brush color = Brushes.Black;
            for (int i = 0; i < parts.Length; i++) {
                string part = parts[i];
                if (part == "[B]")
                {
                    color = Brushes.Blue; continue;
                }
                if (part == "[Y]")
                {
                    color = Brushes.YellowGreen; continue;
                }
                if (part == "[P]")
                {
                    color = Brushes.DeepPink; continue;
                }
                if (part == "[G]")
                {
                    color = Brushes.Green; continue;
                }
                if (part == "[/Y]" || part == "[/B]" || part == "[/P]" || part == "[/G]")
                {
                    color = Brushes.Black; continue;
                }
                Newtext.Inlines.Add(new Run(part) { Foreground = color});
            }
            Texto.Document.Blocks.Add(Newtext);
            Texto.IsUndoEnabled = true;
        }
    }
}
