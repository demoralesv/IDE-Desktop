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
            var cor = Correo.Text;
            var pass = Password.Text;
            ApiService Api = ApiService.getInstance();
            var result = Api.PostUser(cor, pass);
            string Messageresult = await result;
            MessageBox.Show(Messageresult);
        }
    }
}
