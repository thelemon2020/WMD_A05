﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Connection
    {
        public string Name { get; set; }
        public TcpClient Client { get; set; }

        public void Send(string msg)
        {

        }
    }
}
