using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    class DisconnectCommand : Command
    {
        public DisconnectCommand(connection currentConnection)
        {
            command = "DISCONNECT";
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
            userName = currentConnection.username;
        }
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
            protocol.Append(',');
            protocol.Append(userName);
            protocol.Append(',');
            protocol.Append("<EOF>");
        }
    }

}
