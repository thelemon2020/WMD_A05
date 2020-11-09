/*
* FILE : DisconnectCommand.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 05 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the DisconnectCommand Class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
    {
   /*
    * NAME : DisconnectCommand
    * PURPOSE : This defines the DisconnectCommand class.  It inherits from the Command class.  It exists to tell the server when a user wants to disconnect
    */
    class DisconnectCommand : Command
    {
        public DisconnectCommand(connection currentConnection)
        {
            command = "DISCONNECT";
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
            userName = currentConnection.username;
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
            protocol.Append("<EOF>");
        }
    }

}
