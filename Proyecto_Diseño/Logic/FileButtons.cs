using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;


namespace Proyecto_Diseño
{
    //This class connects with the MainWindow view
    //It's in charge of managing all the work related to files, like saving, creating, writing and signing them
    //For executing files go to Textboxs.cs the Terminal Manager class
    internal class FileButtons
    {
        private OpenFileDialog file = new OpenFileDialog();
        private string path;
        private Dictionary<string, string> FileSigns = new Dictionary<string, string>(); //Dictionary to manage the documents and their respective sign 

        
        public FileButtons()
        {
            try
            {
                //Evaluates the path of the file with the signs, creates a new one in case its not there
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IDE", "keys.txt");
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.Create(path);
                }
                else
                {
                    char[] Separator = { ',' };
                    string Keystring = File.ReadAllText(path);
                    string[] Keys = Keystring.Split(Separator);
                    for (int i = 0; i < Keys.Length - 1; i += 2)
                    {
                        FileSigns.Add(Keys[i], Keys[i + 1]);
                    }
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error: {ex.Message}");
            }   
        }
        //Opens a new document 
        //TODO change the function to not use the richtextbox and just get a string, move the work of the textbox to the view
        public bool OpenDoc()
        {

            file.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
            bool? success = file.ShowDialog();
            if (success == true)
            {
                //Checks if the file was created with the IDE and if the sign equals the one the keys.txt has
                if (!FileSigns.ContainsKey(Path.GetFileName(file.FileName)) || !FileSigns[Path.GetFileName(file.FileName)].Equals(GetHash(File.ReadAllText(file.FileName))))
                {
                    MessageBox.Show("No puedes abrir este documento debido a que ha sido alterado fuera del IDE");
                    file = new OpenFileDialog();
                    return false;
                }
                return true;
            }
            file = new OpenFileDialog();
            return false;
        }

        private string GetHash(string content)
        {
            using (SHA256 hash = SHA256.Create()) //creates the sign for the document 
            {
                byte[] keys = hash.ComputeHash(Encoding.UTF8.GetBytes(content));
                StringBuilder sb = new StringBuilder();
                foreach (byte key in keys)
                {
                    sb.Append(key.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public string GetCurrentFileContent()
        {
            return File.ReadAllText(file.FileName);
        }
         public bool NewFile()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "py files (*.py)|*.py|All files (*.*)|*.*";
            bool? success = saveFileDialog1.ShowDialog();
            if (success == true)
            {
                file.FileName = saveFileDialog1.FileName;
                return true;
            }
            return false;
        }

        public void SaveDoc(string content)
        {
            if (String.IsNullOrEmpty(file.FileName)) //if it's a new file
            {
                 //Return if no new file was selected
                if (NewFile()) return;
            }
            string filename = Path.GetFileName(file.FileName);
            string sign = GetHash(content);
            if (FileSigns.ContainsKey(filename))  //adds the sign to the dictionary and the keys file
            {
                FileSigns[filename] = sign;
                string fileContent = File.ReadAllText(path);
                int index = fileContent.IndexOf(Path.GetFileName(file.FileName));
                index += filename.Length + 1;
                fileContent = fileContent.Remove(index, 64);
                fileContent = fileContent.Insert(index, sign);
                File.WriteAllText(path, fileContent);
            }
            else
            {
                FileSigns.Add(filename, sign);
                using (StreamWriter writer = File.AppendText(path)) //saves the sign in the txt document
                {
                    writer.Write(filename);
                    writer.Write(",");
                    writer.Write(sign);
                    writer.Write(",");
                }
            }  
            using (StreamWriter writer = new StreamWriter(file.FileName))//saves the content in the file
            {
                    writer.Write(content);
            }

        }
        public string getFilePath()
        {
            return file.FileName;
        }
    }
}