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
            string cor = Correobox.Text.Trim();
            string pass = PassBox.Text;
            string name = NameBox.Text.Trim();
            string ape = ApellidoBox.Text.Trim();

            CreateAccountMessage.Text = "";

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(cor) ||
                string.IsNullOrWhiteSpace(pass))
            {
                CreateAccountMessage.Text = "Nombre, correo y contraseña son obligatorios.";
                CreateAccountMessage.Foreground = Brushes.Firebrick;
                return;
            }

            RegistrarButton.IsEnabled = false;

            try
            {
                ApiService Api = ApiService.getInstance();
                string Messageresult = await Api.PostCreateUser(name, ape, cor, pass);

                CreateAccountMessage.Text = Messageresult;

                if (Messageresult.ToLower().Contains("correcto") ||
                    Messageresult.ToLower().Contains("exitoso") ||
                    Messageresult.ToLower().Contains("creado") ||
                    Messageresult.ToLower().Contains("registrado"))
                {
                    CreateAccountMessage.Foreground = Brushes.ForestGreen;
                }
                else
                {
                    CreateAccountMessage.Foreground = Brushes.Firebrick;
                }
            }
            catch
            {
                CreateAccountMessage.Text = "Ocurrió un error conectando con el server. Por favor intente de nuevo.";
                CreateAccountMessage.Foreground = Brushes.Firebrick;
            }
            finally
            {
                RegistrarButton.IsEnabled = true;
            }
        }
    }
}
