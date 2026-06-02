using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;


namespace Proyecto_Diseño
{

    interface ScriptInt
    {
        string GetCurrentFileContent();
        string GetPath();
        void SaveContent(string content);
    }
    internal class Script : ScriptInt
    {
        private string scriptcontent;
        private string path;

        public Script(string path)
        {
            this.path = path;
            this.scriptcontent = File.ReadAllText(path);
        }

        public string GetCurrentFileContent()
        {
            return scriptcontent;
        }
        public void SaveContent(string content)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(content);
            }
            scriptcontent = content;
        }

        public string GetPath()
        {
            return path;
        }
    }
    internal class ScriptDecorator : ScriptInt
    {
        protected ScriptInt wrappee;
        public ScriptDecorator(ScriptInt wrappee)
        {
            this.wrappee = wrappee;
        }

        public string GetPath()
        {
            return wrappee.GetPath();
        }
        public string GetCurrentFileContent()
        {
            return wrappee.GetCurrentFileContent();
        }

        public virtual void SaveContent(string content)
        {
            wrappee.SaveContent(content);
        }
    }

    internal class SignedScript : ScriptDecorator
    {
        string Keyspath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IDE", "keys.txt");
        public SignedScript(ScriptInt wrappee) : base(wrappee)
        {
            if (!File.Exists(Keyspath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Keyspath));
                File.Create(Keyspath);
            }
        }

        public bool verificarfirma()
        {
            char[] Separator = { ',' };
            string Keystring = File.ReadAllText(Keyspath);
            string[] Keys = Keystring.Split(Separator);
            int pos = Array.IndexOf(Keys, Path.GetFileName(wrappee.GetPath()));
            if (pos != -1)
            {
                return String.Equals(GetHash(wrappee.GetCurrentFileContent()), Keys[pos + 1]);
            }
            return false;
        }

        private string GetHash(string content)
        {
            using (SHA256 hash = SHA256.Create())
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

        public override void SaveContent(string content)
        {
            wrappee.SaveContent(content);
            SignScript();
        }
        public void SignScript()
        {
            string filename = Path.GetFileName(wrappee.GetPath());
            string KeysfileContent = File.ReadAllText(Keyspath);
            if (KeysfileContent.Contains(filename))
            {
                int index = KeysfileContent.IndexOf(filename);
                index += filename.Length + 1;
                KeysfileContent = KeysfileContent.Remove(index, 64);
                KeysfileContent = KeysfileContent.Insert(index, GetHash(wrappee.GetCurrentFileContent()));
                File.WriteAllText(Keyspath, KeysfileContent);
            }
            else
            {
                using (StreamWriter writer = File.AppendText(Keyspath)) //saves the sign in the txt document
                {
                    writer.Write(filename);
                    writer.Write(",");
                    writer.Write(GetHash(wrappee.GetCurrentFileContent()));
                    writer.Write(",");
                }
            }
        }
    }

    internal class ScriptFormat : ScriptDecorator
    {

        public ScriptFormat(ScriptInt wrappee) : base(wrappee)
        {

        }
        //Blue [B]
        //Yellow [Y]
        //Pink [P]
        //Green [G]
        public string GetFormattedText()
        {
            string text = wrappee.GetCurrentFileContent();
            string[] specialkey = { "def", "class", "if", "else", "elif", "for", "while", "return", "import", "from", "as", "try", "except", "print", "True", "true", "false", "False", "none", "None", "and", "or", "not", "in", "is", "pass", "with" };
            string comment = "#.*";
            text = Regex.Replace(text, comment, character => $"[Y]{character.Value}[/Y]");
            text = Regex.Replace(text, @"\b[a-zA-Z_][a-zA-Z0-9_]*\s*(?=\()", character => $"[B]{character.Value}[/B]");
            text = Regex.Replace(text, @"(""(?:\\.|[^""\\])*""|'(?:\\.|[^'\\])*')", character => $"[G]{character.Value}[/G]");
            for (int i = 0; i < specialkey.Length; i++)
            {
                string key = specialkey[i];
                text = Regex.Replace(text, $@"\b{key}\b", character => $"[P]{character.Value}[/P]");
            }
            return text;
        }


    }
}