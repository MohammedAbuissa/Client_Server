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
    class UDPServer : Server
    {
        public UDPServer(int Port, IPAddress IP):base(Port,IP){ }
        public override void Start()
        {
            try
            {
                Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                S.Bind(EndPoint);
                S.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
                while (true)
                {
                    Next.Reset();
                    DatagramState DState = new DatagramState();
                    DState.ClientSocket = S;
                    Console.WriteLine("listenning....");
                    S.BeginReceiveFrom(DState.Buffer, 0, DatagramState.BufferSize, 0, ref DState.EndPoint, new AsyncCallback(RecieveCallback),DState);
                    Next.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.Read();
        }

        protected override void RecieveCallback(IAsyncResult Result)
        {   
            DatagramState Dstate = (DatagramState)Result.AsyncState;
            int bytes = 0;
            Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            S.Bind(new IPEndPoint(IPAddress.Any, 0));
            Socket past = Dstate.ClientSocket;
            Dstate.ClientSocket = S;
            try
            {
                bytes = past.EndReceiveFrom(Result, ref Dstate.EndPoint);
            }
            catch (Exception e)
            {
                Next.Set();
                Send("Respect the buffer size which is " + DatagramState.BufferSize.ToString(), Dstate);
                return;
            }
            if (bytes>0)
            {
                string content = "";
                Dstate.MsgBuilder.Append(Encoding.ASCII.GetString(Dstate.Buffer, 0, bytes));
                content = Dstate.MsgBuilder.ToString();
                Next.Set();
                try
                {
                    string r = Calculate(content).ToString();
                    Send(r, Dstate);
                }
                catch (Exception e)
                {
                    Send(e.Message, Dstate);
                }
            }
        }

        protected override void Send(string Message,State ConnectionState)
        {
            byte[] Buffer = Encoding.ASCII.GetBytes(Message);
            ConnectionState.ClientSocket.BeginSendTo(Buffer, 0, Buffer.Length, 0, (ConnectionState as DatagramState).EndPoint, new AsyncCallback(SendCallback), ConnectionState);
        }
        private int Calculate(string Parameters)
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
        protected override void SendCallback(IAsyncResult Result)
        {
            DatagramState Dstate = (DatagramState)Result.AsyncState;
            int bytes = Dstate.ClientSocket.EndSendTo(Result);
            Console.WriteLine("sent {0} bytes to Client at address: {1} and port: {2}",bytes,(Dstate.EndPoint as IPEndPoint).Address, (Dstate.EndPoint as IPEndPoint).Port);
            Dstate.ClientSocket.Shutdown(SocketShutdown.Both);
            Dstate.ClientSocket.Close();
        }
    }
}
