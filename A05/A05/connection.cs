/*
* FILE : connections.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the connections Class
*/
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
   /*
    * NAME : connections
    * PURPOSE : This defines the connection class.  It is a class that holds the information of the current server connection
    */
    public class connection
    {
        public string username { get; set; }
        public string userPassword { get; set; }
        public IPAddress ipAddress { get; set; }
        public int serverPort { get; set; }
        public int clientPort { get; set; }
       /*
        * METHOD : connection()
        *
        * DESCRIPTION : The constructor for this class
        *
        * PARAMETERS : user - the username of the user connecting to the server
        *              pass - the password in plaintext
        *              address - address of the server
        *              port - port of the server
        *
        * RETURNS : Nothing
        */
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
                MessageBox.Show("Invalid IP Address Format", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error); //in case the user enters a bad IP
            }
            serverPort = port;
        }
    }
}
