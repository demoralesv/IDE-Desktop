using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Diseño.UI
{
    internal class GitController
    {
        string CurrentDirectory;
        Process gitprocess;
        public GitController() { }

        public string setDirectory(string dir)
        {
            CurrentDirectory = dir;
            return GitCommand("init");
        }

        public string GitCommand(string command)
        {
            gitprocess = new Process();
            gitprocess.StartInfo.FileName = "git";
            gitprocess.StartInfo.Arguments = command;
            gitprocess.StartInfo.WorkingDirectory = CurrentDirectory;
            gitprocess.StartInfo.RedirectStandardOutput = true;
            gitprocess.StartInfo.RedirectStandardError = true;
            gitprocess.StartInfo.RedirectStandardInput = true;
            gitprocess.StartInfo.UseShellExecute = false;
            gitprocess.StartInfo.CreateNoWindow = true;

            gitprocess.Start();
            gitprocess.WaitForExit();
            if (gitprocess.ExitCode != 0)
            {
                return gitprocess.StandardError.ReadToEnd();
            }
            return gitprocess.StandardOutput.ReadToEnd();
        }
        public string commit(string message)
        {
            GitCommand("add .");
            return GitCommand($"commit -m \"{message}\"");
        } 

        public string pull()
        {
            return GitCommand("pull");
        }
        public string setorigin(string repo)
        {
            return GitCommand("remote add origin {repo}");
        }
        public string clone(string repo)
        {
            return GitCommand($"clone {repo}");
        }
        public string push()
        {
            return GitCommand($"push");
        }

    }
}
