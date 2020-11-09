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

namespace A05
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public string username { get; set; }
        public string userPassword { get; set; }
        public string ipAddress { get; set; }
        public int port { get; set; }
        public bool canProceed { get; set; }
        public RegisterWindow()
        {
            canProceed = false;
            InitializeComponent();
        }
        private void Register(object sender, RoutedEventArgs e)
        {
            int tempPort = 0;//
            username = userName.Text;
            userPassword = password.Password;
            ipAddress = IP.Text;
            int.TryParse(Port.Text, out tempPort);
            port = tempPort;
            canProceed = true;
            this.Close();
        }
        private void cancelWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
