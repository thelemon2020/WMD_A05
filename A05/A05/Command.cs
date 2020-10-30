using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A05
{
    public class Command
    {
        public string header { get; set; }
        public string command { get; set; }
        public string footer { get; set; }
        public IPAddress serverIP { get; set; }
        public int serverPort { get; set; }
        public StringBuilder protocol { get; set; }
        
        public string ExecuteCommand()
        {
            TcpClient server = new TcpClient();
            server.Connect(serverIP, serverPort);// establish a connection with the server and get the data stream
            NetworkStream serverStream = server.GetStream();
            SendToServer(serverStream);
            string serverResponse = ReceiveFromServer(serverStream);
            serverStream.Close();
            server.Close();
            return serverResponse;
        }
        public void SendToServer(NetworkStream serverStream)
        {
            byte[] dataSent = Encoding.ASCII.GetBytes(protocol.ToString()); // package the data to send into bytes for the stream           
            serverStream.Write(dataSent, 0, dataSent.Length); // write the bytes to the stream
        }

        public string ReceiveFromServer(NetworkStream serverStream)
        {
            string recievedData = null;
            byte[] rawData = null;
            rawData = new byte[1024]; // buffer that the bytes of response data are stored into from the stream read
            int bytesRec = serverStream.Read(rawData, 0, rawData.Length); // need to know how many bytes were received to decode to ascii
            recievedData += Encoding.ASCII.GetString(rawData, 0, bytesRec);// decode the response to a string
            return recievedData; // return the response
        }
    }

  
}
