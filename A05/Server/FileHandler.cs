﻿using System;
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
        private string super = "admin,!#/)zW??C?J\u000eJ?\u001f?"; // this is awful form, I know, not very secure

        public FileHandler()
        {
            credentialPath = "./login.txt";
            try
            {
                if (!File.Exists(credentialPath))
                {
                    var pwStream = File.Create(credentialPath);
                    pwStream.Close();
                    WriteCredentials(super);
                }
            }
            catch(IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


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
