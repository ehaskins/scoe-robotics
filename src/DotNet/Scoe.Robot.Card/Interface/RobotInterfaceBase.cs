using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Robot.Interface
{
    public abstract class RobotInterfaceBase: NotifyObject
    {
        public RobotInterfaceBase()
        {

        }

        public event EventHandler UpdatedRobotModel;
        protected void RaiseUpdatedRobotModel()
        {
            if (UpdatedRobotModel != null)
                UpdatedRobotModel(this, null);
        }

        public abstract void Start();
        public abstract void Stop();


    }
}
