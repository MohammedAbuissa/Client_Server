using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            bool connection;
            Console.WriteLine("Type of Connection enter T to TCP or U to UDP");
            String C = Console.ReadLine();
            if(C.Contains("T"))
            {
                connection = true;
            }
            else
            {
                connection = false;
            }
            Console.WriteLine("Enter Port number for the server");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter S to Server and C to Client");
            C = Console.ReadLine();
            if (C.Contains("C"))
            {
                Console.WriteLine("Enter the Count of Clients");
                int Count = int.Parse(Console.ReadLine());
                int[] Numbers = new int[Count * 2];
                Console.WriteLine("Enter Count of Clients Lines containing 2 space-seperate integers");
                for (int i = 0; i < Count*2; i+=2)
                {
                    string[] S = Console.ReadLine().Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
                    if (S.Length < 2)
                        for (int j = S.Length; j < 2; j++)
                            S[j] = "0";
                    for (int j = 0; j < 2; j++)
                    {
                        Numbers[i + j] = int.Parse(S[j]);
                    }
                }
                Client Client;
                if (connection)
                    Client = new TCPClient(port, new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
                else
                    Client = new UDPCLient(port, new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
                Client.Start(Numbers);
            }
            else if (C.Contains("S"))
            {
                Server s;
                if (connection)
                    s = new TCPServer(port, new System.Net.IPAddress(new Byte[] { 127, 0, 0, 1 }));
                else
                    s = new UDPServer(port, new System.Net.IPAddress(new Byte[] { 127, 0, 0, 1 }));
                s.Start();
            }
        }
    }
}
