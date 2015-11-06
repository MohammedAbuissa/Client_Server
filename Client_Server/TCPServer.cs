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
    class TCPServer
    {
        IPEndPoint EndPoint;
        private ManualResetEvent Next = new ManualResetEvent(false);
        public TCPServer()
        {

        }

        public TCPServer(int Port, IPAddress IP)
        {
            EndPoint = new IPEndPoint(IP, Port);
        }

        public void Start()
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

        private void AcceptCallback(IAsyncResult Result)
        {
            Next.Set();
            Socket S = (Socket)Result.AsyncState;
            Socket ClientSocket = S.EndAccept(Result);
            State state = new State();
            state.ClientSocket = ClientSocket;
            ClientSocket.BeginReceive(state.Buffer, 0, State.BufferSize, 0, new AsyncCallback(RecieveCallback), state);
        }


        private void RecieveCallback(IAsyncResult Result)
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
                    //Console.WriteLine(content.Substring(0,k));
                    try
                    {
                        string r = Calculate(content).ToString();
                        Send(r, state.ClientSocket);
                    }
                    catch (Exception e)
                    {

                        Send(e.Message, state.ClientSocket);
                    }
                    
                }
                else
                {
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
        private void Send(string Message, Socket Client)
        {
            byte[] Data = Encoding.ASCII.GetBytes(Message);
            Client.BeginSend(Data, 0, Data.Length, 0,new AsyncCallback(SendCallback), Client);
        }

        private void SendCallback(IAsyncResult Result)
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
