using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace Client_Server
{
    class CState
    {
        public Socket ServerSocket;
        public string Msg;
    }
}
