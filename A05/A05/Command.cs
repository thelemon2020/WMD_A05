using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    public class Command
    {
        public string header { get; set; }
        public string command { get; set; }
        public string footer { get; set; }
        public IPAddress serverIP { get; set; }
        public int serverPort { get; set; }
        public StringBuilder protocol { get; set; }
        
        public string ExecuteCommand()
        {
            NetworkStream serverStream = null;
            StringBuilder serverResponse = new StringBuilder();
            serverStream = InteractWithServer.connectToServer(serverIP, serverPort);
            if (serverStream == null)
            {
                serverResponse.Append(command);
                serverResponse.Append(" Failed - Server Could Not Be Reached");
            }
            else
            {
                InteractWithServer.writeToServer(serverStream, protocol.ToString());
                serverResponse.Append(InteractWithServer.readFromServer(serverStream));
                InteractWithServer.closeConnection(serverStream);
            }
            return serverResponse.ToString();
        }
    }

  
}
