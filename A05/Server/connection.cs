using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Connection
    {
        private const int kRegistered = 0;
        private const int kNotRegister = 1;
        private const int kIncomplete = 2;
        private const int kBadPermisson = 3;
        private const int kError = 4;
        private const int kOK = 1;
        private const int kNormalUser = 0;
        private const int kSuperUser = 1;
        public string Name { get; set; }
        public IPAddress IP { get; set; }
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
            try
            {
                stream.Write(msgBytes, 0, msgBytes.Length);
                stream.Flush();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        

        public string Receive(NetworkStream stream)
        {
            //receive a communication from the client, and unpackage it to a string
            byte[] incomingData = new byte[1024];
            int bytesRec = 0;
            string msgRec = "";

            try
            {
                bytesRec = stream.Read(incomingData, 0, incomingData.Length);
                msgRec += Encoding.ASCII.GetString(incomingData, 0, bytesRec);
                stream.Flush();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            return msgRec;
        }


        public void Parse(string recMsg, Connection c)
        {
            //Delegate which resulting command is necessary
            string[] splitMsg = recMsg.Split(',');

            if(splitMsg[splitMsg.Length - 1] != "<EOF>")
            {
                NackCommand nack = new NackCommand();
                AckMsg = nack.BuildProtocol(kIncomplete);
            }
            else if(splitMsg[0] == "REGISTER") // Register command sent by user
            {
                Name = splitMsg[1];
                Password = splitMsg[2];
                lock(lockObj)
                {
                    if (!fh.CheckExist(Name, Password)) // if the user doesn't exist yet, create an entry for them on the file
                    {
                        AckCommand ack = new AckCommand();
                        fh.WriteCredentials(Name + "," + Password);
                        AckMsg = ack.BuildProtocol(); // send an acknowledgment back
                        repo.AddMsg("DISCONNECT,<EOF>");
                    }
                    else // If the user exists already, then send back a NACK
                    {
                        NackCommand nack = new NackCommand();
                        AckMsg = nack.BuildProtocol(kRegistered);
                    }
                }
            }
            else if(splitMsg[0] == "CONNECT")
            {
                Name = splitMsg[1]; // get the name from the incoming connect message
                Password = splitMsg[2];

                lock (lockObj)
                {
                    if (fh.CheckExist(Name, Password)) // if the user exists and has been registered they can connect
                    {
                        AckCommand ack = new AckCommand();
                        repo.Add(Name, c); // Add the new client into the repo
                        if(fh.IsSuper(Name+","+Password))
                        {
                            AckMsg = ack.BuildProtocol(kSuperUser, repo); // build the acknowledgement for super user
                        }
                        else
                        {
                            AckMsg = ack.BuildProtocol(kNormalUser, repo); // build the acknowledgement for normal user
                        }

                    }
                    else
                    {
                        NackCommand nack = new NackCommand();
                        AckMsg = nack.BuildProtocol(kNotRegister);
                    }
                }
            }
            else if(splitMsg[0] == "SEND")
            {
                // delegate the ReplyCommand
                ReplyCommand reply = new ReplyCommand();
                AckCommand ackOK = new AckCommand();
                Name = splitMsg[1];
                AckMsg = ackOK.BuildProtocol(); // still need to send an ok acknowledgement that message was received
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
                Name = splitMsg[1];
                repo.Remove(Name);
                AckCommand ack = new AckCommand();
                AckMsg = ack.BuildProtocol();
            }
            else if(splitMsg[0] == "SHUTDOWN")
            {
                Name = splitMsg[1];
                Password = splitMsg[2];
                bool isSuper = fh.IsSuper(Name + "," + Password);
                if(isSuper)
                {
                    AckCommand ack = new AckCommand();
                    AckMsg = ack.BuildProtocol();
                    ShutDown = true;
                    
                }
                else
                {
                    NackCommand nack = new NackCommand();
                    AckMsg = nack.BuildProtocol(kBadPermisson);
                }
            }
            else
            {
                // delegate Ack fail
                NackCommand nack = new NackCommand();
                AckMsg = nack.BuildProtocol(kError);
            }
        }
    }
}
