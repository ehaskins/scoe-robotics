using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DaemonCore;
using System.IO;

namespace DaemonService
{
    public partial class AppStartService : ServiceBase
    {
        private AppStartDaemon monitor;
        public AppStartService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            monitor = new AppStartDaemon { WatchDirectory = "/home/ubuntu/AutoStart/" };
            if (!Directory.Exists(monitor.WatchDirectory))
                Directory.CreateDirectory(monitor.WatchDirectory);
            monitor.Start();
        }

        protected override void OnStop()
        {
            monitor.Stop();
        }
    }
}
