using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Connection
    {
        private const int kOK = 1;
        private const int kFail = 0;
        public string Name { get; set; }
        public TcpClient Client { get; set; }
        public string AckMsg { get; set; }
        public string ReplyMsg { get; set; }
        public string Password { private get; set; }

        ConnectRepo repo;

        public Connection(TcpClient client, ConnectRepo cr)
        {
            Client = client;
            repo = cr;
        }

        public void Send(string msg, NetworkStream stream)
        {
            //package the message up into bytes, and then send on the current client stream
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
            stream.Write(msgBytes, 0, msgBytes.Length);
            stream.Flush();
        }

        public string Receive(NetworkStream stream)
        {
            //receive a communication from the client, and unpackage it to a string
            byte[] incomingData = new byte[1024];
            int bytesRec = 0;
            string msgRec = "";

            bytesRec = stream.Read(incomingData, 0, incomingData.Length);
            msgRec += Encoding.ASCII.GetString(incomingData, 0, bytesRec);
            stream.Flush();

            return msgRec;
        }

        public string Parse(string recMsg)
        {
            //Delegate which resulting command is necessary
            string[] splitMsg = recMsg.Split(',');

            if(splitMsg[0] == "CONNECT")
            {
                // delegate the AckCommand
                AckCommand ackOK = new AckCommand();
                Name = splitMsg[1]; // get the name from the incoming connect message
                Password = splitMsg[2]; // NEED TO ADD PASSWORD TO FILE
                AckMsg = ackOK.BuildProtocol(kOK); // build the acknowledgement 
                repo.Add(Name, Client); // Add the new client into the repo
                return AckMsg;
            }
            else if(splitMsg[0] == "SEND")
            {
                // delegate the ReplyCommand
                ReplyCommand reply = new ReplyCommand();
                AckCommand ackOK = new AckCommand();
                AckMsg = ackOK.BuildProtocol(kOK); // still need to send an ok acknowledgement that message was received
                string tmpMsg = reply.CheckMessage(splitMsg); // Since we split on commas, rebuild the message to not be split
                Name = splitMsg[1]; // get the name from the message header
                ReplyMsg = reply.BuildProtocol(Name, tmpMsg); // build the reply
                repo.AddMsg(ReplyMsg); // Add the message that came in to the queue to be sent
                return ReplyMsg;
            }
            else
            {
                // delegate Ack fail
                AckCommand ackFail = new AckCommand();
                AckMsg = ackFail.BuildProtocol(kFail);
                return AckMsg;
            }
        }
    }
}
