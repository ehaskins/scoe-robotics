using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.Interface
{
    public abstract class RobotInterfaceBase<TModel> : NotifyObject
            where TModel : RobotModel.RobotModel
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

        private TModel _RobotModel;
        public TModel RobotModel
        {
            get
            {
                return _RobotModel;
            }
            set
            {
                if (_RobotModel == value)
                    return;
                _RobotModel = value;
                RaisePropertyChanged("RobotModel");
            }
        }

        public abstract void Start();
        public abstract void Stop();


    }
}
