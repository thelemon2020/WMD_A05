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
        private ConnectRepo repo;

        public ManageConnection()
        {
            repo = new ConnectRepo();
        }

        public void Connect(TcpListener listener)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread clientThread = new Thread(() => HandleClient(client, repo));
            clientThread.Start();
        }

        private bool CheckConnectionExists(ConnectRepo repo, Command client)
        {
            return repo.CheckExists(client.Name);
        }

        public void HandleClient(TcpClient client, ConnectRepo repo)
        {

            return;
        }
    }
}
