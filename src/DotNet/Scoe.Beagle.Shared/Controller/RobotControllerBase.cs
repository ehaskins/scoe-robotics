using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Robot.Shared.RobotController
{
    public abstract class RobotControllerBase<TModel, TInterface> : NotifyObject
        where TModel : RobotModel.RobotModel
        where TInterface : Interface.RobotInterfaceBase
    {

    }
}
