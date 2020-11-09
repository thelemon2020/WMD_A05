using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Connection
    {
        private const int kOK = 1;
        private const int kFail = 0;
        public string Name { get; set; }
        public string IP { get; set; }
        public string AckMsg { get; set; }
        public string ReplyMsg { get; set; }
        public bool ShutDown { get; set; }
        public string Password { private get; set; }
        public string[] MsgLog { get; set; }

        private readonly object lockObj;

        ConnectRepo repo;
        FileHandler fh = new FileHandler();

        public Connection(ConnectRepo cr, object obj)
        {
            repo = cr;
            ShutDown = false;
            lockObj = obj;
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


        public void Parse(string recMsg, Connection c)
        {
            //Delegate which resulting command is necessary
            string[] splitMsg = recMsg.Split(',');

            if(splitMsg[0] == "REGISTER") // Register command sent by user
            {
                AckCommand ack = new AckCommand();
                Name = splitMsg[2];
                Password = splitMsg[3];
                if(!fh.CheckExist(Name, Password)) // if the user doesn't exist yet, create an entry for them on the file
                {
                    fh.WriteCredentials(Name + "," + Password);
                    AckMsg = ack.BuildProtocol(kOK); // send an acknowledgment back
                }
                else // If the user exists already, then send back a NACK
                {
                    AckMsg = ack.BuildProtocol(kFail);
                }
            }
            else if(splitMsg[0] == "CONNECT")
            {
                // delegate the AckCommand
                AckCommand ackOK = new AckCommand();
                IP = splitMsg[1]; // parse the IP address of the client 
                Name = splitMsg[2]; // get the name from the incoming connect message
                Password = splitMsg[3];

                //if(fh.CheckExist(Name, Password)) // if the user exists and has been registered they can connect
                //{
                //    MsgLog = fh.ReadLog();
                //    AckMsg = ackOK.BuildProtocol(kOK); // build the acknowledgement 

                //}
                //else
                //{
                //    AckMsg = ackOK.BuildProtocol(kFail);
                //}
                AckMsg = ackOK.BuildProtocol(kOK);
                repo.Add(Name, c); // Add the new client into the repo
            }
            else if(splitMsg[0] == "SEND")
            {
                // delegate the ReplyCommand
                ReplyCommand reply = new ReplyCommand();
                AckCommand ackOK = new AckCommand();
                AckMsg = ackOK.BuildProtocol(kOK); // still need to send an ok acknowledgement that message was received
                string tmpMsg = reply.CheckMessage(splitMsg); // Since we split on commas, rebuild the message to not be split
                ReplyMsg = reply.BuildProtocol(tmpMsg); // build the reply
                repo.AddMsg(ReplyMsg); // Add the message that came in to the queue to be sent
            }
            else if(splitMsg[0] == "ACK")
            {
                return; // If the client sends an acknowledgement of reply received, don't need to do anything as of now
            }
            else if(splitMsg[0] == "DISCONNECT") // if a super user sends the server shut off command
            {
                repo.Remove(Name);
            }
            else if(splitMsg[0] == "SHUTDOWN")
            {
                ShutDown = true;
            }
            else
            {
                // delegate Ack fail
                AckCommand ackFail = new AckCommand();
                AckMsg = ackFail.BuildProtocol(kFail);
            }
        }
    }
}
