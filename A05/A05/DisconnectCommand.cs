﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    class DisconnectCommand : Command
    {
        public DisconnectCommand(connection currentConnection, Mutex toWrite)
        {
            command = "DISCONNECT";
            serverIP = currentConnection.ipAddress;
            serverPort = currentConnection.serverPort;
        }
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
            protocol.Append(',');
            protocol.Append("<EOF>");
        }
    }

}