using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DaemonCore
{
    public class AppStartDaemon
    {
        FileSystemWatcher watcher;
        string watchDir;
        string runDir;
        string runningAppPath;
        Process runningProc;

        public string WatchDirectory
        {
            get
            {
                return watchDir;
            }
            set
            {
                watchDir = value;
            }
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(watchDir) && Directory.Exists(watchDir))
                throw new InvalidOperationException("WatchDirectory must not be null or empty, and must be a valid directory.");
            watcher = new FileSystemWatcher(watchDir);
            watcher.Changed += WatchHandler;
            watcher.Created += WatchHandler;
            watcher.Deleted += WatchHandler;
            watcher.Renamed += WatchHandler;
            watcher.EnableRaisingEvents = true;
            runDir = Path.Combine(new string[]{watchDir, "RunTemp"});
            runningAppPath = Path.Combine(new string[] { runDir, "tempApp.exe" });
            if (File.Exists(runningAppPath))
                StartRunningProc();
        }

        public void Stop()
        {
            if (watcher == null)
                throw new InvalidOperationException("Watcher is not running.");
            watcher.EnableRaisingEvents = false;
            watcher.Changed -= WatchHandler;
            watcher.Created -= WatchHandler;
            watcher.Deleted -= WatchHandler;
            watcher.Renamed -= WatchHandler;
            watcher.Dispose();
            watcher = null;
        }

        private void StopRunningProc()
        {
            if (runningProc != null && !runningProc.HasExited)
            {
                runningProc.Kill();
                runningProc.WaitForExit();
            }
        }
        private void StartRunningProc()
        {
            runningProc = Process.Start(runningAppPath);
        }
        private void WatchHandler(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (File.Exists(e.FullPath) && e.Name.EndsWith(".exe"))
                {
                    StopRunningProc();
                    if (File.Exists(runningAppPath))
                        File.Delete(runningAppPath);
                    File.Move(e.FullPath, runningAppPath);
                    StartRunningProc();
                }
                else
                {
                    Debug.WriteLine("Ignoring change to " + e.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
