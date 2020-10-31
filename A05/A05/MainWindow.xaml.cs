using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isConnected { get; set; }
        public connection currentConnection { get; set; }

        public Mutex canWrite { get; set; }


        public MainWindow()
        {
            canWrite = new Mutex();
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {

            ConnectWindow newConnection = new ConnectWindow();
            newConnection.Owner = this;
            newConnection.ShowDialog();
            currentConnection = new connection(newConnection.username, newConnection.userPassword,
                newConnection.ipAddress, newConnection.port, canWrite);
            if (currentConnection != null)
            {
                StartConnectionCommand startUp = new StartConnectionCommand(currentConnection, canWrite);
                string serverResponse = startUp.ExecuteCommand();
                if (serverResponse != "")
                {
                    isConnected = true;

                    string connectedMessage = string.Format("Connected to server at {0}", currentConnection.ipAddress.ToString());
                    chatWindow.Text += connectedMessage;
                    userList.Text += currentConnection.username;
                    userInput.IsEnabled = true;
                    SubmitMessage.IsEnabled = true;
                    ParameterizedThreadStart listenStartThread = new ParameterizedThreadStart(listenForMessage);
                    Thread listenThread = new Thread(listenStartThread);
                    object[] paramObj = new object[2];
                    listenThread.Start(currentConnection);
                }
                else
                {
                    chatWindow.Text += serverResponse;
                }
            }
            newConnection.Close();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            if (userInput.Text != "")
            {
                SendMessageCommand messageToSend = new SendMessageCommand(currentConnection, userInput.Text, canWrite);
                messageToSend.ExecuteCommand();
            }
        }

        private void listenForMessage(object obj)
        {
            connection server = (connection)obj;
            while (isConnected == true)
            {
                canWrite.WaitOne();
                NetworkStream serverStream = null;
                serverStream = InteractWithServer.connectToServer(currentConnection.ipAddress, currentConnection.serverPort);
                string serverResponse = InteractWithServer.readFromServer(serverStream);
                string[] arguments = serverResponse.Split(',');
                AckCommand sendBack = null;
                if (arguments[0] == "REPLY")
                {
                    userInput.Text += arguments[1];
                    sendBack = new AckCommand(currentConnection, "ACK,OK,<EOF>");
                    InteractWithServer.writeToServer(serverStream, sendBack.protocol.ToString());
                }
                else if (arguments[0] == "DISCONNECT")
                {
                    sendBack = new AckCommand(currentConnection, "ACK,OK,<EOF>");
                    InteractWithServer.writeToServer(serverStream, sendBack.protocol.ToString());
                    shutDownServer();
                }
                else
                {
                    sendBack = new AckCommand(currentConnection, "ACK,FAIL,<EOF>");
                    InteractWithServer.writeToServer(serverStream, sendBack.protocol.ToString());
                }
                InteractWithServer.closeConnection(serverStream);
                canWrite.ReleaseMutex();
                Thread.Sleep(5000);
            }
        }
        private void disconnectFromServer(object sender, RoutedEventArgs e)
        {
            shutDownServer();
        }
        public void shutDownServer()
        {
            isConnected = false;
            currentConnection = null;
            userInput.Text = "";
            userInput.IsEnabled = false;
            MenuDisconnect.IsEnabled = false;
            userList.Text = "";
        }
    }
}
