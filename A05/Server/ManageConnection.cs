using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ManageConnection
    {
        ConnectRepo repo;
        public ManageConnection(ConnectRepo cr)
        {
            repo = cr;
        }

        public void Connect(TcpListener listener)
        {
            TcpClient client = listener.AcceptTcpClient(); // Accept a new client
            Console.WriteLine("Connected!");
            Thread clientThread = new Thread(() => HandleClient(client, repo)); // Create a delegate for the client thread to run on
            clientThread.Start();
        }

        public void HandleClient(TcpClient client, ConnectRepo repo)
        {
            Command clientCommand = new Command(); // start a new command class for every new client
            clientCommand.Client = client;
            string message = clientCommand.Parse(clientCommand.Receive(), repo); // Grab an incoming message
            clientCommand.Send(message); // send back an acknowledgement of receiving ok

            if(!repo.CheckExists(clientCommand.Name)) // if the client is not represented in the repo, it's a new connection
            {
                repo.Add(clientCommand.Name, client); // add the client name and info to the repo
            }

            foreach(KeyValuePair<string, TcpClient> entry in repo.repo) // send the message received to all connected clients
            {
                if (entry.Key != clientCommand.Name) // don't send the message to the one who actually sent the message first
                {
                    clientCommand.Send(repo.GetMsg());
                }
            }


        }
    }
}
