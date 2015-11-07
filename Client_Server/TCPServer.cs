using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Client_Server
{
    class TCPServer:Server
    {
        public TCPServer(int Port, IPAddress IP):base(Port,IP)
        {
        }

        public override void Start()
        {
            try
            {
                Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("press enter to end");
            Console.ReadLine();
        }

        protected void AcceptCallback(IAsyncResult Result)
        {
            Next.Set();
            Socket S = (Socket)Result.AsyncState;
            Socket ClientSocket = S.EndAccept(Result);
            S.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(true, 0));
            State state = new State();
            state.ClientSocket = ClientSocket;
            ClientSocket.BeginReceive(state.Buffer, 0, State.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
        }


        protected override void RecieveCallback(IAsyncResult Result)
        { 
            State state = (State)Result.AsyncState;
            int bytes = state.ClientSocket.EndReceive(Result);
            if (bytes > 0)
            {
                string content = "";
                state.MsgBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytes));
                content = state.MsgBuilder.ToString();
                int k = content.IndexOf("<EOF>");
               
                if (k >-1)
                {
                    content = content.Substring(0, k);
                    try
                    {
                        string r = Calculate(content).ToString();
                        Send(r, state);
                    }
                    catch (Exception e)
                    {

                        Send(e.Message, state);
                    }
                    
                }
                else
                {
                    state.Buffer = new byte[State.BufferSize];
                    state.ClientSocket.BeginReceive(state.Buffer, 0, State.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
                }
            }
        }
        //Calculate the result
        private int Calculate (string Parameters)
        {
            string[] Operands = Parameters.Split(',');
            int op1, op2;
            try
            {
                op1 = int.Parse(Operands[0]);
                op2 = int.Parse(Operands[1]);
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
            return op1 + op2;
        }

        //send back the result
        protected override void Send(string Message, State ConnectioState)
        {
            byte[] Data = Encoding.ASCII.GetBytes(Message);
            ConnectioState.ClientSocket.BeginSend(Data, 0, Data.Length, 0,new AsyncCallback(SendCallback), ConnectioState.ClientSocket);
        }

        protected override void SendCallback(IAsyncResult Result)
        {
            try
            {
                Socket Client = (Socket)Result.AsyncState;
                Console.WriteLine("Sent {0} bytes to Client at IP:{1} Port:{2}",Client.EndSend(Result),(Client.RemoteEndPoint as IPEndPoint).Address, (Client.RemoteEndPoint as IPEndPoint).Port);
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
