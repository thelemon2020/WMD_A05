using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ManageConnection
    {
        public void Connect(TcpListener listener)
        {
            TcpClient client = listener.AcceptTcpClient();
        }

        private bool CheckConnectionExists(ConnectRepo repo, Connection client)
        {
            return repo.CheckExists(client.Name);
        }

        private void HandleClient()
        {

        }
    }
}
