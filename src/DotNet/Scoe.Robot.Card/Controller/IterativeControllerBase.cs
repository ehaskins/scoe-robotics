using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Scoe.Robot.Model;
using System.Threading;

namespace Scoe.Robot.Controller
{
    public abstract class IterativeControllerBase<T> : ControllerBase<T>
        where T : CardModelBase
    {
        private System.Timers.Timer _timer;

        bool _initDisabled = true;
        bool _initEnabled = true;
        bool _initAuto = true;
        Semaphore sem = new Semaphore(0, 1);

        public IterativeControllerBase()
        {
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
            catch
            {
                sem.Release();//Return control flow to managing application. If on robot, appDomain will be recycled, and controller re-initialized.
            }
        }

        protected virtual void MainLoop()
        {
            if (this.Robot != null)
            {
                var state = Robot.State;
                if (state.IsEnabled)
                {
                    if (state.IsAutonomous)
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
        }

        protected virtual void DisabledInit() { }
        protected virtual void DisabledLoop() { }
        protected virtual void EnabledInit() { }
        protected virtual void EnabledLoop() { }
        protected virtual void AutonomousInit() { }
        protected virtual void AutonomousLoop() { }
    }
}
