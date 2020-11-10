//*********************************************
// File			 : NackCommand.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, Chris Lemon
// Last Change   : 2020-11-09
// Description	 : A class used to build protocol specific NACK messages back to the user based on a condition code
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //******************************************
    // Name     : NackCommand
    // Purpose  : A class which uses a string builder from its base class to build NACK messages when data from the client
    //          : meets certain error conditions.
    //******************************************
    public class NackCommand: Command
    {
        // error message const strings
        const string NotRegister = "You Have Not Registered Yet.";
        const string Registered = "This User Has Already Been Registered";
        const string Incomplete = "Incomplete Data Received.";
        const string BadPermission = "You Don't Have Permission To Do This.";
        const string Error = "There Was a Problem With The Data Received.";

        // Header and footer const strings
        const string Header = "NACK";
        const string Footer = "<EOF>";


        /////////////////////////////////////////
        // Method       : BuildProtocol
        // Description  : The method that builds the string adhering to protocol specifics depending on the condition code,
        //              : a different error message is returned to the client.
        // Parameters   : int reason : the condition code that relates to the specific error message constants
        // Returns      : string tmp : a temporary string which holds the string built in the method.
        /////////////////////////////////////////
        public string BuildProtocol(int reason)
        {
            protocol.Append(Header);
            protocol.Append(",");
            
            if(reason == 0)
            {
                protocol.Append(Registered);
            }
            else if(reason == 1)
            {
                protocol.Append(NotRegister);
            }
            else if(reason == 2)
            {
                protocol.Append(Incomplete);
            }
            else if(reason == 3)
            {
                protocol.Append(BadPermission);
            }
            else
            {
                protocol.Append(Error);
            }
            protocol.Append(",");
            protocol.Append(Footer);
            string tmp = protocol.ToString();
            protocol.Clear();
            return tmp;
        }
    }
}
