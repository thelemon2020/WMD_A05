using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace A05
{
    public class connection
    {
        public string username { get; set; }
        public string userPassword { get; set; }
        public IPAddress ipAddress { get; set; }
        public int serverPort { get; set; }
        public int clientPort { get; set; }

        public connection(string user, string pass, string address, int port)
        {
            username = user;
            userPassword = pass;
            try
            {
                ipAddress = IPAddress.Parse(address);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid IP Address Format", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            serverPort = port;
        }
    }
}
