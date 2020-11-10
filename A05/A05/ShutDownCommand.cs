/*
* FILE : ShutDownCommand.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Chris Lemon
* FIRST VERSION : 2020 - 11 - 09 
* REVISED ON : 2020 - 11 - 09
* DESCRIPTION : This file defines the ShutDown Command Class
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace A05
{  /*
    * NAME : ShutDownCommand
    * PURPOSE : This defines the Shutdown Command Class.  It inherits from the Command class.  It's purpose is to provide a protocol to tell the server to start shutting down
    */
    class ShutDownCommand : StartConnectionCommand
    {
        public ShutDownCommand(connection currentConnection) : base(currentConnection, currentConnection.ipAddress, "SHUTDOWN")
        {
        }

    }
}
