using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace Client_Server
{
    abstract class Client
    {
        protected IPEndPoint EndPoint;
        public Client(int Port, IPAddress IP)
        {
            EndPoint = new IPEndPoint(IP, Port);
        }
        abstract public void Start(int[] Number);
    }
}
