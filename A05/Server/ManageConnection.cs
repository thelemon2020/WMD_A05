//*********************************************
// File			 : ManageConnection.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, 8656317
// Last Change   : 2020-11-09
// Description	 : The manage connection class is the class that grabs and creates threads from TcpClient objects with the methods
//				 : Contained, as well as runs a reply loop in a separate thread that acts solely to send messages to all connected
//				 : Clients.
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
    // Name     : ManageConnection
    // Purpose  : A class which contains the methods used for the operating threads, it manages sending and receiving patterns
    //          : for all connections including incoming messages, outgoing messages, and replies back to clients after a command
    //          : is received.
    //******************************************
    public class ManageConnection
    {


        ConnectRepo repo;
        public volatile bool run = true;
        public readonly object lockobj;


        /////////////////////////////////////////
        // Method       : ManageConnection (ctor)
        // Description  : The constructor for the ManageConnection class, it assigns the connection repo and an object used
        //              : for locking sensitive operations, such as file access
        // Parameters   : ConnectRepo cr : the reference of the ConnectRepo object defined in server
        // Returns      : N/A
        /////////////////////////////////////////
        public ManageConnection(ConnectRepo cr)
        {
            repo = cr;
            lockobj = new object();
        }


        /////////////////////////////////////////
        // Method       : Connect
        // Description  : A method that takes the listener and gets a client connection as a TcpClient objects, then starts a 
        //              : thread with that new client
        // Parameters   : TcpListener listener : the server listener
        // Returns      : N/A
        /////////////////////////////////////////
        public void Connect(TcpListener listener)
        {
            TcpClient client = listener.AcceptTcpClient(); // Accept a new client
            Thread clientThread = new Thread(() => HandleClient(client, repo)); // Create a delegate for the client thread to run on
            clientThread.Start();
        }


        /////////////////////////////////////////
        // Method       : HandleClient
        // Description  : The method that handles incoming client communications, parses them, then sends back the necessary ACK or NACK
        // Parameters   : TcpClient client : the client connection object used to grab the stream and communicate with the client
        //              : ConnectRepo repo : the connect repo object passed as a reference from the server to be used in the thread
        // Returns      : N/A
        /////////////////////////////////////////
        private void HandleClient(TcpClient client, ConnectRepo repo)
        {
            // In this handle client method, the server doesn't need to keep this in a loop
            // The server simply gets a connection receives a message, sends an acknowledgment and then closes connection
            string recMsg;
            string endPoint = client.Client.RemoteEndPoint.ToString(); // do these 3 steps to get the IP of the client
            string[] ip = endPoint.Split(':');

            Connection clientConnection = new Connection(repo, lockobj); // create a new connection class for each client thread
            NetworkStream stream = client.GetStream(); // Open the network stream to the current client thread.
            
            recMsg = clientConnection.Receive(stream); // get the communication from the client
            clientConnection.Parse(recMsg, clientConnection); // parse the command
            IPAddress.TryParse(ip[0], out IPAddress IP);
            clientConnection.IP = IP;
            clientConnection.Send(clientConnection.AckMsg, stream); // send back an acknowledgement of receiving

            if(clientConnection.ShutDown == true) // if the super user sends the shut down command a flag is set in parse
            {                                     // and then the server will tell all clients to disconnect, and stop the server
                repo.AddMsg("DISCONNECT,<EOF>");
                Thread.Sleep(5000);
                run = false;
            }

            stream.Close(); // close the stream then close the connection. no need to keep them open any longer
            client.Close();
        }


        /////////////////////////////////////////
        // Method       : SendReplies
        // Description  : The method used in a thread to repeatedly send messages to all clients if there is a message in
        //              : the queue to be sent. If there is not message in the queue, the loop repeatedly checks the queue till
        //              : a message shows up to be sent.
        // Parameters   : ConnectRepo cr : the connect repo passed by reference to be used to get client details for sending data
        // Returns      : N/A
        /////////////////////////////////////////
        public void SendReplies(ConnectRepo cr)
        {
            while(run) // loop until the admin gives the shutdown server command
            {
                if (cr.msgQueue.IsEmpty) // don't attempt to send messages if there aren't messages to send
                {
                    Thread.Sleep(500);
                    continue;
                }
                else // if there are messages in the queue then go through each message and send it to each connection
                {
                    foreach (string msg in repo.msgQueue) // send all the messages that are in the queue 
                    {
                        string dequeuedMessage = null;
                        repo.msgQueue.TryDequeue(out dequeuedMessage);
                        string[] split = dequeuedMessage.Split(','); // split this up so we can grab the sender of the message

                        foreach (KeyValuePair<string, Connection> entry in repo.repo) // send message to all other clients
                        {
                            if (split[1] != entry.Value.Name) // don't send the message to the sender
                            {
                                try
                                {
                                    string recMsg = "";
                                    TcpClient tmpClient = new TcpClient(); // server acts like client and connects to client's listener thread
                                    tmpClient.Connect(entry.Value.IP, entry.Value.Port); // connect to client's IP and port
                                    NetworkStream tmpStream = tmpClient.GetStream(); // get stream to client
                                    entry.Value.Send(msg, tmpStream); // Send the message as a reply to the client

                                    recMsg = entry.Value.Receive(tmpStream);
                                    entry.Value.Parse(recMsg, entry.Value); /// parse the received message, which will be an ack

                                    tmpStream.Close(); // close both the stream and connection after sending
                                    tmpClient.Close();
                                }
                                catch(SocketException e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                        }
                    }
                }
            } 
        }
    }
}
