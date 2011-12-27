using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoe.Beagle.Shared.RobotModel
{
    public class AerialRobotBase : RobotModel //TODO:Think about this one.
    {
        public Matrix3 DesiredVelocity;
        public Matrix3 DesiredRotation;
        public Matrix3 CurrentVelocity;
        public Matrix3 CurrentRotation;
    }
}
