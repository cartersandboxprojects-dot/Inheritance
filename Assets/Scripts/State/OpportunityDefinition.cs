using System;
using System.Linq;

namespace Inheritance.State
{
    public sealed class OpportunityCompetition
    {
        public string Id { get; }
        public OpportunityState Result { get; }
        public OpportunityCompetition(string id, OpportunityState result)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Missing competitor id.", nameof(id));
            if (result != OpportunityState.Missed && result != OpportunityState.Deferred)
                throw new ArgumentOutOfRangeException(nameof(result));
            Id = id; Result = result;
        }
    }

    // Definitions are rebuilt from content after load. All progress belongs to GameState.
    public sealed class OpportunityDefinition
    {
        public string Id { get; }
        public ActionCost Cost { get; }
        public ActionCost AcceptanceCost { get; }
        private readonly Condition eligibility;
        private readonly TimeBlock? block;
        private readonly OpportunityCompetition[] competing;
        private readonly Effect[] effects;

        public OpportunityDefinition(string id, ActionCost cost, Condition condition, params Effect[] effects)
            : this(id, cost, condition, null, ActionCost.Free, new OpportunityCompetition[0], effects) { }

        public OpportunityDefinition(string id, ActionCost cost, Condition condition, TimeBlock? block,
            ActionCost acceptanceCost, OpportunityCompetition[] competing, params Effect[] effects)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Missing opportunity id.", nameof(id));
            if (block.HasValue && !Enum.IsDefined(typeof(TimeBlock), block.Value)) throw new ArgumentOutOfRangeException(nameof(block));
            Id = id; Cost = cost ?? throw new ArgumentNullException(nameof(cost));
            AcceptanceCost = acceptanceCost ?? throw new ArgumentNullException(nameof(acceptanceCost));
            eligibility = condition ?? throw new ArgumentNullException(nameof(condition));
            this.block = block;
            this.competing = competing?.ToArray() ?? throw new ArgumentNullException(nameof(competing));
            this.effects = effects?.ToArray() ?? throw new ArgumentNullException(nameof(effects));
            if (this.effects.Any(e => e == null) || this.competing.Any(c => c == null || c.Id == id) ||
                this.competing.Select(c => c.Id).Distinct(StringComparer.Ordinal).Count() != this.competing.Length)
                throw new ArgumentException("Invalid effects or competitors.");
        }

        private bool Eligible(GameState state) => eligibility.IsMet(state) && (!block.HasValue || state.CurrentTimeBlock == block.Value);
        private bool CompetitorsAllow(GameState state) => competing.All(c =>
            state.GetOpportunityState(c.Id) != OpportunityState.Accepted && state.GetOpportunityState(c.Id) != OpportunityState.Completed);

        public bool TryMakeAvailable(GameState state)
        {
            var current = state.GetOpportunityState(Id);
            if ((current != OpportunityState.Unavailable && current != OpportunityState.Deferred) || !Eligible(state) || !CompetitorsAllow(state)) return false;
            state.SetOpportunityState(Id, OpportunityState.Available);
            return true;
        }
        public bool CanStart(GameState state) => state.GetOpportunityState(Id) == OpportunityState.Available &&
            Eligible(state) && CompetitorsAllow(state) && state.CanAfford(Cost);

        public bool TryAccept(GameState state) => Apply(state, OpportunityState.Accepted, AcceptanceCost, false);
        public bool TryComplete(GameState state) => Apply(state, OpportunityState.Completed, Cost, true);
        private bool Apply(GameState state, OpportunityState target, ActionCost cost, bool complete)
        {
            var current = state.GetOpportunityState(Id);
            if ((current != OpportunityState.Available && !(complete && current == OpportunityState.Accepted)) ||
                !Eligible(state) || !CompetitorsAllow(state) || !state.CanAfford(cost)) return false;
            var candidate = GameState.Restore(state.Capture());
            candidate.TryApplyCost(cost);
            foreach (var competitor in competing)
            {
                var status = candidate.GetOpportunityState(competitor.Id);
                if (status == OpportunityState.Unavailable || status == OpportunityState.Available || status == OpportunityState.Deferred)
                    candidate.SetOpportunityState(competitor.Id, competitor.Result);
            }
            if (complete) foreach (var effect in effects) effect.Apply(candidate);
            candidate.SetOpportunityState(Id, target);
            state.CommitFrom(candidate);
            return true;
        }
        public bool TryDefer(GameState state)
        {
            if (state.GetOpportunityState(Id) != OpportunityState.Available) return false;
            state.SetOpportunityState(Id, OpportunityState.Deferred); return true;
        }
        // Caller supplies an authored expiry predicate; no implicit clock tick or expiry policy.
        public bool TryExpire(GameState state, Condition when)
        {
            if (when == null) throw new ArgumentNullException(nameof(when));
            var current = state.GetOpportunityState(Id);
            if ((current != OpportunityState.Available && current != OpportunityState.Deferred && current != OpportunityState.Accepted) || !when.IsMet(state)) return false;
            state.SetOpportunityState(Id, OpportunityState.Expired); return true;
        }
    }
}
