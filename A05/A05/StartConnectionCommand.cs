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
    public class StartConnectionCommand : Command
    {
        public byte[] password { get; set; }
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

        private byte[] hashPassword(string password)
        {
            MD5 hasher = MD5.Create();
            byte[] originalPass = new byte[1024];
            byte[] hashPass = new byte[1024];
            originalPass = Encoding.ASCII.GetBytes(password);
            hashPass = hasher.ComputeHash(originalPass);
            return hashPass;
        }

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
