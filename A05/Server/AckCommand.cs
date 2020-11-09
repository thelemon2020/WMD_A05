//*********************************************
// File			 : AckCommand.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, Chris Lemon
// Last Change   : 2020-11-09
// Description	 : A command class for building acknowledgements to send back to the client
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AckCommand : Command
    {
        private const string header = "ACK";
        private const string okMsg = "OK";
        private const string footer = "<EOF>";

        /////////////////////////////////////////
        // Method       : BuildProtocol
        // Description  : One of two overloaded methods that produces an acknowledgement string to send to the client
        // Parameters   : N/A
        // Returns      : string tmpMsg : the built message to send to the user
        /////////////////////////////////////////
        public string BuildProtocol()
        {
            string tmpMsg = "";

            protocol.Append(header); // build the acknowledgement so it's ready to send
            protocol.Append(",");
            protocol.Append(okMsg);
            protocol.Append(",");
            protocol.Append(footer);
            tmpMsg = protocol.ToString();
            protocol.Clear();
            return tmpMsg;
        }


        /////////////////////////////////////////
        // Method       : BuildProtocol
        // Description  : The second overloaded build protocol method, this one is only used on connection request to send all
        //              : The users that are currently using the chat and are connected
        // Parameters   : int flag: The flag for whether a user is normal or has admin priviledges
        //              : ConnectRepo repo : The repo for which users are connected
        // Returns      : string tmpMsg : The built string to be sent to the user containing an ack, as well as the list of users
        /////////////////////////////////////////
        public string BuildProtocol(int flag, ConnectRepo repo, Connection c)
        {
            string tmpMsg = "";

            protocol.Append(header); // build the acknowledgement so it's ready to send
            protocol.Append(",");
            protocol.Append(okMsg);
            protocol.Append(",");
            protocol.Append(c.Port);
            protocol.Append(",");
            protocol.Append(flag);
            protocol.Append(",");
            foreach (KeyValuePair<string, Connection> entry in repo.repo) // Add all the active users
            {
                protocol.Append(entry.Value.Name);
                protocol.Append(",");
            }
            protocol.Append(footer);
            tmpMsg = protocol.ToString();
            protocol.Clear();
            return tmpMsg;
        }
    }
}
