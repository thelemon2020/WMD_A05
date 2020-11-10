//*********************************************
// File			 : Server.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, Chris Lemon
// Last Change   : 2020-11-09
// Description	 : The server class is where the main is held, and it's here that the server listens for new connections
//               : and then uses the manager class to connect and start a client thread. The server listens until it gets
//               : the command to stop running from the admin.
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
    //******************************************
    // Name     : Server
    // Purpose  : The server class acts as a server waiting for incoming communications from clients on the port 23000
    //          : when it sees a connection is pending it shoots off into a method of the ManageConnection class and 
    //          : accepts the new client.
    //******************************************
    class Server
    {
        const int kDefaultPort = 23000;
        public ManageConnection manager;
        public ConnectRepo repo;

        static void Main(string[] args)
        {
            Server myServer = new Server();
        }


        /////////////////////////////////////////
        // Method       : Server (ctor)
        // Description  : The server constructor, it creates a new TcpListener, instantiates the ManageConnection and ConnectRepo
        //              : classes and then starts the server function.
        // Parameters   : N/A
        // Returns      : N/A
        /////////////////////////////////////////
        public Server()
        {
            repo = new ConnectRepo();
            manager = new ManageConnection(repo);
            TcpListener listener = new TcpListener(IPAddress.Any, kDefaultPort);
            listener.Start();
            startServer(listener);
        }


        /////////////////////////////////////////
        // Method       : StartServer
        // Description  : The method that runs the main server loop it listens for clients until the admin shuts down the server.
        //              : it also starts the outgoing message reply thread: SendReplies.
        // Parameters   : TcpListener listener : The server specific listener that waits for client communications
        // Returns      : N/A
        /////////////////////////////////////////
        private void startServer(TcpListener listener)
        {
            Console.WriteLine("Listening for Connections. . .");
            Thread replyThread = new Thread(new ThreadStart(() => manager.SendReplies(repo))); // start the message reply thread
            replyThread.Start(); // start the message reply thread

            while(manager.run) // run the listener until it is told to stop by the admin
            {
                if(!listener.Pending()) // loop without blocking until there is a pending connecting
                {
                    Thread.Sleep(1000);
                    continue;
                }

                manager.Connect(listener);
                Console.WriteLine("Connected!");
            }
            listener.Stop(); // stop the listener after the shut down command is given
            Console.WriteLine("Server stopped. . .\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
