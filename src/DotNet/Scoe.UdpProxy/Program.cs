using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoe.UdpProxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Scoe Communication Udp Proxy Server");
            (new UdpProxy()).Run("COM5", 115200);
        }
    }
}
