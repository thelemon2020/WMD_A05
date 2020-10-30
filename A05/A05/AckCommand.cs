using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    class AckCommand : Command
    {
        public AckCommand(connection currentConnection, string userCommand)
        {
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
            command = userCommand;
            createProtocol();
        }
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
        }
    }
}
