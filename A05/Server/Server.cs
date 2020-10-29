using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        const int kDefaultPort = 23000;
        public ManageConnection manager;

        static void Main(string[] args)
        {

        }


        public Server()
        {
            manager = new ManageConnection();
            TcpListener listener = new TcpListener(IPAddress.Any, kDefaultPort);
            listener.Start();
            startServer(listener);
        }


        private void startServer(TcpListener listener)
        {
            Console.WriteLine("Listening for Connections. . .");

            while(true)
            {
                if(!listener.Pending())
                {
                    Thread.Sleep(1000);
                    continue;
                }

                manager.Connect(listener);
            }
        }
    }
}
