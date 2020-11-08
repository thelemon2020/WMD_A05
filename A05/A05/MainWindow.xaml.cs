using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Hosting;
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

        delegate void MyCallback(Object obj);


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ConnectWindow newConnection = new ConnectWindow();
            newConnection.Owner = this;
            newConnection.ShowDialog();
            if (newConnection.canProceed == true)
            {
                currentConnection = new connection(newConnection.username, newConnection.userPassword,
                    newConnection.ipAddress, newConnection.port);
                if (currentConnection != null)
                {
                    StartConnectionCommand startUp = new StartConnectionCommand(currentConnection, currentConnection.ipAddress);
                    string serverResponse = startUp.ExecuteCommand();
                    if (serverResponse != "")
                    {
                        isConnected = true;

                        string connectedMessage = string.Format("Connected to server at {0}\n", currentConnection.ipAddress.ToString());
                        chatWindow.Text += connectedMessage;
                        userList.Text += currentConnection.username;
                        userInput.IsEnabled = true;
                        SubmitMessage.IsEnabled = true;
                        Thread checkForNewMessages = new Thread(listenForMessages);
                        checkForNewMessages.Start();
                    }
                    else
                    {
                        chatWindow.Text += serverResponse;
                    }
                }
            }
            newConnection.Close();
        }
        private void listenForMessages()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 35000);
            startServer(listener);
        }
        private void startServer(TcpListener listener)
        {
            listener.Start();
            while (isConnected == true)
            {
                if (!listener.Pending())
                {
                    Thread.Sleep(1000);
                    continue;
                }
                TcpClient client = listener.AcceptTcpClient(); // Accept a new client
                handleConnection(client);
                client.Close();
            }
            listener.Stop();
        }
        private void handleConnection(TcpClient client)
        {
            NetworkStream readStream = client.GetStream();
            string[] incomingData = InteractWithServer.readFromServer(readStream).Split(',');
            updateUI(incomingData);
            AckCommand acknowledge = new AckCommand("ACK,<EOF>");
            InteractWithServer.writeToServer(readStream, acknowledge.protocol.ToString());
            readStream.Close();
        }
        private void updateUI(string[] arguments)
        {                    
            if (arguments[0] == "REPLY")
            {
                changeChatWindow(arguments[1]);
            }
            else if (arguments[0] == "DISCONNECT")
            { 
                shutDownServer();
            }
            else if (arguments[0] == "ADD")
            {
                addUser(arguments[1]);
            }
            else if (arguments[0] == "REMOVE")
            {
                removeUser(arguments[1]);
            }
        }
        private void changeChatWindow(Object str)
        {
            var dispatcher = chatWindow.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(changeChatWindow);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                chatWindow.Text += (string)str;
            }
        }
        private void addUser(Object str)
        {
            var dispatcher = userList.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(addUser);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                string[] list = userList.Text.Split(',');
                list.Append((string) str + '\n');
                Array.Sort(list);
                userList.Text = list.ToString();
            }
        }

        private void removeUser(Object str)
        {
            var dispatcher = userList.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(addUser);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                string list = userList.Text;
                list.Replace((string)str, "");
                userList.Text = list.ToString();
            }
        }
        private void Submit(object sender, RoutedEventArgs e)
        {
            if (userInput.Text != "")
            {
                SendMessageCommand messageToSend = new SendMessageCommand(currentConnection, userInput.Text);
                messageToSend.ExecuteCommand();
                chatWindow.Text += (currentConnection.username + "> " + userInput.Text + "\n");
                userInput.Text = "";              
            }
        }
        private void disconnectFromServer(object sender, RoutedEventArgs e)
        {
            DisconnectCommand disconnect = new DisconnectCommand(currentConnection);
            disconnect.ExecuteCommand();
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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow reg = new RegisterWindow();
            reg.ShowDialog();
            if (reg.canProceed == true)
            {
                connection currentConnection = new connection(reg.username, reg.userPassword,
                    reg.ipAddress, reg.port);
                RegisterCommand regComm = new RegisterCommand(currentConnection, currentConnection.ipAddress);
                string returnMessage = regComm.ExecuteCommand();
            }
            reg.Close();
        }
    }
}
