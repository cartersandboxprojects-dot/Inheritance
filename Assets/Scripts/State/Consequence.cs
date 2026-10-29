using System;
using System.Linq;

namespace Inheritance.State
{
    // Immutable, code-authored predicates. No story vocabulary belongs here.
    public sealed class Condition
    {
        private readonly Func<GameState, bool> evaluate;
        private Condition(Func<GameState, bool> evaluate) { this.evaluate = evaluate; }
        public bool IsMet(GameState state) => evaluate(state ?? throw new ArgumentNullException(nameof(state)));

        public static Condition Fact(string fact, bool expected = true) => new Condition(s => s.GetFact(fact) == expected);
        public static Condition Knowledge(string character, string fact, bool expected = true) =>
            new Condition(s => s.Knows(character, fact) == expected);
        public static Condition RelationshipAtMost(string character, string attribute, int maximum) =>
            new Condition(s => s.GetRelationship(character, attribute) <= maximum);
        public static Condition Opportunity(string opportunity, OpportunityState expected) =>
            new Condition(s => s.GetOpportunityState(opportunity) == expected);
        public static Condition Message(string id, MessageStatus expected) => new Condition(s => s.GetMessageStatus(id) == expected);
        public static Condition Time(TimeBlock expected) => new Condition(s => s.CurrentTimeBlock == expected);
        public static Condition Phase(string expected) => new Condition(s => s.StoryPhase == expected);

        // Copy the collection so callers cannot change a definition after constructing it.
        public static Condition All(params Condition[] conditions)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            var copy = conditions.ToArray();
            if (copy.Any(c => c == null)) throw new ArgumentException("Conditions must not contain null.", nameof(conditions));
            return new Condition(s => copy.All(c => c.IsMet(s)));
        }
    }

    public sealed class Effect
    {
        private readonly Action<GameState> apply;
        private Effect(Action<GameState> apply) { this.apply = apply; }
        public void Apply(GameState state) => apply(state ?? throw new ArgumentNullException(nameof(state)));

        public static Effect SetFact(string fact, bool value) => new Effect(s => s.SetFact(fact, value));
        public static Effect ModifyRelationship(string character, string attribute, int delta) =>
            new Effect(s => s.ModifyRelationship(character, attribute, delta));
        public static Effect GrantKnowledge(string character, string fact) => new Effect(s => s.GrantKnowledge(character, fact));
        public static Effect SetOpportunity(string opportunity, OpportunityState value) =>
            new Effect(s => s.SetOpportunityState(opportunity, value));
        public static Effect AdvanceTime() => new Effect(s => s.AdvanceTime());
        public static Effect SetPhase(string phase) => new Effect(s => s.SetStoryPhase(phase));
    }

    // Explicit caller-driven evaluation: no event loop, scheduler, or automatic propagation.
    public sealed class Consequence
    {
        public Condition Condition { get; }
        private readonly Effect[] effects;
        private readonly ActionCost cost;
        public Consequence(Condition condition, ActionCost cost, params Effect[] effects) : this(condition, effects)
        { this.cost = cost ?? throw new ArgumentNullException(nameof(cost)); }
        public Consequence(Condition condition, params Effect[] effects)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            if (effects == null) throw new ArgumentNullException(nameof(effects));
            this.effects = effects.ToArray();
            if (this.effects.Any(e => e == null)) throw new ArgumentException("Effects must not contain null.", nameof(effects));
        }

        public bool TryApply(GameState state)
        {
            if (!Condition.IsMet(state)) return false;
            if (cost != null && !state.CanAfford(cost)) return false;
            // Every consequence is atomic, including free/legacy authored effects.
            var candidate = GameState.Restore(state.Capture());
            if (cost != null) candidate.TryApplyCost(cost);
            foreach (var effect in effects) effect.Apply(candidate);
            state.CommitFrom(candidate);
            return true;
        }
    }
}
