using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace A05
{
    class RegisterCommand : StartConnectionCommand
    {
        public RegisterCommand(connection newConnection, IPAddress ip) : base(newConnection, ip, "REGISTER") 
        {
        }

        //public string
    } 
}
