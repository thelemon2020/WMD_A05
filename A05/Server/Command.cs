using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Command
    {
        public string Name { get; set; }
        public TcpClient Client { get; set; }

        public string Receive()
        {
            NetworkStream clientStream = Client.GetStream();
            byte[] rawData = new byte[1024];
            string message = "";
            int bytesRec;

            bytesRec = clientStream.Read(rawData, 0, rawData.Length);
            clientStream.Flush();
            clientStream.Close();
            message += Encoding.ASCII.GetString(rawData, 0, bytesRec);

            return message;
        }
    }
}
