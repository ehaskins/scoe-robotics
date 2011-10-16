using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaemonCore;
using System.IO;

namespace ConsoleDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            AppStartDaemon monitor = new AppStartDaemon();
            monitor.WatchDirectory = "/home/ubuntu/AutoStart/";
            if (!Directory.Exists(monitor.WatchDirectory))
                Directory.CreateDirectory(monitor.WatchDirectory);
            monitor.Start();
            Console.ReadLine();
        }
    }
}
