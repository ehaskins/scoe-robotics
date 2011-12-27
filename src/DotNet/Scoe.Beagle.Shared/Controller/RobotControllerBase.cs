using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.RobotController
{
    public abstract class RobotControllerBase<TModel, TInterface> : NotifyObject
        where TModel : RobotModel.RobotModel
        where TInterface : Interface.RobotInterfaceBase<TModel>
    {

    }
}
