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
        public string userName {get; set;}
        public IPAddress serverIP { get; set; }
        public int serverPort { get; set; }
        public StringBuilder protocol { get; set; }
        
        public string ExecuteCommand()
        {
            string serverResponse = "";
            try
            {
                NetworkStream serverStream = null;
              
                serverStream = InteractWithServer.connectToServer(serverIP, serverPort);
                if (serverStream == null)
                {
                    serverResponse += command + " Failed - Server Could Not Be Reached";
                }
                else
                {
                    InteractWithServer.writeToServer(serverStream, protocol.ToString());
                    serverResponse = InteractWithServer.readFromServer(serverStream);
                    InteractWithServer.closeConnection(serverStream);
                    if (serverResponse.Contains("NACK"))
                    {
                        string[] temp = serverResponse.Split(',');
                        serverResponse = "Server Returned Error -" + temp[1] + "\n";
                    }
                }
            }
            catch
            {
                serverResponse = "Connection Failed - Server Could Not Be Reached";
            }
            return serverResponse;
        }
    }

  
}
