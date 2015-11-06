using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace Client_Server
{
    class State
    {
        public Socket ClientSocket = null;
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder MsgBuilder = new StringBuilder();
    }
}
