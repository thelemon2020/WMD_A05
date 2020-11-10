//*********************************************
// File			 : Connection.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, 8656317
// Last Change   : 2020-11-09
// Description	 : A connection class that holds relevant details about the user, as well as methods to send, receive,
//               : and parse data that has come in. It serves to keep all client data in one place
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Connection
    {
        // all constants and properties used
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
        public int Port { get; set; }
        public string AckMsg { get; set; }
        public string ReplyMsg { get; set; }
        public bool ShutDown { get; set; }
        public string Password { private get; set; }

        private readonly object lockObj;

        ConnectRepo repo;
        FileHandler fh = new FileHandler();


        /////////////////////////////////////////
        // Method       : Connection (ctor)
        // Description  : The Connection class ctor, this initializes some values and grabs the reference to the repo so that
        //              : the parser can add details or grab details where necessary
        // Parameters   : ConnectRepo cr : the repo class that holds user details and messages
        //              : object obj : the object used in the mutex when accessing files to see if a user is registered
        // Returns      : N/A
        /////////////////////////////////////////
        public Connection(ConnectRepo cr, object obj)
        {
            repo = cr;
            ShutDown = false;
            lockObj = obj;
        }


        /////////////////////////////////////////
        // Method       : Send
        // Description  : A function to send data back to client through their stream
        // Parameters   : string msg : the message to send to the client
        //              : NetworkStream stream : The client's network stream
        // Returns      : N/A
        /////////////////////////////////////////
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


        /////////////////////////////////////////
        // Method       : Receive
        // Description  : A method to receive incoming communication from the client
        // Parameters   : networkStream stream : The network stream of the client
        // Returns      : string msgRec : The message received from the client
        /////////////////////////////////////////
        public string Receive(NetworkStream stream)
        {
            //receive a communication from the client, and unpackage it to a string
            byte[] incomingData = new byte[1024];
            int bytesRec = 0;
            string msgRec = "";

            try
            {
                bytesRec = stream.Read(incomingData, 0, incomingData.Length); // read from the stream
                msgRec += Encoding.ASCII.GetString(incomingData, 0, bytesRec); // convert to string
                stream.Flush();
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

            return msgRec;
        }


        /////////////////////////////////////////
        // Method       : Parse
        // Description  : A method which takes input and depending on the content delegates different tasks
        // Parameters   : string recMsg : the string received from the user
        //              : Connection c : the current client connection class
        // Returns      : N/A
        /////////////////////////////////////////
        public void Parse(string recMsg, Connection c)
        {
            //Delegate which resulting command is necessary
            string[] splitMsg = recMsg.Split(',');

            if(splitMsg[splitMsg.Length - 1] != "<EOF>") // If the data sent in the stream is incomplete for some reason
            {
                NackCommand nack = new NackCommand();
                AckMsg = nack.BuildProtocol(kIncomplete); // Build a NACK to send back
            }
            else if(splitMsg[0] == "REGISTER") // If the user wants to register
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
                        Console.WriteLine("{0} Has Registered", Name);
                    }
                    else // If the user exists already, then send back a NACK
                    {
                        NackCommand nack = new NackCommand();
                        AckMsg = nack.BuildProtocol(kRegistered);
                    }
                }
            }
            else if(splitMsg[0] == "CONNECT") // If the user wants to make initial connection to the server
            {
                Name = splitMsg[1]; // get the name from the incoming connect message
                Password = splitMsg[2];
                lock (lockObj) // since these actions involve a file, lock it
                {
                    Port = fh.ClientCount(); // get the port that client listener will use
                    fh.UpdateClientLog(Port); // update the log so that the next user gets a different port
                    if (fh.CheckExist(Name, Password)) // if the user exists and has been registered they can connect
                    {
                        AckCommand ack = new AckCommand();
                        repo.Add(Name, c); // Add the new client into the repo
                        if(fh.IsSuper(Name+","+Password))
                        {
                            AckMsg = ack.BuildProtocol(kSuperUser, repo, c); // build the acknowledgement for super user
                        }
                        else
                        {
                            AckMsg = ack.BuildProtocol(kNormalUser, repo, c); // build the acknowledgement for normal user
                        }
                        Console.WriteLine("User {0} Connected", Name);
                    }
                    else
                    {
                        NackCommand nack = new NackCommand(); // if the user has not registered, send them back a NACK
                        AckMsg = nack.BuildProtocol(kNotRegister);
                    }
                }
            }
            else if(splitMsg[0] == "SEND") // If the user sends a message to be displayed in chat
            {
                // delegate the ReplyCommand
                ReplyCommand reply = new ReplyCommand();
                AckCommand ackOK = new AckCommand();
                Name = splitMsg[1];
                AckMsg = ackOK.BuildProtocol(); // still need to send an ok acknowledgement that message was received
                string tmpMsg = reply.CheckMessage(splitMsg); // Since we split on commas, rebuild the message to not be split if it had commas in it
                ReplyMsg = reply.BuildProtocol(tmpMsg); // build the reply
                repo.AddMsg(ReplyMsg); // Add the message that came in to the queue to be sent
                Console.WriteLine("{0} Sent: {1}", Name, ReplyMsg);
            }
            else if(splitMsg[0] == "ACK")
            {
                return; // If the client sends an acknowledgement of reply received, don't need to do anything
            }
            else if(splitMsg[0] == "DISCONNECT") // the user wants to disconnect from the server
            {
                Name = splitMsg[1];
                repo.Remove(Name); // remove the user
                AckCommand ack = new AckCommand();
                AckMsg = ack.BuildProtocol(); // send ack of command received ok
                Console.WriteLine("User {0} Disconnected", Name);
            }
            else if(splitMsg[0] == "SHUTDOWN") // if a super user sends the server shut off command
            {
                Name = splitMsg[1];
                Password = splitMsg[2];
                bool isSuper = fh.IsSuper(Name + "," + Password); // make sure the user is the admin
                if(isSuper) // if the user is the admin set the shutdown property to true
                {
                    AckCommand ack = new AckCommand();
                    AckMsg = ack.BuildProtocol();
                    ShutDown = true;
                    Console.WriteLine("{0} Shutdown the Server");
                    lock(lockObj) // clear the client log so the first client to join starts at 35000 for their listener port again
                    {
                        fh.ClearClientLog();
                    }
                }
                else // if the user is not the admin, then send a NACK
                {
                    NackCommand nack = new NackCommand();
                    AckMsg = nack.BuildProtocol(kBadPermisson);
                }
            }
            else // if any other kind of data comes through and is unrecognized, send a NACK
            {
                NackCommand nack = new NackCommand();
                AckMsg = nack.BuildProtocol(kError);
            }
        }
    }
}
