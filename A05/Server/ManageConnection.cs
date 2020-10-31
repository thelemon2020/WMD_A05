using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ManageConnection
    {
        ConnectRepo repo;
        


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
            string recMsg, ackMsg, sendMsg, tmpMsg;
            Connection clientConnection = new Connection(client, repo); // create a new connection class for each client thread
            NetworkStream stream = client.GetStream(); // Open the network stream to the current client thread.

            recMsg = clientConnection.Receive(stream); // get the communication from the client
            tmpMsg = clientConnection.Parse(recMsg);

            if(tmpMsg.StartsWith("ACK,OK")) // The acknowledgement message is only returned with new connection
            {
                ackMsg = tmpMsg;
                clientConnection.Send(ackMsg, stream); // send acknowledgement of first connection
                stream.Close();
            }
            else if(tmpMsg.StartsWith("REPLY")) // If the message was sent from a client to chat it will be sent out as reply
            {
                sendMsg = tmpMsg;
                clientConnection.Send(clientConnection.AckMsg, stream); // send the acknowledgement that the message was received
                foreach(KeyValuePair<string, TcpClient> entry in repo.repo) // send message to all other clients
                {
                    if (entry.Key != clientConnection.Name) // don't send the message to the sender
                    {
                        NetworkStream tmpStream = entry.Value.GetStream();
                        clientConnection.Send(sendMsg, tmpStream);
                        tmpStream.Close();
                    }

                }
            }
        }
    }
}
