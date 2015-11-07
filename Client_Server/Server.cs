using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Client_Server
{
    abstract class Server
    {
        protected IPEndPoint EndPoint;
        protected ManualResetEvent Next = new ManualResetEvent(false);
        public Server(int Port, IPAddress IP)
        {
            EndPoint = new IPEndPoint(IP, Port);
        }
        public abstract void Start();
        protected abstract void RecieveCallback(IAsyncResult Result);
        protected abstract void Send(string Message, State ConnectionState);
        protected abstract void SendCallback(IAsyncResult Result);
    }
}
