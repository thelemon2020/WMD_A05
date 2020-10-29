using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    class ConnectRepo
    {
        public Dictionary<string, TcpClient> repo;

        public ConnectRepo()
        {
            repo = new Dictionary<string, TcpClient>();
        }

        public void Add(string key, TcpClient client)
        {
            repo.Add(key, client);
        }


        public void Remove(string key)
        {
            repo.Remove(key);
        }

        public bool CheckExists(string key)
        {
            if(repo.TryGetValue(key, out TcpClient tmpClient))
            {
                return true;
            }
            else { return false; }
        }
    }
}
