using System.Collections.Generic;

namespace Inheritance.State
{
    // PROVISIONAL SANDBOX DAY: AI-generated systems exercise. No accepted story or arcs.
    public static class SandboxDayContent
    {
        public const string Notice = "PROVISIONAL / NON-CANONICAL - systems validation only; Personal Carter may replace every beat.";
        public const string Phase = "sandbox.day.exercise";
        public const string Greeting = "sandbox.day.greeted";
        public const string Briefing = "sandbox.day.briefed";
        public const string FamilyFact = "sandbox.day.family_helped";
        public const string BusinessFact = "sandbox.day.business_helped";
        public const string ReactionFact = "sandbox.day.reacted";
        public const string EndFact = "sandbox.day.finished";
        public const string FamilyId = "sandbox.day.family_elena";
        public const string BusinessId = "sandbox.day.business_charlie";
        public const string FamilyMessageId = "sandbox.day.phone_elena";
        public const string BusinessMessageId = "sandbox.day.phone_charlie";
        public static readonly Consequence Greet = new Consequence(Condition.All(Condition.Phase(Phase), Condition.Fact(Greeting, false)),
            ActionCost.Free, Effect.SetFact(Greeting, true));
        public static readonly Consequence Brief = new Consequence(Condition.All(Condition.Phase(Phase), Condition.Fact(Briefing, false)),
            ActionCost.Free, Effect.SetFact(Briefing, true));
        private static Condition Morning => Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Morning), Condition.Fact(Greeting), Condition.Fact(Briefing));
        public static readonly OpportunityDefinition Family = new OpportunityDefinition(FamilyId,
            ActionCost.Fixed(2), Morning, TimeBlock.Morning, ActionCost.Free,
            new[] { new OpportunityCompetition(BusinessId, OpportunityState.Missed) },
            Effect.SetFact(FamilyFact, true), Effect.GrantKnowledge(FuneralNightContent.Elena, FamilyFact));
        public static readonly OpportunityDefinition Business = new OpportunityDefinition(BusinessId,
            ActionCost.Fixed(3), Morning, TimeBlock.Morning, ActionCost.Free,
            new[] { new OpportunityCompetition(FamilyId, OpportunityState.Deferred) },
            Effect.SetFact(BusinessFact, true), Effect.GrantKnowledge(FuneralNightContent.Charlie, BusinessFact));
        // Explicit end-of-morning boundary lets the two-unit family path retain its spare unit.
        public static readonly Consequence FinishMorning = new Consequence(Condition.All(Condition.Phase(Phase),
            Condition.Time(TimeBlock.Morning), Condition.Opportunity(FamilyId, OpportunityState.Completed)), ActionCost.ConsumeRemainder);
        public static readonly Consequence FinishAfternoon = new Consequence(Condition.All(Condition.Phase(Phase),
            Condition.Time(TimeBlock.Afternoon)), ActionCost.ConsumeRemainder);
        public static readonly MessageDefinition FamilyMessage = new MessageDefinition(FamilyMessageId, FuneralNightContent.Elena,
            "[PROVISIONAL TEST] Elena: The sample family boxes are sorted. Thanks for helping; Charlie's test call window has closed.",
            Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Afternoon), Condition.Opportunity(FamilyId, OpportunityState.Completed),
                Condition.Fact(FamilyFact)), FamilyId);
        public static readonly MessageDefinition BusinessMessage = new MessageDefinition(BusinessMessageId, FuneralNightContent.Charlie,
            "[PROVISIONAL TEST] Charlie: The sample figures are checked. Elena's test task is deferred to another day.",
            Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Afternoon), Condition.Opportunity(BusinessId, OpportunityState.Completed),
                Condition.Fact(BusinessFact)), BusinessId);
        private static Condition Reaction(bool family) => Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Evening),
            Condition.Opportunity(family ? FamilyId : BusinessId, OpportunityState.Completed),
            Condition.Opportunity(family ? BusinessId : FamilyId, family ? OpportunityState.Missed : OpportunityState.Deferred),
            Condition.Fact(family ? FamilyFact : BusinessFact),
            Condition.Knowledge(family ? FuneralNightContent.Elena : FuneralNightContent.Charlie, family ? FamilyFact : BusinessFact),
            Condition.Message(family ? FamilyMessageId : BusinessMessageId, MessageStatus.Read), Condition.Fact(ReactionFact, false));
        public static readonly Consequence FamilyReaction = new Consequence(Reaction(true), ActionCost.Free,
            Effect.GrantKnowledge(FuneralNightContent.Victoria, FamilyFact), Effect.SetFact(ReactionFact, true));
        public static readonly Consequence BusinessReaction = new Consequence(Reaction(false), ActionCost.Free,
            Effect.GrantKnowledge(FuneralNightContent.Victoria, BusinessFact), Effect.SetFact(ReactionFact, true));
        public static readonly Consequence EndDay = new Consequence(Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Evening),
            Condition.Fact(ReactionFact), Condition.Fact(EndFact, false)), ActionCost.ConsumeRemainder, Effect.SetFact(EndFact, true));
        public static ContentCatalog Catalog()
        {
            var entries = new List<ContentEntry>();
            foreach (var character in new[] { FuneralNightContent.Elena, FuneralNightContent.Charlie, FuneralNightContent.Victoria })
                entries.Add(new ContentEntry(ContentKind.Character, character, true, Notice));
            entries.Add(new ContentEntry(ContentKind.Phase, Phase, provisionalReason: Notice));
            foreach (var fact in new[] { Greeting, Briefing, FamilyFact, BusinessFact, ReactionFact, EndFact })
                entries.Add(new ContentEntry(ContentKind.Fact, fact, provisionalReason: Notice));
            entries.Add(ContentEntry.Opportunity(Family, Notice)); entries.Add(ContentEntry.Opportunity(Business, Notice));
            entries.Add(ContentEntry.Message(FamilyMessage, Notice)); entries.Add(ContentEntry.Message(BusinessMessage, Notice));
            var bindings = new[] { Greet, Brief, FinishMorning, FinishAfternoon, FamilyReaction, BusinessReaction, EndDay };
            var names = new[] { "greet", "brief", "finish_morning", "finish_afternoon", "family_reaction", "business_reaction", "end" };
            for (int i = 0; i < bindings.Length; i++) entries.Add(ContentEntry.Binding("sandbox.day." + names[i], bindings[i], Notice));
            return new ContentCatalog(entries, messages: new[] { FamilyMessage, BusinessMessage });
        }
        public static GameState CreateState()
        {
            Catalog().RequireValid(ContentValidationMode.Runtime);
            var state = new GameState(Phase); Evaluate(state); return state;
        }
        public static void Evaluate(GameState state)
        {
            Family.TryMakeAvailable(state); Business.TryMakeAvailable(state);
            // Queue only the chosen branch; delivery requires an explicit later block.
            if (state.GetFact(FamilyFact)) FamilyMessage.TryQueue(state);
            if (state.GetFact(BusinessFact)) BusinessMessage.TryQueue(state);
            FamilyMessage.TryDeliver(state); BusinessMessage.TryDeliver(state);
        }
    }
}
