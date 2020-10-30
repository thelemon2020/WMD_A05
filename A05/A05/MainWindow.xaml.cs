using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isConnected { get; set; }
        public connection currentConnection { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            
            ConnectWindow newConnection = new ConnectWindow();
            newConnection.Owner = this;
            newConnection.Show();
            currentConnection = new connection(newConnection.Name, newConnection.userPassword, 
                newConnection.ipAddress, newConnection.port);
            if (currentConnection !=null)
            {
                StartConnectionCommand startUp = new StartConnectionCommand(currentConnection);
                if (startUp.ExecuteCommand() != null)
                {
                    isConnected = true;

                    string connectedMessage = string.Format("Connected to server at {0}", currentConnection.ipAddress.ToString());
                    chatWindow.Text+=connectedMessage;
                }
            }
            newConnection.Close();
        }
    }
}
