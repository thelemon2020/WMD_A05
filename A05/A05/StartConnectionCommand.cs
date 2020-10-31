using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A05
{
    public class StartConnectionCommand : Command
    {
        public string userName { get; set; }
        public byte[] password { get; set; }

        public StartConnectionCommand(connection newConnection, Mutex toWrite)
        {
            password = hashPassword(newConnection.userPassword);
            userName = newConnection.username;
            serverIP = newConnection.ipAddress;
            serverPort = newConnection.serverPort;
            command = "CONNECT";
            waitToWrite = toWrite;
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
            protocol.Append(",");
            protocol.Append(userName);
            protocol.Append(",");
            protocol.Append(password);  
        }
    }
}
