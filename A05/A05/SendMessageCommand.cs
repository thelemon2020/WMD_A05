using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    public class SendMessageCommand : Command
    {
        public string message { get; set; }
        
        public SendMessageCommand(connection currentConnection, string userMessage, Mutex toWrite)
        {
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
            message = userMessage;
            command = "SEND";
            waitToWrite = toWrite;
            createProtocol();
        }
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
            protocol.Append(message);

        }
    }
}
