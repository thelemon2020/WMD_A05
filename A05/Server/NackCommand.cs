using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class NackCommand: Command
    {
        const string NotRegister = "You Have Not Registered Yet.";
        const string Registered = "This User Has Already Been Registered";
        const string Incomplete = "Incomplete Data Received.";
        const string BadPermission = "You Don't Have Permission To Do This.";
        const string Error = "There Was a Problem With The Data Received.";
        const string Header = "NACK";
        const string Footer = "<EOF>";

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
