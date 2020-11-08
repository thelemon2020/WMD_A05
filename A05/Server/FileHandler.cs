using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Server
{
    public class FileHandler
    {
        private string msgLogPath;
        private string credentialPath;

        public FileHandler()
        {
            msgLogPath = "./log.txt";
            credentialPath = "./login.txt";
        }

        public void WriteLog(string msg)
        {
            File.AppendAllText(msgLogPath, msg);
        }

        public string[] ReadLog()
        {
            string[] log = new string[1024]; // the log variable will hold 1024 lines of text
            log = File.ReadAllLines(msgLogPath);
            return log;
        }


        public void WriteCredentials(string msg)
        {
            File.AppendAllText(credentialPath, msg);
        }


        public bool CheckExist(string user, string pw)
        {
            string[] lines = File.ReadAllLines(credentialPath); // get all lines from the password file
            foreach(string line in lines) // for every line retrieved from the password file
            {
                if(line.Contains(user) && line.Contains(pw)) // check if the username and hashed password match
                {
                    return true; // If both fields match, return true, the user exists
                }
            }
            return false; // if the username and password mix is not found, return false
        }
    }
}
