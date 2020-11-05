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
            string recMsg, ackMsg, tmpMsg;
            Connection clientConnection = new Connection(repo); // create a new connection class for each client thread
            NetworkStream stream = client.GetStream(); // Open the network stream to the current client thread.
            
            recMsg = clientConnection.Receive(stream); // get the communication from the client
            tmpMsg = clientConnection.Parse(recMsg, clientConnection);

            if(tmpMsg.StartsWith("ACK,OK")) // The acknowledgement message is only returned with new connection
            {
                ackMsg = tmpMsg;
                clientConnection.Send(ackMsg, stream); // send acknowledgement of first connection
                stream.Close();
            }
            else if(tmpMsg.StartsWith("REPLY")) // If the message was sent from a client to chat it will be sent out as reply
            {
                
                clientConnection.Send(clientConnection.AckMsg, stream); // send the acknowledgement that the message was received

            }
        }

        public void SendReplies(ConnectRepo cr)
        {
            string recMsg, tmpMsg;

            while(run)
            {
                foreach (string msg in repo.msgQueue) // send all the message that are in the queue 
                {
                    foreach (KeyValuePair<string, Connection> entry in repo.repo) // send message to all other clients
                    {
                        if (entry.Key != entry.Value.Name) // don't send the message to the sender
                        {
                            TcpClient tmpClient = new TcpClient(); // server acts like client and connects to client's listener thread
                            tmpClient.Connect(entry.Value.IP, 23000); // connect to client's IP and port
                            NetworkStream tmpStream = tmpClient.GetStream(); // get stream to client
                            entry.Value.Send(msg, tmpStream); // Send the message as a reply to the client

                            recMsg = entry.Value.Receive(tmpStream);
                            tmpMsg = entry.Value.Parse(recMsg, entry.Value); // consider doing something with this... but I'm not sure yet

                            tmpStream.Close();
                            tmpClient.Close();
                        }

                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
