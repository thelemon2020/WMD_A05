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

        public Command()
        {
            protocol = new StringBuilder();
        }
    }
}
