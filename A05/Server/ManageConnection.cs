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
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Connected!");
            Thread clientThread = new Thread(() => HandleClient(client, repo));
            clientThread.Start();
        }

        private bool CheckConnectionExists(ConnectRepo repo, Command client)
        {
            return repo.CheckExists(client.Name);
        }

        public void HandleClient(TcpClient client, ConnectRepo repo)
        {
            Command clientCommand = new Command();
            clientCommand.Client = client;
            string message = clientCommand.Receive();
            Console.WriteLine(message);
            return;
        }
    }
}
