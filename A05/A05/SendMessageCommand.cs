/*
* FILE : SendMessageCommand.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the SendMessage Command Class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    /*
    * NAME : SendMessageCommand
    * PURPOSE : This defines the SendMessage Command Class.  It inherits from the Command class.  It's purpose is to provide a protocol to send a message to the server
    */
    public class SendMessageCommand : Command
    {
        public string message { get; set; }
        
        public SendMessageCommand(connection currentConnection, string userMessage)
        {
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
            userName = currentConnection.username;
            message = userMessage;
            command = "SEND";
            createProtocol();
        }
       /*
        * METHOD : createProtocol()
        *
        * DESCRIPTION : This creates the string that will be converted to bytes and sent to the server
        *
        * PARAMETERS : None
        *
        * RETURNS : Nothing
        */
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
            protocol.Append(',');
            protocol.Append(userName);
            protocol.Append(',');
            protocol.Append(message);
            protocol.Append(',');
            protocol.Append("<EOF>");

        }
    }
}
