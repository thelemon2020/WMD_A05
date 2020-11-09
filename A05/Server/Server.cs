//*********************************************
// File			 : 
// Project		 : 
// Programmer	 : Nick Byam, 8656317
// Last Change   : 
// Description	 : 
//				 : 
//				 : 
//*********************************************


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
        public ConnectRepo repo;

        static void Main(string[] args)
        {
            Server myServer = new Server();
        }


        public Server()
        {
            repo = new ConnectRepo();
            manager = new ManageConnection(repo);
            TcpListener listener = new TcpListener(IPAddress.Any, kDefaultPort);
            listener.Start();
            startServer(listener);
        }


        private void startServer(TcpListener listener)
        {
            Console.WriteLine("Listening for Connections. . .");
            Thread replyThread = new Thread(new ThreadStart(() => manager.SendReplies(repo))); // create a client that acts so send messages
            replyThread.Start();

            while(manager.run)
            {
                if(!listener.Pending())
                {
                    Thread.Sleep(1000);
                    continue;
                }

                manager.Connect(listener);
                Console.WriteLine("Connected!");
            }
            listener.Stop();
            Console.WriteLine("Server stopped. . .\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
