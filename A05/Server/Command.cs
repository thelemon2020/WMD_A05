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
        public string Header { get; set; }
        public string Message { get; set; }
        public string Password { get; set; } // consider making private
        private const string footer = "<EOF>";
        public StringBuilder protocol;

        public Command()
        {
            protocol = new StringBuilder();
        }

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

        public void Send(string msg)
        {
            byte[] dataSent = Encoding.ASCII.GetBytes(msg);
            NetworkStream clientStream = Client.GetStream();
            clientStream.Write(dataSent, 0, dataSent.Length);
            clientStream.Flush();
            clientStream.Close();
        }

        public string Parse(string msg, ConnectRepo repo)
        {
            string[] segments = msg.Split(',');

            if (segments[0] == "CONNECT")
            {
                if (segments[1] != null)
                {
                    Name = segments[1];
                    Password = segments[2];
                    Header = "ACK";
                    Message = "OK";
                }
                else
                {
                    Header = "ACK";
                    Message = "FAIL";
                }
            }
            else if (segments[0] == "SEND")
            {
                string tmpMsg = "REPLY,";
                int endIndex = 0;
                for (int i = 0; i < segments.Length; i++)
                {
                    if(segments[i] == "<EOF>")
                    {
                        endIndex = i;
                    }
                }
                tmpMsg += Name + ",";
                for (int i = 1; i < endIndex; i++)
                {
                    tmpMsg += segments[i];
                }
                tmpMsg += "," + footer;
                repo.AddMsg(tmpMsg);

                Header = "ACK";
                Message = "OK"; 
            }

            protocol.Append(Header);
            protocol.Append(",");
            protocol.Append(Message);
            protocol.Append(",");
            protocol.Append(footer);

            string tmp = protocol.ToString();
            protocol.Clear();
            return tmp;
        }
    }
}
