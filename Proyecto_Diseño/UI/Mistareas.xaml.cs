using Google.Protobuf.WellKnownTypes;
using Proyecto_Diseño.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
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
using System.Windows.Shapes;

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
        private async void Prueba_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TareaSelect(object sender, SelectionChangedEventArgs e)
        {
            TareaInfo tarea = (TareaInfo)Tareascombo.SelectedItem;
            TareaDescrip.Document.Blocks.Clear();
            TareaDescrip.Document.Blocks.Add(new Paragraph(new Run(tarea.descripcion)));
            AdjuntoBox.Text = tarea.adjunto;
            DateBox.Text = tarea.fechaEntrega;
        }
    }
}
