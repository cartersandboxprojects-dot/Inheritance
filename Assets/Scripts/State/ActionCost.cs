using System;
using System.Linq;

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

    // Optional content wrapper. Conversations need not be opportunities.
    public sealed class OpportunityDefinition
    {
        public string Id { get; }
        public ActionCost Cost { get; }
        private readonly Consequence consequence;
        public OpportunityDefinition(string id, ActionCost cost, Condition condition, params Effect[] effects)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Missing opportunity id.", nameof(id));
            Id = id; Cost = cost ?? throw new ArgumentNullException(nameof(cost));
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            if (effects == null) throw new ArgumentNullException(nameof(effects));
            consequence = new Consequence(Condition.All(Condition.Opportunity(id, OpportunityState.Available), condition),
                cost, effects.Concat(new[] { Effect.SetOpportunity(id, OpportunityState.Completed) }).ToArray());
        }
        public bool CanStart(GameState state) => consequence.Condition.IsMet(state) && state.CanAfford(Cost);
        public bool TryComplete(GameState state) => consequence.TryApply(state);
    }
}
