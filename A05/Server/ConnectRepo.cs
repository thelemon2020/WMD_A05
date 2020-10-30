using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Server
{
    public class ConnectRepo
    {
        public ConcurrentDictionary<string, TcpClient> repo;

        public ConnectRepo()
        {
            repo = new ConcurrentDictionary<string, TcpClient>();
        }

        public void Add(string key, TcpClient client)
        {
            repo.TryAdd(key, client);
        }


        public void Remove(string key)
        {
            repo.TryRemove(key, out TcpClient client);
        }

        public bool CheckExists(string key)
        {
            if(repo.TryGetValue(key, out TcpClient tmpClient)) { return true; }
            else { return false; }
        }
    }
}
