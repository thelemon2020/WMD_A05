/*
* FILE : InteractWithServer.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the InteractWithServer class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace A05
{
  /*
   * NAME : InteractWithServer
   * PURPOSE : This defines the InteractWithServer class.  It is a static class that holds methods that help other parts of the client
   *            interact with the server, through sending and receiving data
   */
   static public class InteractWithServer
    {   
       /*
        * METHOD : connectToServer()
        *
        * DESCRIPTION : This method takes an IP and Port and opens a connection to that server and returns a stream to that server
        *
        * PARAMETERS : serverIp - IP of the server
        *              serverPort - Port of the server
        *
        * RETURNS : serverStream - the network stream to the server
        */
        static public NetworkStream connectToServer(IPAddress serverIP, int serverPort)
        {
            NetworkStream serverStream = null;
            TcpClient server = new TcpClient();
            server.Connect(serverIP, serverPort);// establish a connection with the server and get the data stream
            serverStream = server.GetStream();
            return serverStream;
        }
        /*
        * METHOD :writeToServer()
        *
        * DESCRIPTION : This method takes a network stream and sends a protocol through it
        *
        * PARAMETERS : serverStream - the network stream to write to
        *              protocol - the string to send to the server
        *
        * RETURNS : Nothing
        */
        static public void writeToServer(NetworkStream serverStream, string protocol)
        {
            byte[] dataSent = Encoding.ASCII.GetBytes(protocol.ToString()); // package the data to send into bytes for the stream           
            serverStream.Write(dataSent, 0, dataSent.Length); // write the bytes to the stream
        }
        /*
       * METHOD :readFromServer()
       *
       * DESCRIPTION : This method receives data from a network stream
       *
       * PARAMETERS : serverStream - the network stream to read from
       *
       * RETURNS : receivedData - the return string from the server
       */
        static public string readFromServer(NetworkStream serverStream)
        {
            string recievedData = null;
            byte[] rawData = null;
            rawData = new byte[1024]; // buffer that the bytes of response data are stored into from the stream read
            int bytesRec = serverStream.Read(rawData, 0, rawData.Length); // need to know how many bytes were received to decode to ascii
            recievedData += Encoding.ASCII.GetString(rawData, 0, bytesRec);// decode the response to a string
            return recievedData; // return the response
        }
       /*
        * METHOD :closeConnection()
        *
        * DESCRIPTION : This method closes the stream
        *
        * PARAMETERS : serverStream - the network stream to close
        *
        * RETURNS :  nothing
        */
        static public void closeConnection(NetworkStream serverStream)
        {
            serverStream.Close();
        }
    }
}
