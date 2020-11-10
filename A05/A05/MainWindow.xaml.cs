/*
* FILE : MainWindow.xaml.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 09
* DESCRIPTION : This file defines the MainWindow UI class.  
*/
using System;
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
                    StartConnectionCommand startUp = new StartConnectionCommand(currentConnection, currentConnection.ipAddress); //create startconnection command
                    string[] serverResponse = startUp.ExecuteCommand().Split(','); //get the server response back and split it
                    if (serverResponse[0] == "ACK") //if all good
                    {
                        isConnected = true; //mark connected
                        string connectedMessage = "";
                        int tempNum = 0;
                        int.TryParse(serverResponse[2], out tempNum); //get the port number to listen on
                        currentConnection.clientPort = tempNum; //set client port number
                        if (serverResponse[3] == "1")//check if user is a super user 
                        {
                            connectedMessage = string.Format("Connected to server at {0} with Super User Privileges\n", currentConnection.ipAddress.ToString());
                            SuperUserButton.IsEnabled = true; //allow for server shutdown command
                        }
                        else
                        {
                            connectedMessage = string.Format("Connected to server at {0}\n", currentConnection.ipAddress.ToString());
                        }
                        chatWindow.Text += connectedMessage;
                        int i = 4; //the user list starts in the fifth element
                        List<string> listOfUsers = new List<string>();
                        while (serverResponse[i] != "<EOF>") //create a list of current users
                        {
                            string newEntry = serverResponse[i] + "\n";
                            listOfUsers.Add(newEntry);
                            i++;
                        }
                        listOfUsers.Sort();
                        foreach (string user in listOfUsers) // populate the userList textbox
                        {
                            userList.Text += user;
                        }
                        userInput.IsEnabled = true;
                        SubmitMessage.IsEnabled = true;
                        MenuDisconnect.IsEnabled = true;
                        MenuConnect.IsEnabled = false;
                        Thread checkForNewMessages = new Thread(listenForMessages);  //create listener thread
                        checkForNewMessages.Start(); //start thread
                    }
                    else if (serverResponse[0] == "NACK") //if server refuses connection
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
       /*
        * METHOD : listenForMessages()
        *
        * DESCRIPTION : This method creates a listener
        *
        * PARAMETERS : None
        *
        * RETURNS : Nothing
        */
        private void listenForMessages()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, currentConnection.clientPort);
            startServer(listener);
        }
       /*
        * METHOD : startServer()
        *
        * DESCRIPTION : This method starts the listener and checks for a connection
        *
        * PARAMETERS : listener - the tcp listener
        *
        * RETURNS : Nothing
        */
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
                handleConnection(client); //pass the connection to be handled
                client.Close();
            }
            listener.Stop();
        }
       /*
        * METHOD : handleConnection()
        *
        * DESCRIPTION : This method gets information from the server, parses it and sends a response back
        *
        * PARAMETERS : client - the incoming connection
        *
        * RETURNS : Nothing
        */
        private void handleConnection(TcpClient client)
        {
            try
            {
                NetworkStream readStream = client.GetStream();
                string[] incomingData = InteractWithServer.readFromServer(readStream).Split(',');
                AckCommand acknowledge = new AckCommand("ACK,<EOF>");
                InteractWithServer.writeToServer(readStream, acknowledge.protocol.ToString());
                readStream.Close();
                updateUI(incomingData);
            }
            catch (Exception e)
            {
                string[] exceptionString = new string[3];
                exceptionString[0] = "REPLY";
                exceptionString[1] = "Server Error";
                exceptionString[2] = e.Message.ToString();
            }
        }
        /*
       * METHOD : updateUI()
       *
       * DESCRIPTION : this method takes the protocol that just came in and parses it out
       *
       * PARAMETERS : arguments - an array that makes up the protocol that was just recieved
       *
       * RETURNS : Nothing
       */
        private void updateUI(string[] arguments)
        {
            if (arguments[0] == "REPLY")
            {
                changeChatWindow(arguments);
            }
            else if (arguments[0] == "DISCONNECT")
            {
                startShutDownProcess();
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
       /*
        * METHOD : changeChatWindow()
        *
        * DESCRIPTION : This method uses a dispatcher to update the chat window with new messages
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
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
       /*
        * METHOD : addUser()
        *
        * DESCRIPTION : This method uses a dispatcher to update the userList with a new user
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
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
                    listOfCurrentUsers.Add(list[i] + '\n');
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

       /*
        * METHOD : removeUser()
        *
        * DESCRIPTION : This method uses a dispatcher to update the userList when a user disconnects
        *
        * PARAMETERS : str - an object that is cast into a string
        * 
        * RETURNS : Nothing
        */
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
       /*
        * METHOD : Submit()
        *
        * DESCRIPTION : This method is triggered when the user clicks the send button to send a message to the server. 
        *               It generates the information needed to make that connection
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
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

        /*
       * METHOD : disconnectFromServer()
       *
       * DESCRIPTION : This method is triggered when the user clicks the Disconnect From Server button.  It sends a protocol to the server
       *               and resets the client to it's original state
       *
       * PARAMETERS : sender - the object that called this method
       *              e - the arguments sent by the routed event
       *
       * RETURNS : Nothing
       */
        private void disconnectFromServer(object sender, RoutedEventArgs e)
        {
            DisconnectCommand disconnect = new DisconnectCommand(currentConnection);
            disconnect.ExecuteCommand();
            startShutDownProcess();
        }
        /*
       * METHOD : startShutDownProcess()
       *
       * DESCRIPTION : This method resets the client to it's original state 
       * PARAMETERS : None
       *
       * RETURNS : Nothing
       */
        private void startShutDownProcess()
        {
            string disconnect = "";
            shutDownServerChatWindow(disconnect);
            shutDownServerMenuConnect(disconnect);
            shutDownServerMenuDisconnect(disconnect);
            shutDownServerSubmitMessage(disconnect);
            shutDownServerUserInput(disconnect);
            shutDownServerUserList(disconnect);
            isConnected = false;
            currentConnection = null;
        }
       /*
        * METHOD : shutDownServerChatWindow()
        *
        * DESCRIPTION : This method uses a dispatcher to update the chat window with a disconnect message
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
        public void shutDownServerChatWindow(Object str)
        {
            var dispatcher = chatWindow.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServerChatWindow);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                chatWindow.Text += "Disconnected from server at " + currentConnection.ipAddress + "\n";
            }
        }
       /*
        * METHOD : shutDownServerMenuDisconnect()
        *
        * DESCRIPTION : This method uses a dispatcher to disable the disconnect button
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
        public void shutDownServerMenuDisconnect(Object str)
        {
            var dispatcher = MenuDisconnect.Dispatcher;

            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServerMenuDisconnect);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                MenuDisconnect.IsEnabled = false;
            }
        }
       /*
        * METHOD : shutDownServerMenuConnect()
        *
        * DESCRIPTION : This method uses a dispatcher to enable the connect button
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
        public void shutDownServerMenuConnect(Object str)
        {
            var dispatcher = MenuConnect.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServerMenuConnect);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                MenuConnect.IsEnabled = true;
            }
        }
       /*
        * METHOD : shutDownServerUserList()
        *
        * DESCRIPTION : This method uses a dispatcher to clear the userlist
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
        public void shutDownServerUserList(Object str)
        {
            var dispatcher = userList.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServerUserList);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                userList.Text = "";
            }
        }
       /*
        * METHOD : shutDownServerSubmitMessage()
        *
        * DESCRIPTION : This method uses a dispatcher to disable the submit button
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
        public void shutDownServerSubmitMessage(Object str)
        {
            var dispatcher = SubmitMessage.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServerSubmitMessage);
                dispatcher.Invoke(callback, new object[] { str });
            }
            else
            {
                SubmitMessage.IsEnabled = false;
            }
        }
       /*
        * METHOD : shutDownServerUserInput()
        *
        * DESCRIPTION : This method uses a dispatcher to disable the userInput field
        *
        * PARAMETERS : str - an object that is cast into a string
        *
        * RETURNS : Nothing
        */
        public void shutDownServerUserInput(Object str)
        {
            var dispatcher = userInput.Dispatcher;
            if (!dispatcher.CheckAccess())
            {
                MyCallback callback = new MyCallback(shutDownServerUserInput);
                dispatcher.Invoke(callback, new object[] { str });
            }      
            else
            {
                userInput.Text = "";
                userInput.IsEnabled = false;
            }

        }
        /*
        * METHOD : Register_Click()
        *
        * DESCRIPTION : This method is triggered when the user clicks the Register button. It opens a new window that allows the user to enter information
        *               then uses that information to register on the server
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
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
                    chatWindow.Text += "Registration Failed - " + returnMessage[1] + "\n";
                }
                else
                {
                    chatWindow.Text += "Successfully registered account at " + currentConnection.ipAddress + "\n";
                }
                currentConnection = null;
            }
            reg.Close();
        }
       /*
        * METHOD : SuperShutDown()
        *
        * DESCRIPTION : This method is triggered when the super user clicks the Shutdown Server button.  It sends a protocol to the server
        *               that triggers a shut down
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
        private void SuperShutDown(object sender, RoutedEventArgs e)
        {
            ShutDownCommand shutItDown = new ShutDownCommand(currentConnection);
            shutItDown.ExecuteCommand();
        }

       /*
        * METHOD : commandBinding_CanExecute_Close()
        *
        * DESCRIPTION : Checks to see if the command can be executed, which it always can
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
        private void CommandBinding_CanExecute_Close(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

       /*
        * METHOD : commandBinding_Executed_Close()
        *
        * DESCRIPTION : Closes the window when close clicked
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the routed event
        *
        * RETURNS : Nothing
        */
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

       /*
        * METHOD : MainWindow_Closing()
        *
        * DESCRIPTION : Shuts down the connection to the server if window is closed
        *
        * PARAMETERS : sender - the object that called this method
        *              e - the arguments sent by the cancel event
        *
        * RETURNS : Nothing
        */
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (currentConnection != null)
            {
                startShutDownProcess();
            }
        }
    }
}
