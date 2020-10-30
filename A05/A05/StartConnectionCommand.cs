using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A05
{
    public class StartConnectionCommand : Command
    {
        public string userName { get; set; }
        public byte[] password { get; set; }

        public StartConnectionCommand(connection newConnection)
        {
            password = hashPassword(newConnection.userPassword);
            userName = newConnection.username;
            serverIP = newConnection.ipAddress;
            serverPort = newConnection.serverPort;
            command = "CONNECT";
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

            protocol.AppendLine(command + " " + userName + " " + password);
         
        }
    }
}
