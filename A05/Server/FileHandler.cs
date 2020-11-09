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
        private string credentialPath;

        public FileHandler()
        {
            credentialPath = "./login.txt";

            if(!File.Exists(credentialPath))
            {
                var pwStream = File.Create(credentialPath);
                pwStream.Close();
            }
        }


        public void WriteCredentials(string msg)
        {
            File.AppendAllText(credentialPath, msg);
        }


        public bool CheckExist(string user, string pw)
        {
            string credentials = user + "," + pw;
            string[] lines = File.ReadAllLines(credentialPath); // get all lines from the password file
            foreach(string line in lines) // for every line retrieved from the password file
            {
                if(line.Contains(credentials)) // check if the username and hashed password match
                {
                    return true; // If both fields match, return true, the user exists
                }
            }
            return false; // if the username and password mix is not found, return false
        }

        //public string FormatForLog(string msg)
        //{

        //}
    }
}
