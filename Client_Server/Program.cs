﻿using System;
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
            Server s = new Server(2001, new System.Net.IPAddress(new Byte[] { 127, 0, 0, 1 }),false);
            s.Start();
        }
    }
}
