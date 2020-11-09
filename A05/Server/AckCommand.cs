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

        public string BuildProtocol(int status, ConnectRepo repo)
        {
            string tmpMsg = "";

            protocol.Append(header); // build the acknowledgement so it's ready to send
            protocol.Append(",");

            if (status == 1)
            {
                protocol.Append(okMsg);
            }
            protocol.Append(",");
            foreach (KeyValuePair<string, Connection> entry in repo.repo)
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
