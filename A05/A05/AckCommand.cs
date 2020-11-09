/*
* FILE : AckCommand.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the Acknowledge Command Class
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
    * NAME : AckCommand
    * PURPOSE : This defines the Acknowledge Command Class.  It inherits from the Command class.  It's purpose is to provide a protocol to send to the server to acknowledge 
    *           that the last command was received correctly
    */
    class AckCommand : Command
    {
       /*
        * METHOD : AckCommand()
        *
        * DESCRIPTION : The constructor for AckCommand
        *
        * PARAMETERS : UserCommand - the command to be sent to the server
        *
        * RETURNS : Nothing
        */
        public AckCommand(string userCommand)
        {
            command = userCommand;
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
        }
    }
}
