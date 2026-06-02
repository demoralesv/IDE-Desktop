using Microsoft.Win32;
using Proyecto_Diseño.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json;
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

///
namespace Proyecto_Diseño
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Script script;
        TerminalManager TextBoxManager = new TerminalManager();
        
        public MainWindow()
        {
            InitializeComponent();
            TextBoxManager.PythonOutput += procesoutputevent;
            TextBoxManager.PythonErrorOutput += TextBoxManager_PythonErrorOutput;
        }

        private void TextBoxManager_PythonErrorOutput(string obj)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                LogTerminal.Document.Blocks.Add(new Paragraph(new Run(obj)));
                LogTerminal.ScrollToEnd();
            }));
        }

        //Open Doc Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
            bool? success = file.ShowDialog();
            if (success == true)
            {
                SignedScript prueba = new SignedScript(new Script(file.FileName));
                if (!prueba.verificarfirma()) {
                    MessageBox.Show("El archivo no esta firmado por el IDE");
                    return;
                }
                script = new Script(file.FileName);
                IDE.Document.Blocks.Clear();
                IDE.IsUndoEnabled = false;
                IDE.AppendText(script.GetCurrentFileContent());
                IDE.IsUndoEnabled = true;
                if (TextBoxManager.ProcessRunning())
                {
                    TextBoxManager.StopProcess();
                }
                return;
            }
            return;
        }
        //Login Button
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {

            LoginWindow LoginW = new LoginWindow();
            LoginW.Show();
        }

     

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange textstring = new TextRange(IDE.Document.ContentStart, IDE.Document.ContentEnd);
            if (script == null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
                bool? success = saveFileDialog1.ShowDialog();
                if (success == true)
                {
                    File.WriteAllText(saveFileDialog1.FileName, textstring.Text);
                    script = new Script(saveFileDialog1.FileName);
                    SignedScript script1 = new SignedScript(this.script);
                    script1.SignScript();
                    return;
                }
                return;
            }
            SignedScript script2 = new SignedScript(this.script);
            script2.SignScript();
            script2.SaveContent(textstring.Text);
        }

            
        private void IDEKey(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && (e.Key == System.Windows.Input.Key.C || e.Key == System.Windows.Input.Key.V))
            {
                e.Handled = true;
            }
        }


        private void RunScript(object sender, RoutedEventArgs e)
        {
            if (TextBoxManager.ProcessRunning())return;
            TextBoxManager.PyCommand(script.GetPath(), false);
        }

        private void CmdKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                TextRange range = new TextRange(CommandTerminal.Document.ContentStart, CommandTerminal.Document.ContentEnd);
                string text = range.Text;
                if (TextBoxManager.ProcessRunning())
                {
                    text = text.Trim().Replace("\r", "");
                    TextBoxManager.InputUsr(text);
                    LogTerminal.Document.Blocks.Add(new Paragraph(new Run(">>>" + text)));
                    LogTerminal.ScrollToEnd();
                }
                else
                {
                    TextBoxManager.PyCommand(text, true);
                }
                CommandTerminal.Document.Blocks.Clear();
            }
        }

        private void GitWindow(object sender, RoutedEventArgs e)
        {
            GitWindow GitW = new GitWindow();
            GitW.Show();
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            TextRange textstring = new TextRange(IDE.Document.ContentStart, IDE.Document.ContentEnd);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
            bool? success = saveFileDialog1.ShowDialog();
            if (success == true)
            {
                IDE.Document.Blocks.Clear();
                if (TextBoxManager.ProcessRunning())
                {
                    TextBoxManager.StopProcess();
                }
                using (File.Create(saveFileDialog1.FileName)){}
                script = new Script(saveFileDialog1.FileName);
             }
        }

        private void procesoutputevent(string output)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                LogTerminal.Document.Blocks.Add(new Paragraph(new Run(output)));
                LogTerminal.ScrollToEnd();
            }));
        }


        private void TareaClick(object sender, RoutedEventArgs e)
        {
            if (ApiService.getInstance().tokeninit())
            {
                Mistareas CursosW = new Mistareas();
                CursosW.Show();
            }
            else
            {
                MessageBox.Show("Sin sesión iniciada");
            }
        }
    }
}


