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
    class TCPClient : Client
    {
        public TCPClient(int Port, IPAddress IP): base(Port,IP){ }

        public override void Start(int[] Number)
        {
            for (int i = 0; i < Number.Length; i+=2)
            {
                Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //socket options
                S.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(true, 10));
                CState state = new CState();
                state.ServerSocket = S;
                state.Msg = Number[i].ToString() + "," + Number[i + 1].ToString()+"<EOF>";
                S.BeginConnect(EndPoint, new AsyncCallback(ConnectCallback), state);
            }
            Console.ReadLine();
        }
        private void ConnectCallback(IAsyncResult Result)
        {
            CState state = (CState)Result.AsyncState;
            state.ServerSocket.EndConnect(Result);
            byte[] bytes = Encoding.ASCII.GetBytes(state.Msg);
            state.ServerSocket.Send(bytes);
            byte[] Buffer = new byte[1024];
            StringBuilder R = new StringBuilder();
            string Content = "";
            int k;
            do
            {
                state.ServerSocket.Receive(Buffer);
                R.Append(Encoding.ASCII.GetString(Buffer));
                Content = R.ToString();
                k = Content.IndexOf("<EOF>");
            } while (k <= -1);
            Console.WriteLine("Sum of {0} is {1}", state.Msg.Substring(0, state.Msg.IndexOf("<EOF>")), Content.Substring(0, k));
            state.ServerSocket.Shutdown(SocketShutdown.Both);
            state.ServerSocket.Close();
        }
    }
}
