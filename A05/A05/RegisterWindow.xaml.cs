/*
* FILE : RegisterWindow.xaml.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 04
* REVISED ON : 2020 - 11 - 05
* DESCRIPTION : This file defines the RegisterWindow UI class.  
*/
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
    /*
    * NAME : RegisterWindow
    * PURPOSE : This defines the RegisterWindow class.  It allows the user to enter the information neccessary to register on a server
    */
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
        /*
        * METHOD : Register()
        *
        * DESCRIPTION : This method copies the user information into properties to be accessed by the MainWindow
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
        private void Register(object sender, RoutedEventArgs e)
        {
            int tempPort = 0;
            username = userName.Text;
            userPassword = password.Password;
            ipAddress = IP.Text;
            int.TryParse(Port.Text, out tempPort);
            port = tempPort;
            canProceed = true;
            this.Close();
        }
        /*
       * METHOD : cancelWindow()
       *
       * DESCRIPTION : This method closes this window
       *
       * PARAMETERS : sender - the object that called this method
       *              e - the arguments sent by the routed event
       *
       * RETURNS : Nothing
       */
        private void cancelWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
