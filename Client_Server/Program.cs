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
     l:     Console.WriteLine("Type of Connection enter T to TCP or U to UDP");
            String C = Console.ReadLine();
            if(C.Contains("T"))
            {
                connection = true;
            }
            else if(C.Contains("U"))
            {
                connection = false;
            }
            else
            {
                Console.WriteLine("Invalid Input");
                goto l;
            }
            Console.WriteLine("Enter Port number for the server");
            int port = int.Parse(Console.ReadLine());
     t:     Console.WriteLine("Enter S to Server and C to Client");
            C = Console.ReadLine();
            if (C.Contains("C"))
            {
                Console.WriteLine("Enter the Count of Clients");
                int Count = int.Parse(Console.ReadLine());
                int[] Numbers = new int[Count * 2];
                Console.WriteLine("Press enter to start generating numbers");
                Console.ReadLine();
                Random InputGenerator = new Random(DateTime.Now.Millisecond);
                for (int i = 0; i < Count * 2; i += 2)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Numbers[i + j] = InputGenerator.Next()%33554432;
                        Console.Write(Numbers[i+j].ToString() + " ");
                    }
                    Console.WriteLine();
                }
                Client Client;
                if (connection)
                    Client = new TCPClient(port, new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
                else
                    Client = new UDPCLient(port, new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }));
                Console.WriteLine("Press enter to start simulation");
                Console.ReadLine();
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
            else
            {
                Console.WriteLine("Invalid Input");
                goto t;
            }

        }
    }
}
