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
        private const string failMsg = "FAIL";
        private const string footer = "<EOF>";


        public string BuildProtocol(int status)
        {
            string tmpMsg = "";

            protocol.Append(header); // build the acknowledgement so it's ready to send
            protocol.Append(",");

            if (status == 1)
            {
                protocol.Append(okMsg);
            }
            else
            {
                protocol.Append(failMsg);
            }
            protocol.Append(",");
            protocol.Append(footer);
            tmpMsg = protocol.ToString();
            protocol.Clear();
            return tmpMsg;
        }
    }
}
