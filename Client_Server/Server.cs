using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Client_Server
{
    class Server
    {
        IPEndPoint EndPoint;
        private ManualResetEvent Next = new ManualResetEvent(false);
        bool isTCP;
        public Server()
        {

        }

        public Server(int Port, IPAddress IP, bool isTCP = true)
        {
            EndPoint = new IPEndPoint(IP, Port);
            this.isTCP = isTCP;
        }

        public void Start()
        {
            Socket S;
            try
            {
                if (isTCP)
                {
                    S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    S.Bind(EndPoint);
                    S.Listen(1000);
                    while (true)
                    {
                        Next.Reset();
                        Console.WriteLine("Waiting for customers");
                        S.BeginAccept(new AsyncCallback(AcceptCallback), S);
                        Next.WaitOne();
                    }
                }
                else
                {
                    S = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    S.Bind(EndPoint);
                    while (true)
                    {
                        Next.Reset();
                        Console.WriteLine("waiting for custormers");
                        State state = new State();
                        state.ClientSocket = S;
                        S.BeginReceive(state.Buffer, 0, State.BufferSize, 0, new AsyncCallback(UDPRecieveCallback), state);
                        Next.WaitOne();
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("press enter to end");
            Console.ReadLine();
        }

        private void AcceptCallback(IAsyncResult Result)
        {
            Next.Set();
            Socket S = (Socket)Result.AsyncState;
            Socket ClientSocket = S.EndAccept(Result);
            State state = new State();
            state.ClientSocket = ClientSocket;
            ClientSocket.BeginReceive(state.Buffer, 0, State.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
        }



        private void UDPRecieveCallback(IAsyncResult Result)
        {
            Next.Set();
            RecieveCallback(Result);
        }

        private void RecieveCallback(IAsyncResult Result)
        {
            string content = "";
            State state = (State)Result.AsyncState;
            Socket ClientSocket = state.ClientSocket;
            int bytes = ClientSocket.EndReceive(Result);
            if (bytes > 0)
            {
                state.MsgBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytes));
                content = state.MsgBuilder.ToString();
                if(content.IndexOf("E")>-1)
                {
                    Console.WriteLine(content);
                }
                else
                {
                    ClientSocket.BeginReceive(state.Buffer, 0, State.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
                }
            }
        }

    }
}
