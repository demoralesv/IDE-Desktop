using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;


namespace Proyecto_Diseño.UI
{
    internal class Project
    {
        List<Script> scripts;
        string foldername;

        public Project()
        {
            
        }
        public void setDirectory(string path)
        {
            foldername = path;
        }
    }
}
