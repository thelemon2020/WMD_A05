using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{ 
    public class ReplyCommand : Command
    {
        private const string header = "REPLY";
        private const string footer = "<EOF>";

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

        public string CheckMessage(string[] splitMsg)
        {
            int endIndex = 0;
            for(int i = 0; i < splitMsg.Length; i++)
            {
                if (splitMsg[i] == "<EOF>")
                {
                    endIndex = i;
                }
            }
            string tmpMsg = "";

            for (int i = 1; i < endIndex; i++)
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
