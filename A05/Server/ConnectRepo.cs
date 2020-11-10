//*********************************************
// File			 : ConnectRepo.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, Chris Lemon
// Last Change   : 2020-11-09
// Description	 : A class that acts as a repo for all collections. The two collections used are a concurrent dictionary to hold
//               : User's and their details, as well as a concurrent queue to hold outgoing messages.
//*********************************************


using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Net;
using System.IO;

namespace Server
{
    //******************************************
    // Name     : ConnectRepo
    // Purpose  : A repository for connections and messages so that other parts of the system have easy access to add, update,
    //          : or remove client connection data and messages.
    //******************************************
    public class ConnectRepo
    {
        public ConcurrentDictionary<string, Connection> repo; // holds the client connection class, key is the client name
        public ConcurrentQueue<string> msgQueue; // holds all outgoing messages


        /////////////////////////////////////////
        // Method       : ConnectRepo (ctor)
        // Description  : Initializes new instances of the dictionary and queue
        // Parameters   : N/A
        // Returns      : N/A
        /////////////////////////////////////////
        public ConnectRepo()
        {
            repo = new ConcurrentDictionary<string, Connection>();
            msgQueue = new ConcurrentQueue<string>();
        }


        /////////////////////////////////////////
        // Method       : Add
        // Description  : A function to added a new client to the dictionary. Also adds a message of the client's name to send
        //              : to the client to update all clients' user list
        // Parameters   : string key : The user's name is the key
        //              : Connection c : The connection class holding the user's details
        // Returns      : N/A
        /////////////////////////////////////////
        public void Add(string key, Connection c)
        {
            repo.TryAdd(key, c); // add to dictionary
            string addMessage = "ADD," + key + ",<EOF>"; // Create a message so clients know to add the user to their list
            AddMsg(addMessage); // Add the message to the out going message queue
        }


        /////////////////////////////////////////
        // Method       : Remove
        // Description  : A method to remove a user from the dictionary when they want to disconnect
        // Parameters   : string key : the user's name is used as the key to remove them from the dictionary
        // Returns      : N/A
        /////////////////////////////////////////
        public void Remove(string key)
        {
            repo.TryRemove(key, out Connection c);
            string removeMessage = "REMOVE," + key + ",<EOF>"; // a message to the clients to remove that person from their user list
            AddMsg(removeMessage);
        }


        /////////////////////////////////////////
        // Method       : AddMsg
        // Description  : A method to add an outgoing message to the message queue to be sent to all client
        // Parameters   : string msg : the message to add to the queue
        // Returns      : N/A
        /////////////////////////////////////////
        public void AddMsg(string msg)
        {
            msgQueue.Enqueue(msg);
        }
    }
}
