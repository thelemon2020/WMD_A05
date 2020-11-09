/*
* FILE : Command.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the Command Class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
   /*
    * NAME : Command
    * PURPOSE : This defines the Command class.  This is a parent class from which all commands derive.  It contains the logic to send a command to the server
    */
    public class Command
    {
        public string command { get; set; }
        public string userName {get; set;}
        public IPAddress serverIP { get; set; }
        public int serverPort { get; set; }
        public StringBuilder protocol { get; set; }
       /*
        * METHOD : ExecuteCommand()
        *
        * DESCRIPTION : This method initiates the connection with the server, sends the command and receives the response.
        *
        * PARAMETERS : None
        *
        * RETURNS : Nothing
        */
        public string ExecuteCommand()
        {
            string serverResponse = "";
            try
            {
                NetworkStream serverStream = null;
              
                serverStream = InteractWithServer.connectToServer(serverIP, serverPort); // start server connection
                if (serverStream == null)
                {
                    serverResponse += command + " Failed - Server Could Not Be Reached"; //error message if the connection fails
                }
                else
                {
                    InteractWithServer.writeToServer(serverStream, protocol.ToString());
                    serverResponse = InteractWithServer.readFromServer(serverStream);
                    InteractWithServer.closeConnection(serverStream);
                    if (serverResponse.Contains("NACK"))
                    {
                        string[] temp = serverResponse.Split(',');
                        serverResponse = "Server Returned Error -" + temp[1] + "\n"; //server rejects connection
                    }
                }
            }
            catch
            {
                serverResponse = "Connection Failed - Server Could Not Be Reached"; //in case of exception
            }
            return serverResponse;
        }
    }

  
}
