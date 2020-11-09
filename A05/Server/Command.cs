//*********************************************
// File			 : Command.cs
// Project		 : PROG2121 - A5 Chat Program
// Programmer	 : Nick Byam, Chris Lemon
// Last Change   : 2020-11-08
// Description	 : A General command class that just introduces a stringbuilder class for all other command classes to use
//*********************************************


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
        public StringBuilder protocol;


        /////////////////////////////////////////
        // Method       : Command (ctor)
        // Description  : Initializes the string builder
        // Parameters   : N/A
        // Returns      : N/A
        /////////////////////////////////////////
        public Command()
        {
            protocol = new StringBuilder();
        }
    }
}
