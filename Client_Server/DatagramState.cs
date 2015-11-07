using System.Net;
namespace Client_Server
{
    class DatagramState:State
    {
        public EndPoint EndPoint = new IPEndPoint(IPAddress.Any,0);
        public bool Recursive = false;
    }
}
