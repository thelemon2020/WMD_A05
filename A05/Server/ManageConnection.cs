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
    public class ManageConnection
    {
        ConnectRepo repo;
        public volatile bool run = true;
        public readonly object lockobj;

        public ManageConnection(ConnectRepo cr)
        {
            repo = cr;
        }

        public void Connect(TcpListener listener)
        {
            TcpClient client = listener.AcceptTcpClient(); // Accept a new client
            Thread clientThread = new Thread(() => HandleClient(client, repo)); // Create a delegate for the client thread to run on
            clientThread.Start();
        }

        private void HandleClient(TcpClient client, ConnectRepo repo)
        {
            // In this handle client method, the server doesn't need to keep this in a loop
            // The server simply gets a connection receives a message, sends an acknowledgment and then closes connection
            string recMsg;
            Connection clientConnection = new Connection(repo); // create a new connection class for each client thread
            NetworkStream stream = client.GetStream(); // Open the network stream to the current client thread.
            
            recMsg = clientConnection.Receive(stream); // get the communication from the client
            clientConnection.Parse(recMsg, clientConnection); // parse the command

            clientConnection.Send(clientConnection.AckMsg, stream); // send back an acknowledgement of receiving

            stream.Close(); // close the stream then close the connection. no need to keep them open any longer
            client.Close();
        }

        public void SendReplies(ConnectRepo cr)
        {
            while(run)
            {
                if(cr.msgQueue.IsEmpty) // we only need to send messages when there are messages in the queue
                {
                    Thread.Sleep(500);
                    continue;
                }
                else // if there are messages in the queue then go through each message and send it to each connection
                {
                    foreach (string msg in repo.msgQueue) // send all the message that are in the queue 
                    {
                        foreach (KeyValuePair<string, Connection> entry in repo.repo) // send message to all other clients
                        {
                            if (entry.Key != entry.Value.Name) // don't send the message to the sender
                            {
                                string recMsg = "";
                                TcpClient tmpClient = new TcpClient(); // server acts like client and connects to client's listener thread
                                tmpClient.Connect(entry.Value.IP, 23000); // connect to client's IP and port
                                NetworkStream tmpStream = tmpClient.GetStream(); // get stream to client
                                entry.Value.Send(msg, tmpStream); // Send the message as a reply to the client

                                recMsg = entry.Value.Receive(tmpStream);
                                entry.Value.Parse(recMsg, entry.Value); /// parse the received message, which will be an ack

                                tmpStream.Close();
                                tmpClient.Close();

                                if(entry.Value.ShutDown == true) // if the shut down command is given, finish sending all messages
                                {                                // then stop the server.
                                    run = false;
                                }
                            }

                        }
                    }
                }
            }
        }
    }
}
