using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        private async void CreateUserButton(object sender, RoutedEventArgs e)
        {
            try
            {
                string cor = Correobox.Text;
                string pass = PassBox.Text;
                string name = NameBox.Text;
                string ape = ApellidoBox.Text;
                ApiService Api = ApiService.getInstance();
                var result = Api.PostCreateUser(name, ape, cor, pass);
                string Messageresult = await result;
                MessageBox.Show(Messageresult);
            }
            catch{
                MessageBox.Show("Ocurrió un error conectando con el server. Porfavor intente de nuevo");
            }
        }
    }
}
