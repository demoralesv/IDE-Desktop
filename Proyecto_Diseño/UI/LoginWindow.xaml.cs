using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Macs;
using Proyecto_Diseño.UI;
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


namespace Proyecto_Diseño
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        //Button create account
        private async void CrearCuentaB_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount CreateW = new CreateAccount();
            CreateW.Show();
        }

        private async void Iniciar_Sesión_Click(object sender, RoutedEventArgs e)
        {
            var cor = Correo.Text.Trim();
            var pass = Passwordbox.Password;

            LoginMessage.Text = "";

            if (string.IsNullOrWhiteSpace(cor) || string.IsNullOrWhiteSpace(pass))
            {
                LoginMessage.Text = "Ingrese el correo y la contraseña.";
                LoginMessage.Foreground = Brushes.Firebrick;
                return;
            }

            Iniciar_Sesión.IsEnabled = false;

            try
            {
                ApiService Api = ApiService.getInstance();
                string Messageresult = await Api.PostUser(cor, pass);

                LoginMessage.Text = Messageresult;

                if (Messageresult.ToLower().Contains("correcto") || Messageresult.ToLower().Contains("exitoso"))
                {
                    LoginMessage.Foreground = Brushes.ForestGreen;
                }
                else
                {
                    LoginMessage.Foreground = Brushes.Firebrick;
                }
            }
            catch
            {
                LoginMessage.Text = "No se pudo conectar con el servidor.";
                LoginMessage.Foreground = Brushes.Firebrick;
            }
            finally
            {
                Iniciar_Sesión.IsEnabled = true;
            }
        }
    }
}
