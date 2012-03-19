using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading;
using Scoe.Shared.Model;

namespace Scoe.Shared.Controller
{
    public abstract class IterativeControllerBase :  ControllerBase, IDisposable
    {
        private System.Timers.Timer _timer;

        public List<Scoe.Communication.IInterface> RequiredConnectecions {get;set;}

        bool _initDisabled = true;
        bool _initEnabled = true;
        bool _initAuto = true;
        Semaphore sem = new Semaphore(0, 1);


        public RobotState State { get; private set; }

        public IterativeControllerBase()
        {
            State = new RobotState();
            RequiredConnectecions = new List<Communication.IInterface>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
                if (sem != null)
                {
                    sem.Dispose();
                    sem = null;
                }
            }
        }
        ~IterativeControllerBase()
        {
            Dispose(false);
        }

        public override void Run()
        {
            _timer = new System.Timers.Timer(20);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
            sem.WaitOne();
            _timer.Stop();
        }

        public override void Stop()
        {
            sem.Release();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                MainLoop();
            }
            catch (Exception ex)
            {
                sem.Release();//Return control flow to managing application. If on robot, appDomain will be recycled, and controller re-initialized.
                throw;
            }
        }

        private bool AreRequiredConnectectionsOk()
        {
            bool areOk = true;

            
            foreach (var conn in RequiredConnectecions)
            {
                if (!conn.IsConnected)
                    areOk = false;
            }
            return areOk;
        }

        protected virtual void MainLoop()
        {
            //TODO: Add connected test
            if (State.IsEnabled && AreRequiredConnectectionsOk())
            {
                if (State.IsAutonomous)
                {
                    if (_initAuto)
                    {
                        _initAuto = false;
                        _initDisabled = true;
                        _initEnabled = true;
                        AutonomousInit();
                    }
                    AutonomousLoop();
                }
                else
                {
                    if (_initEnabled)
                    {
                        _initAuto = true;
                        _initDisabled = true;
                        _initEnabled = false;
                        EnabledInit();
                    }
                    EnabledLoop();
                }
            }
            else
            {
                if (_initDisabled)
                {
                    _initAuto = true;
                    _initDisabled = false;
                    _initEnabled = true;
                    DisabledInit();
                }
                DisabledLoop();
            }
        }

        protected virtual void DisabledInit() { }
        protected virtual void DisabledLoop() { }
        protected virtual void EnabledInit() { }
        protected virtual void EnabledLoop() { }
        protected virtual void AutonomousInit() { }
        protected virtual void AutonomousLoop() { }
    }
}
