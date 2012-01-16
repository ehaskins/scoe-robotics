using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Robot.Shared.Interface
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

        private RobotModel.RobotModel _Model;
        public RobotModel.RobotModel Model
        {
            get
            {
                return _Model;
            }
            set
            {
                if (_Model == value)
                    return;
                _Model = value;
                RaisePropertyChanged("RobotModel");
            }
        }

        public abstract void Start();
        public abstract void Stop();


    }
}
