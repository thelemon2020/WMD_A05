using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net;

namespace Server
{
    public class ConnectRepo
    {
        public ConcurrentDictionary<string, Connection> repo;
        public ConcurrentQueue<string> msgQueue;

        public ConnectRepo()
        {
            repo = new ConcurrentDictionary<string, Connection>();
            msgQueue = new ConcurrentQueue<string>();
        }

        public void Add(string key, Connection c)
        {
            repo.TryAdd(key, c);
        }


        public void Remove(string key)
        {
            repo.TryRemove(key, out Connection c);
        }

        public bool CheckExists(string key)
        {
            if(repo.TryGetValue(key, out Connection c)) { return true; }
            else { return false; }
        }

        public void AddMsg(string msg)
        {
            msgQueue.Enqueue(msg);
        }

        public string GetMsg()
        {
            msgQueue.TryDequeue(out string tmp);
            return tmp;
        }
    }
}
