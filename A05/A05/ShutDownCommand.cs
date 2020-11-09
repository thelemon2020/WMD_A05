using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace A05
{
    class ShutDownCommand : StartConnectionCommand
    {
        public ShutDownCommand(connection currentConnection) : base(currentConnection, currentConnection.ipAddress, "SHUTDOWN")
        {
        }

    }
}
