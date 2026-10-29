using System;
using System.Linq;

namespace Inheritance.State
{
    // Immutable, code-authored predicates. No story vocabulary belongs here.
    public sealed class Condition
    {
        private readonly Func<GameState, bool> evaluate;
        internal string[] Errors { get; private set; } = new string[0];
        private string constraintKey, constraintValue;
        private Condition[] children = new Condition[0];
        private System.Collections.Generic.IEnumerable<Condition> Leaves => children.Length == 0 ? new[] { this } : children.SelectMany(c => c.Leaves);
        private Condition ExpectEnum<T>(T value)
        { if (!Enum.IsDefined(typeof(T), value)) Errors = new[] { "Invalid predicate enum: " + typeof(T).Name + ":" + value }; return this; }
        private Condition Constrain(string key, object value)
        { constraintKey = key; constraintValue = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture); return this; }
        internal ContentReference[] References { get; }
        private Condition(Func<GameState, bool> evaluate, params ContentReference[] references)
        { this.evaluate = evaluate; References = references; }
        public bool IsMet(GameState state) => evaluate(state ?? throw new ArgumentNullException(nameof(state)));

        public static Condition Fact(string fact, bool expected = true) => new Condition(s => s.GetFact(fact) == expected, new ContentReference(ContentKind.Fact, fact)).Constrain("fact:" + fact, expected);
        public static Condition Knowledge(string character, string fact, bool expected = true) =>
            new Condition(s => s.Knows(character, fact) == expected, new ContentReference(ContentKind.Character, character), new ContentReference(ContentKind.Fact, fact)).Constrain("knowledge:" + character + ":" + fact, expected);
        public static Condition RelationshipAtMost(string character, string attribute, int maximum) =>
            new Condition(s => s.GetRelationship(character, attribute) <= maximum, new ContentReference(ContentKind.Character, character), new ContentReference(ContentKind.Attribute, attribute));
        public static Condition Opportunity(string opportunity, OpportunityState expected) =>
            new Condition(s => s.GetOpportunityState(opportunity) == expected, new ContentReference(ContentKind.Opportunity, opportunity)).ExpectEnum(expected).Constrain("opportunity:" + opportunity, expected);
        public static Condition Message(string id, MessageStatus expected) => new Condition(s => s.GetMessageStatus(id) == expected, new ContentReference(ContentKind.Message, id)).ExpectEnum(expected).Constrain("message:" + id, expected);
        public static Condition Time(TimeBlock expected) => new Condition(s => s.CurrentTimeBlock == expected).ExpectEnum(expected).Constrain("time", expected);
        public static Condition Phase(string expected) => new Condition(s => s.StoryPhase == expected, new ContentReference(ContentKind.Phase, expected)).Constrain("phase", expected);

        // Copy the collection so callers cannot change a definition after constructing it.
        public static Condition All(params Condition[] conditions)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            var copy = conditions.ToArray();
            if (copy.Any(c => c == null)) throw new ArgumentException("Conditions must not contain null.", nameof(conditions));
            var result = new Condition(s => copy.All(c => c.IsMet(s)), copy.SelectMany(c => c.References).ToArray()) { children = copy };
            result.Errors = copy.SelectMany(c => c.Errors).Concat(result.Leaves.Where(c => c.constraintKey != null)
                .GroupBy(c => c.constraintKey).Where(g => g.Select(c => c.constraintValue).Distinct().Count() > 1)
                .Select(g => "Contradictory equality predicates for " + g.Key)).ToArray();
            return result;
        }
    }

    public sealed class Effect
    {
        private readonly Action<GameState> apply;
        internal string[] Errors { get; private set; } = new string[0];
        private Effect ExpectEnum<T>(T value)
        { if (!Enum.IsDefined(typeof(T), value)) Errors = new[] { "Invalid effect enum: " + typeof(T).Name + ":" + value }; return this; }
        internal ContentReference[] References { get; }
        private Effect(Action<GameState> apply, params ContentReference[] references)
        { this.apply = apply; References = references; }
        public void Apply(GameState state) => apply(state ?? throw new ArgumentNullException(nameof(state)));

        public static Effect SetFact(string fact, bool value) => new Effect(s => s.SetFact(fact, value), new ContentReference(ContentKind.Fact, fact));
        public static Effect ModifyRelationship(string character, string attribute, int delta) =>
            new Effect(s => s.ModifyRelationship(character, attribute, delta), new ContentReference(ContentKind.Character, character), new ContentReference(ContentKind.Attribute, attribute));
        public static Effect GrantKnowledge(string character, string fact) => new Effect(s => s.GrantKnowledge(character, fact), new ContentReference(ContentKind.Character, character), new ContentReference(ContentKind.Fact, fact));
        public static Effect SetOpportunity(string opportunity, OpportunityState value) =>
            new Effect(s => s.SetOpportunityState(opportunity, value), new ContentReference(ContentKind.Opportunity, opportunity)).ExpectEnum(value);
        public static Effect AdvanceTime() => new Effect(s => s.AdvanceTime());
        public static Effect SetPhase(string phase) => new Effect(s => s.SetStoryPhase(phase), new ContentReference(ContentKind.Phase, phase));
    }

    // Explicit caller-driven evaluation: no event loop, scheduler, or automatic propagation.
    public sealed class Consequence
    {
        public Condition Condition { get; }
        internal string[] Errors => Condition.Errors.Concat(effects.SelectMany(e => e.Errors)).ToArray();
        internal ContentReference[] References => Condition.References.Concat(effects.SelectMany(e => e.References)).ToArray();
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
