using Microsoft.Win32;
using Proyecto_Diseño.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
///
namespace Proyecto_Diseño
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Script script;
        Project project;
        ObservableCollection<Script> scripts = new ObservableCollection<Script>();
        TerminalManager TextBoxManager = new TerminalManager();
        bool changed = false;
        LoginWindow LoginW;
        Mistareas CursosW;
        ProjectFiles ProjectTree;
        String copybuffer = "";
        

        public MainWindow()
        {
            InitializeComponent();
            TextBoxManager.PythonOutput += procesoutputevent;
            TextBoxManager.PythonErrorOutput += TextBoxManager_PythonErrorOutput;
            script = new Script("");
            scripts.Add(script);
            Scriptstab.ItemsSource = scripts;
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
                //Checking if the script is already in the IDE
                bool scriptopen = false;
                foreach (var Existingscript in scripts) {
                    if (Existingscript.GetPath().Equals(prueba.GetPath()))
                    {
                        scriptopen = true;
                        script = Existingscript;      
                        break;
                    }
                }
                if (!scriptopen)
                {
                    script = new Script(file.FileName);
                    scripts.Add(script);
                }
                //////////////////////
                //process
                IDE.Document.Blocks.Clear();
                IDE.IsUndoEnabled = false;
                IDE.AppendText(script.GetCurrentFileContent());
                IDE.IsUndoEnabled = true;
                Scriptstab.SelectedItem = script;
                return;
            }
            return;
        }
        //Login Button
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (LoginW == null)
            {
                LoginW = new LoginWindow();
                LoginW.Show();
            }
            else 
            {
                if (!LoginW.IsActive)
                {
                    LoginW.Show();
                }
            }
        }

     

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange textstring = new TextRange(IDE.Document.ContentStart, IDE.Document.ContentEnd);
            if (script.GetPath().Equals(""))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
                bool? success = saveFileDialog1.ShowDialog();
                if (success != true) return;
                script.SetPath(saveFileDialog1.FileName);
                if (project != null)
                {
                    Project.Items.Clear();
                    ProjectFiles ProjectTree = project.AllProjectFiles(project.projpath());
                    Project.Items.Add(BuildProjectTree(ProjectTree));
                }
                Scriptstab.SelectedItem = script;
            }
            SignedScript script2 = new SignedScript(this.script);
            script2.SignScript();
            script2.SaveContent(textstring.Text);
        }

            
        private void IDEKey(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == System.Windows.Input.Key.C)
            {
                copybuffer = IDE.Selection.Text;
                e.Handled = true;
                return;
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == System.Windows.Input.Key.V)
            { 
                e.Handled = true;
                IDE.CaretPosition.InsertTextInRun(copybuffer);
                return;
            }
        }


        private void RunScript(object sender, RoutedEventArgs e)
        {
            if (TextBoxManager.ProcessRunning()) TextBoxManager.StopProcess();
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
                //Process
                using (File.Create(saveFileDialog1.FileName)){}
                script = new Script(saveFileDialog1.FileName);
                scripts.Add(script);
                Scriptstab.SelectedItem = script;
                SignedScript script2 = new SignedScript(this.script);
                script2.SignScript();

                if (project != null)
                {
                    Project.Items.Clear();
                    ProjectFiles ProjectTree = project.AllProjectFiles(project.projpath());
                    Project.Items.Add(BuildProjectTree(ProjectTree));
                }
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
                if (CursosW == null)
                {
                    CursosW = new Mistareas();
                    CursosW.Show();
                }
                else
                {
                    if (!CursosW.IsActive)
                    {
                        CursosW.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("Sin sesión iniciada");
            }
        }
        private void ApplyColor()
        {
            TextRange textstring = new TextRange(IDE.Document.ContentStart, IDE.Document.ContentEnd);
            this.script.SetContent(textstring.Text);
            ScriptFormat script = new ScriptFormat(this.script);
            string TF = script.GetFormattedText();
            Paragraph Coloredtext = new Paragraph();
            IDE.IsUndoEnabled = false;
            IDE.Document.Blocks.Clear();      
            string pattern = @"\[(/)?([BYPG])\]";
            int pos = 0;
            Brush StringColor;
            string color;
            var BrushesStack = new Stack<Brush>();
            BrushesStack.Push(Brushes.Black);
            foreach (Match match in Regex.Matches(TF, pattern, RegexOptions.Singleline))
            {
                if (match.Index > pos)
                { 
                    Coloredtext.Inlines.Add(new Run(TF.Substring(pos, match.Index - pos)) {Foreground = BrushesStack.Peek()});
                }
                bool colortag = match.Groups[1].Success;
                if (!colortag)
                {
                    color = match.Groups[2].Value;
                    switch (color)
                    {
                        case "B": StringColor = Brushes.Blue; break;
                        case "Y": StringColor = Brushes.YellowGreen; break;
                        case "P": StringColor = Brushes.DeepPink; break;
                        case "G": StringColor = Brushes.Green; break;
                        default: StringColor = Brushes.Black; break;
                    }
                    BrushesStack.Push(StringColor);
                }
                else
                {
                    if (BrushesStack.Count > 1)
                    {
                        BrushesStack.Pop();
                    }
                }
                pos = match.Index + match.Length;
            }
            if (pos < TF.Length)
            {
                Coloredtext.Inlines.Add(new Run(TF.Substring(pos)) { Foreground = BrushesStack.Peek()});
            }
            IDE.Document.Blocks.Add(Coloredtext);
            IDE.IsUndoEnabled = true;
        }

        private void IDETextChange(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            if (changed) return;
            int caret = new TextRange(IDE.Document.ContentStart, IDE.CaretPosition).Text.Length;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                changed = true;
                ApplyColor();
                changed = false;
                int pos = 0;
                Run PosCaret = null;
                foreach (var block in IDE.Document.Blocks)
                {
                    if (!(block is Paragraph paragraph))
                        continue;
                    foreach (var inline in paragraph.Inlines)
                    {
                        {
                            if (!(inline is Run run))
                                continue;
                            if (PosCaret == null && caret > 0)
                            {
                                pos += run.Text.Length;
                                if (pos >= caret)
                                {
                                    PosCaret = run;
                                    break;
                                }
                            }
                        }
                        if (PosCaret != null)
                            break;
                    }
                }
                if (PosCaret != null)
                    IDE.CaretPosition = PosCaret.ContentEnd.GetPositionAtOffset(caret - pos, LogicalDirection.Forward);
            }));
        }

        private void ScriptTabChange(object sender, SelectionChangedEventArgs e)
        {

            TabControl TC = (TabControl)sender;
            Script newscript = TC.SelectedContent as Script;
            this.script = newscript;
            IDE.Document.Blocks.Clear();
            IDE.IsUndoEnabled = false;
            IDE.AppendText(script.GetCurrentFileContent());
            IDE.IsUndoEnabled = true;
        }

        private void NewProject(object sender, RoutedEventArgs e)
        {
            using (var newProject = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = newProject.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    project = new Project(newProject.SelectedPath);
                    ProjectFiles ProjectTree = project.AllProjectFiles(project.projpath());
                    Project.Items.Add(BuildProjectTree(ProjectTree));
                }
                else
                {
                    MessageBox.Show("Sin directorio especificado");
                }
            }
        }
        private TreeViewItem BuildProjectTree(ProjectFiles pf)
        {
            TreeViewItem Treeitem = new TreeViewItem { Header = pf.filename, Tag = pf};
            foreach (ProjectFiles pf2 in pf.folder)
            {
                Treeitem.Items.Add(BuildProjectTree(pf2));
            }
            return Treeitem;
        }

        //obtener el script actual 
        private void OpenProjectScript(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView TV = (TreeView)sender;
            if (TV.SelectedItem is TreeViewItem TVitem)
            {
                ProjectFiles file = TVitem.Tag as ProjectFiles;
                string fullpath = file.path + "\\" + file.filename;
                SignedScript prueba = new SignedScript(new Script(fullpath));
                if (!prueba.verificarfirma())
                {
                    MessageBox.Show("El archivo no esta firmado por el IDE");
                    return;
                }

                bool scriptopen = false;
                foreach (var Existingscript in scripts)
                {
                    if (Existingscript.GetPath().Equals(prueba.GetPath()))
                    {
                        scriptopen = true;
                        script = Existingscript;
                        break;
                    }
                }
                if (!scriptopen)
                {
                    script = new Script(fullpath);
                    scripts.Add(script);
                }
                IDE.Document.Blocks.Clear();
                IDE.IsUndoEnabled = false;
                IDE.AppendText(script.GetCurrentFileContent());
                IDE.IsUndoEnabled = true;
                Scriptstab.SelectedItem = script;
                return;
            }
        }
    }
   
}


