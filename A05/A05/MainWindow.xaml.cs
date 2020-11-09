/*
* FILE : MainWindow.xaml.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 08
* DESCRIPTION : This file defines the MainWindow UI class.  
*/using System;
using System.Collections.Generic;
using System.IO;
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

   /*
    * NAME : MainWindow
    * PURPOSE : This is the main window for the client portion of the chat program.  it holds the logic for listening for registering, connecting
    *           recieiving, sending and updating the chat window itself 
    */
    public partial class MainWindow : Window
    {
        //properties
        public bool isConnected { get; set; }
        public connection currentConnection { get; set; }

        delegate void MyCallback(Object obj);

        public MainWindow()
        {
            InitializeComponent();
        }
       /*
        * METHOD : Connect_Click()
        *
        * DESCRIPTION : This method is triggered when the user clicks the Connect To Server button.  It creates a instance of ConnectWindow
        *               and creates a ConnectCommand to send off to the chat server.
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            //start instance of connectWindow
            ConnectWindow newConnection = new ConnectWindow();
            newConnection.Owner = this;
            newConnection.ShowDialog();
            //if user fills it out properly and clicks connect
            if (newConnection.canProceed == true)
            {
                //class to hold the connection details for this chat server
                currentConnection = new connection(newConnection.username, newConnection.userPassword,
                    newConnection.ipAddress, newConnection.port);
                if (currentConnection != null)
                {
                    StartConnectionCommand startUp = new StartConnectionCommand(currentConnection, currentConnection.ipAddress);
                    string[] serverResponse = startUp.ExecuteCommand().Split(',');
                    if (serverResponse[0] == "ACK")
                    {
                        isConnected = true;
                        string connectedMessage = "";
                        if (serverResponse[2] == "1")
                        {
                            connectedMessage = string.Format("Connected to server at {0} with Super User Privileges\n", currentConnection.ipAddress.ToString());
                            SuperUserButton.IsEnabled = true;
                        }
                        else
                        {
                            connectedMessage = string.Format("Connected to server at {0}\n", currentConnection.ipAddress.ToString());
                        }   
                        chatWindow.Text += connectedMessage;
                        int i = 3;
                        List<string> listOfUsers = new List<string>();
                        while (serverResponse[i] != "<EOF>")
                        {
                            string newEntry = serverResponse[i] + "\n";
                            listOfUsers.Add(newEntry);
                            i++;
                        }
                        listOfUsers.Sort();
                        foreach (string user in listOfUsers)
                        {
                            userList.Text += user;
                        }
                        userInput.IsEnabled = true;
                        SubmitMessage.IsEnabled = true;
                        MenuDisconnect.IsEnabled = true;
                        MenuConnect.IsEnabled = true;
                        Thread checkForNewMessages = new Thread(listenForMessages);
                        checkForNewMessages.Start();
                    }
                    else if(serverResponse[0] == "NACK")
                    {
                        chatWindow.Text += "Connection Failed - " + serverResponse[1] + "\n"; 
                        currentConnection = null;
                    }
                    else
                    {
                        chatWindow.Text += serverResponse[0];
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
                changeChatWindow(arguments);
            }
            else if (arguments[0] == "DISCONNECT")
            { 
                shutDownServer(arguments);
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
            string[] userMessage = (string[])str;
            var dispatcher = chatWindow.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(changeChatWindow);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                chatWindow.Text += userMessage[1] + ">" + userMessage[2] + "\n";
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
                string[] list = userList.Text.Split('\n');
                userList.Text = "";
                int i = 0;
                List<string> listOfCurrentUsers = new List<string>();
                while (list[i] != "")
                {
                    listOfCurrentUsers.Add(list[i]);
                    i++;
                }
                string newEntry = (string)str + "\n";
                listOfCurrentUsers.Add(newEntry);
                listOfCurrentUsers.Sort();
                foreach (string line in listOfCurrentUsers)
                {
                    userList.Text += line;
                }
            }
        }

        private void removeUser(Object str)
        {
            var dispatcher = userList.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(removeUser);
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
                chatWindow.Text += (currentConnection.username + ">" + userInput.Text + "\n");
                userInput.Text = "";              
            }
        }
        private void disconnectFromServer(object sender, RoutedEventArgs e)
        {
            DisconnectCommand disconnect = new DisconnectCommand(currentConnection);
            disconnect.ExecuteCommand();
            shutDownServer(disconnect);
        }
        public void shutDownServer(Object str)
        {
            var dispatcher = chatWindow.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServer);
                dispatcher.Invoke(callback);
            }
            else
            {
                chatWindow.Text += "Disconnected from server at " + currentConnection.ipAddress + "\n";
            }
            dispatcher = MenuDisconnect.Dispatcher;
            isConnected = false;
            currentConnection = null;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServer);
                dispatcher.Invoke(callback);
            }
            else
            {
                MenuDisconnect.IsEnabled = false;
            }
            dispatcher = MenuConnect.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServer);
                dispatcher.Invoke(callback);
            }
            else
            {
                MenuConnect.IsEnabled = true;
            }
            dispatcher = userList.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServer);
                dispatcher.Invoke(callback);
            }
            else
            {
                userList.Text = "";
            }
            
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
                string[] returnMessage = regComm.ExecuteCommand().Split(',');
                if (returnMessage[0] == "NACK")
                {
                    if (returnMessage[1] == "0")
                    {
                        chatWindow.Text += "Registration Failed - " + returnMessage[1] + "\n";
                    }
                }
                else
                {
                    chatWindow.Text += "Successfully registered account at " + currentConnection.ipAddress + "\n";
                }
                currentConnection = null;
            }
            reg.Close();
        }
    }
}
