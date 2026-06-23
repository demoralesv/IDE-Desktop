using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;


namespace Proyecto_Diseño.UI
{
    public class ProjectFiles
    {
        public string filename { get; set; }
        public string path { get; set; }
        public bool Directory {  get; set; }
        public List<ProjectFiles> folder { get; set; } = new List<ProjectFiles>();
    }

    internal class Project
    {
        List<string> scriptspath = new List<string>();
        string foldername;
        

        public Project(string path)
        {
            foldername = path;
        }
        public void addScript(string sc)
        {
            scriptspath.Add(sc);
        }
        //prueba
        public void Scriptslists()
        {
            foreach (string s in scriptspath)
            {
                MessageBox.Show(s);
            }
        }
        //Prueba
        public string projpath()
        {
            return foldername;
        }

        public ProjectFiles AllProjectFiles(string path)
        {

            ProjectFiles root = new ProjectFiles { path = path, filename = Path.GetFileName(path), Directory = true };
            foreach (string f in Directory.GetDirectories(path)){
                root.folder.Add(AllProjectFiles(f));
            }

            foreach (string f in Directory.GetFiles(path)) {
                root.folder.Add(new ProjectFiles {path = path, filename = Path.GetFileName(f), Directory = false });
            }
            return root;
        }
    }
}
