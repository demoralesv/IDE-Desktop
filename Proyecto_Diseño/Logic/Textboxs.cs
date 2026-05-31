using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Proyecto_Diseño.UI
{
    internal class TerminalManager
    {
        Process pyprogram;
        bool processactive = false;
        public event Action<string> PythonOutput;
        public event Action<string> PythonErrorOutput;
        StreamWriter UserInput;
        public TerminalManager() { }


        public void PyCommand(string program, bool command)
        {
            string exec;
            if (command) {
                exec = $"-c \"{program}\"";
            }
            else
            {
                if (String.IsNullOrEmpty(program))
                {
                    MessageBox.Show("Sin archivo abierto");
                    return;
                }
                exec = $"\"{program}\"";
            }
            pyprogram = new Process();
            pyprogram.StartInfo.FileName = "python";
            pyprogram.StartInfo.Arguments = $"{exec}";
            pyprogram.StartInfo.RedirectStandardOutput = true;
            pyprogram.StartInfo.RedirectStandardError = true;
            pyprogram.StartInfo.RedirectStandardInput = true;
            pyprogram.OutputDataReceived += new DataReceivedEventHandler(Pyprogram_OutputDataReceived);
            pyprogram.ErrorDataReceived += new DataReceivedEventHandler(Pyprogram_ErrorDataReceived);
            pyprogram.EnableRaisingEvents = true;
            pyprogram.Exited += ProcessEnd;
            pyprogram.StartInfo.UseShellExecute = false;
            pyprogram.StartInfo.CreateNoWindow = true;
            pyprogram.Start();
            processactive = true;
            UserInput = pyprogram.StandardInput;
            pyprogram.BeginOutputReadLine();
            pyprogram.BeginErrorReadLine();
        }

        private void ProcessEnd(object sender, EventArgs e)
        {
            UserInput.Dispose();
            pyprogram.Close();
            processactive = false;
        }
        public void StopProcess()
        {
            UserInput.Dispose();
            pyprogram.CancelOutputRead();
            pyprogram.CancelErrorRead();
            pyprogram.Kill();
            processactive = false;
        }
        private void Pyprogram_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }
            PythonOutput?.Invoke(e.Data);
        }
        private void Pyprogram_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }
            PythonOutput?.Invoke(e.Data);
        }
        public void InputUsr(string input)
        {
            pyprogram.StandardInput.Write(input + "\n");
        }
        public string Gitcommand(String command)
        {
            return command;
        }
        public bool ProcessRunning()
        {
            return processactive;
        }
    }
}
