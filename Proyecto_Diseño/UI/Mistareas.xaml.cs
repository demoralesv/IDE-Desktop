using Google.Protobuf.WellKnownTypes;
using Microsoft.Win32;
using Proyecto_Diseño.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Proyecto_Diseño.UI
{
    /// <summary>
    /// Interaction logic for Mistareas.xaml
    /// </summary>
    public partial class Mistareas : Window
    {
        List<CourseInfo> info;
        public Mistareas()
        {
            InitializeComponent();
            this.IsEnabled = false;
            this.Loaded += InitCursos;
        }

        private async void InitCursos(object sender, RoutedEventArgs e)
        {
            try
            {
                var Api = ApiService.getInstance();
                var result = Api.GetCursos();
                info = await result;
                CoursesList.ItemsSource = info;
            }
            catch 
            {
                MessageBox.Show("Ocurrió un error cargando los cursos");
            }
            this.IsEnabled=true;
        }

        private async void TareaSelect(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                TareaInfo tarea = (TareaInfo)Tareascombo.SelectedItem;
                TareaDescrip.Document.Blocks.Clear();
                if (tarea != null)
                {
                    TareaDescrip.Document.Blocks.Add(new Paragraph(new Run(tarea.descripcion)));
                    AdjuntoBox.Text = tarea.adjunto;
                    DateBox.Text = tarea.fechaEntrega;
                    var Api = ApiService.getInstance();
                    var result = Api.GetAssignmentGroup(tarea.ID);
                    ResultCourses jsonresult = await result;
                    if (jsonresult.data != null)
                    {
                        Groups.ItemsSource = jsonresult.data.group.members;
                    }
                }
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void BotonPruebaClick(object sender, RoutedEventArgs e)
        {
            
        }

        private async void SubmitWork(object sender, RoutedEventArgs e)
        {

            TareaInfo tarea = (TareaInfo)Tareascombo.SelectedItem;
            if (tarea != null)
            {
                OpenFileDialog file = new OpenFileDialog();
                bool? success = file.ShowDialog();
                if (success == true)
                {
                    var Api = ApiService.getInstance();
                    if (Path.GetExtension(file.FileName).Equals(".zip"))
                    {
                        if (checksignsZip(file.FileName))
                        {
                            MessageBox.Show("El archivo contiene un script sin firmar ");
                        }
                    }
                    if (Path.GetExtension(file.FileName).Equals(".py"))
                    {
                        SignedScript prueba = new SignedScript(new Script(file.FileName));
                        if (!prueba.verificarfirma())
                        {
                            MessageBox.Show("El archivo no esta firmado por el IDE");
                            return;
                        }
                    }
                    
                    var result = Api.submitAssignment(file.FileName, tarea.ID);
                    var jsonresult = await result;
                    
                    MessageBox.Show("Entrega realizada con éxito");
                }
            }
        }

        private bool checksignsZip(string path)
        {
            Script s = new Script("");
            using (ZipArchive zip = ZipFile.OpenRead(path)) {
                
                foreach (var file in zip.Entries)
                {
 
                    if (Path.GetExtension(file.FullName).Equals(".py")){
                        s.SetPath(file.FullName);
                        StreamReader sr = new StreamReader(file.Open());
                        s.SetContent(sr.ReadToEnd());
                        
                        SignedScript ss = new SignedScript(s);
                        if (!ss.verificarfirma())
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
