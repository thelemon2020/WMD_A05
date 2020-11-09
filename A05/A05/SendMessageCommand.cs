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
        
        public SendMessageCommand(connection currentConnection, string userMessage)
        {
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
            userName = currentConnection.username;
            message = userMessage;
            command = "SEND";
            createProtocol();
        }
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
            protocol.Append(',');
            protocol.Append(userName);
            protocol.Append(',');
            protocol.Append(message);
            protocol.Append(',');
            protocol.Append("<EOF>");

        }
    }
}
