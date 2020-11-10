//*********************************************
// File			 : ReplyCommand.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, 8656317
// Last Change   : 2020-11-09
// Description	 : A class used to build protocol specific outgoing reply messages to send back to clients.
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //******************************************
    // Name     : ReplyCommand
    // Purpose  : A class which acts to build reply commands with the strig builder inherited from the command class
    //******************************************
    public class ReplyCommand : Command
    {
        private const string header = "REPLY";
        private const string footer = "<EOF>";


        /////////////////////////////////////////
        // Method       : BuildProtocol
        // Description  : A method that builds a reply string containing the message sent from one user, to be sent to all other users
        // Parameters   : string message : the message sent by the sending client.
        // Returns      : string tmpMsg : The fully built reply string
        /////////////////////////////////////////
        public string BuildProtocol(string message)
        {
            string tmpMsg = "";
            protocol.Append(header);
            protocol.Append(",");
            protocol.Append(message);
            protocol.Append(",");
            protocol.Append(footer);

            tmpMsg = protocol.ToString();
            protocol.Clear();

            return tmpMsg;
        }


        /////////////////////////////////////////
        // Method       : CheckMessage
        // Description  : A method that puts together a string from the user that may have had commas in it. We split on commas
        //              : to parse the commands, but that means the message must be rebuilt and sent to all other clients properly
        // Parameters   : string[] splitMsg : the message from the sending user, split on commas.
        // Returns      : string tmpMsg : the rebuilt string to be sent back to the other clients
        /////////////////////////////////////////
        public string CheckMessage(string[] splitMsg)
        {
            string tmpMsg = "";

            for (int i = 1; i < splitMsg.Length - 1; i++)
            {
                if (i != 1)
                {
                    tmpMsg += ", ";
                }
                tmpMsg += splitMsg[i];
                
            }

            return tmpMsg;
        }
    }
}
