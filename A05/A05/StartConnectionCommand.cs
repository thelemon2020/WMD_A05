/*
* FILE : StartConnectionCommand.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 04 
* REVISED ON : 2020 - 11 - 04
* DESCRIPTION : This file defines the ShutDown Command Class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    /*
    * NAME : StartConnectionCommand
    * PURPOSE : This defines the StartConnection Command Class.  It inherits from the Command class.  It's purpose is to provide a protocol 
    *           to initiate the first connection between client and server
    */
    public class StartConnectionCommand : Command
    {
        public string password { get; set; }
        public IPAddress ipAddress { get; set; }

        public StartConnectionCommand(connection newConnection, IPAddress ip)
        {
            password = hashPassword(newConnection.userPassword);
            userName = newConnection.username;
            serverIP = newConnection.ipAddress;
            serverPort = newConnection.serverPort;
            command = "CONNECT";
            ipAddress = ip;

            createProtocol();
        }

        public StartConnectionCommand(connection newConnection, IPAddress ip, string userCommand)
        {
            password = hashPassword(newConnection.userPassword);
            userName = newConnection.username;
            serverIP = newConnection.ipAddress;
            serverPort = newConnection.serverPort;
            command = userCommand;
            ipAddress = ip;
         
            createProtocol();
        }
       /*
        * METHOD : hashPassword()
        *
        * DESCRIPTION : This method hashes the user's password for security purpose
        *
        * PARAMETERS : password - the password to hash
        *
        * RETURNS : hashString - the characters that make up the newly hashed password
        */
        private string hashPassword(string password)
        {
            MD5 hasher = MD5.Create();
            byte[] originalPass = new byte[1024];
            byte[] hashPass = new byte[1024];
            string hashString = "";
            originalPass = Encoding.ASCII.GetBytes(password);
            hashPass = hasher.ComputeHash(originalPass);
            hashString += Encoding.ASCII.GetString(hashPass, 0, hashPass.Length);
            return hashString;
        }
       /*
        * METHOD : createProtocol()
        *
        * DESCRIPTION : This creates the string that will be converted to bytes and sent to the server
        *
        * PARAMETERS : None
        *
        * RETURNS : Nothing
        */
        private void createProtocol()
        {
            protocol = new StringBuilder();
            protocol.Append(command);
            protocol.Append(',');
            protocol.Append(userName);
            protocol.Append(',');
            protocol.Append(password);
            protocol.Append(',');
            protocol.Append("<EOF>");
        }
    }
}
