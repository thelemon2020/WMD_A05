/*
* FILE : RegisterCommands.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 02 
* REVISED ON : 2020 - 11 - 09
* DESCRIPTION : This file defines the Register Command Class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace A05
{
    /*
    * NAME : RegisterCommand
    * PURPOSE : This defines the Register Command Class.  It inherits from the StartConnectionCommand class.  It's purpose is to 
    *           create a protocol that will allow the user to register an account with the server
    */
    class RegisterCommand : StartConnectionCommand
    {

        public RegisterCommand(connection newConnection, IPAddress ip) : base(newConnection, ip, "REGISTER") 
        {
        }
    } 
}
