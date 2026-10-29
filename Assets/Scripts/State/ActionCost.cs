using System;

namespace Inheritance.State
{
    public enum ActionCostMode { Units, ConsumeRemainder, HardTransition }

    // Immutable authored definitions; importance never determines duration.
    public sealed class ActionCost
    {
        public ActionCostMode Mode { get; }
        public int Units { get; }
        private ActionCost(ActionCostMode mode, int units = 0) { Mode = mode; Units = units; }
        public static ActionCost Fixed(int units)
        {
            if (units < 0) throw new ArgumentOutOfRangeException(nameof(units));
            return new ActionCost(ActionCostMode.Units, units);
        }
        public static readonly ActionCost Free = Fixed(0);
        public static readonly ActionCost ConsumeRemainder = new ActionCost(ActionCostMode.ConsumeRemainder);
        public static readonly ActionCost HardTransition = new ActionCost(ActionCostMode.HardTransition);
    }

}
