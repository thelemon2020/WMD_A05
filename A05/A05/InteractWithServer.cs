using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace A05
{
   static public class InteractWithServer
    {
        static public NetworkStream connectToServer(IPAddress serverIP, int serverPort)
        {
            NetworkStream serverStream = null;
            TcpClient server = new TcpClient();
            server.Connect(serverIP, serverPort);// establish a connection with the server and get the data stream
            serverStream = server.GetStream();
            return serverStream;
        }
        static public void writeToServer(NetworkStream serverStream, string protocol)
        {
            byte[] dataSent = Encoding.ASCII.GetBytes(protocol.ToString()); // package the data to send into bytes for the stream           
            serverStream.Write(dataSent, 0, dataSent.Length); // write the bytes to the stream
        }
        static public string readFromServer(NetworkStream serverStream)
        {
            string recievedData = null;
            byte[] rawData = null;
            rawData = new byte[1024]; // buffer that the bytes of response data are stored into from the stream read
            int bytesRec = serverStream.Read(rawData, 0, rawData.Length); // need to know how many bytes were received to decode to ascii
            recievedData += Encoding.ASCII.GetString(rawData, 0, bytesRec);// decode the response to a string
            return recievedData; // return the response
        }
        static public void closeConnection(NetworkStream serverStream)
        {
            serverStream.Close();
        }
    }
}
