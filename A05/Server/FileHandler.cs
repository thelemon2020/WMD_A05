//*********************************************
// File			 : FileHandler.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, Chris Lemon
// Last Change   : 2020-11-09
// Description	 : A class file that does all the interaction with the files necessary to the server's function
//*********************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Server
{
    //******************************************
    // Name     : FileHandler
    // Purpose  : A class with methods to create, write to, and clear content in the files used for the server
    //******************************************
    public class FileHandler
    {
        // necessary constants and data members
        private const int kPort = 35000;
        private string credentialPath;
        private string clientLog;
        private string super = "admin,!#/)zW??C?J\u000eJ?\u001f?"; // this is awful form, I know, not very secure
                                                                   // Was having trouble figuring out another way to do it

        /////////////////////////////////////////
        // Method       : FileHandler (ctor)
        // Description  : the constructor for the FileHandler class, defines the paths for the necessary files
        //              : If the file does not exist at time of running, it creates it and closes the file stream.
        // Parameters   : N/A
        // Returns      : N/A
        /////////////////////////////////////////
        public FileHandler()
        {
            credentialPath = "./login.txt"; // define paths to the files
            clientLog = "./clientLog.txt";
            try
            {
                if (!File.Exists(credentialPath)) // if file doesn't exist for login info, make it
                {
                    var pwStream = File.Create(credentialPath);
                    pwStream.Close();
                    WriteCredentials(super); // on a new system, it defines the admin login right away
                }
                if(!File.Exists(clientLog)) // if the client log doesn't exist, create it
                {
                    var logStream = File.Create(clientLog);
                    logStream.Close();
                }
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /////////////////////////////////////////
        // Method       : ClientCount
        // Description  : A method that checks how many clients have connected to the server, based on how many lines are in
        //              : the client log. To allow multiple clients to connect from the same computer, it increments the 
        //              : the port number it returns, so that no two clients have the same port for their listener.
        // Parameters   : N/A
        // Returns      : int port : The port number the client's listener thread will use to get incoming messages
        /////////////////////////////////////////
        public int ClientCount()
        {
            int port = kPort;
            string[] lines = new string[1024]; // define a large ish array to hold all lines in the file without fail
            try
            {
                lines = File.ReadAllLines(clientLog); // read all lines from the file
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
                return port;
            }

            if(lines.Length == 0) // if there is nothing written in the file, then this is the first client, return 35000
            {
                return port;
            }
            foreach(string line in lines) // If clients have connected there will be lines in the file, increment for each line
            {
                port++;
            }
            return port;
        }


        /////////////////////////////////////////
        // Method       : UpdateClientLog
        // Description  : A method which writes a new line to the client log file stating which ports are in use or have been used
        // Parameters   : int port : the current port the client is using
        // Returns      : N/A
        /////////////////////////////////////////
        public void UpdateClientLog(int port)
        {
            try
            {
                File.AppendAllText(clientLog, "port: " + port.ToString() + "In use.\n");
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /////////////////////////////////////////
        // Method       : ClearClientLog
        // Description  : A method to clear the clientLog text file, only used upon server shut down
        // Parameters   : N/A
        // Returns      : N/A
        /////////////////////////////////////////
        public void ClearClientLog()
        {
            try
            {
                File.WriteAllText(clientLog, "");
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /////////////////////////////////////////
        // Method       : WriteCredentials
        // Description  : A method that writes a client's user name and hashed password to the login.txt file
        // Parameters   : string msg : a string containing the user's name and pw
        // Returns      : N/A
        /////////////////////////////////////////
        public void WriteCredentials(string msg)
        {
            try
            {
                string tmp = msg + "\n";
                File.AppendAllText(credentialPath, tmp);
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        /////////////////////////////////////////
        // Method       : IsSuper
        // Description  : a method which returns true or false to confirm if the current user is a super user with extra privileges
        // Parameters   : string msg : the current user's name and password
        // Returns      : true : if the user is a super user
        //              : false : if the user is not a super user
        /////////////////////////////////////////
        public bool IsSuper(string msg)
        {
            if(msg == super)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /////////////////////////////////////////
        // Method       : CheckExist
        // Description  : A method that checks to see if a user has already been registered
        // Parameters   : string user : the user name
        //              : string pw : the password
        // Returns      : true : if the user exists
        //              : false if the user does not exist
        /////////////////////////////////////////
        public bool CheckExist(string user, string pw)
        {
            string credentials = user + "," + pw;

            try
            {
                string[] lines = File.ReadAllLines(credentialPath); // get all lines from the password file
                foreach (string line in lines) // for every line retrieved from the password file
                {
                    if (line.Contains(credentials)) // check if the username and hashed password match
                    {
                        return true; // If both fields match, return true, the user exists
                    }
                }
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return false; // if the username and password mix is not found, return false
        }
    }
}
