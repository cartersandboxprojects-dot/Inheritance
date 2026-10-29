using System;
using System.Collections.Generic;
using System.IO;

namespace Inheritance.State
{
    // Real Day 1 structure; all sample lines, variations and unresolved identities remain provisional.
    public static class DayOneMorningContent
    {
        public const string Notice = "PROVISIONAL content: minimal dialogue and outcome variations await Personal writing; unresolved identities are non-canonical.";
        public const string Prefix = "day1.morning.";
        public const string Wake = Prefix + "wake";
        public const string First = Prefix + "first_choice";
        public const string BreakfastPhase = Prefix + "breakfast";
        public const string Final = Prefix + "final_choice";
        public const string OfficePending = "day1.office.pending";
        public const string Brecken = "brecken", Marie = "marie", Richard = "richard.hale";
        public const string Family1 = "provisional.family.name1", Family2 = "provisional.family.name2", Family3 = "provisional.family.name3";
        public const string Adrian = "adrian.kessler", FormerGirlfriend = "provisional.contact.former_girlfriend";
        public const string Rapport = "relationship.rapport";
        public const string Coffee = Prefix + "coffee_delivered", Agenda = Prefix + "agenda_read";
        public const string Stayed = Prefix + "stayed", Left = Prefix + "left_room";
        public const string Coverage = Prefix + "funeral_coverage", StockDrop = Prefix + "stock_drop";
        public const string Preparation = Prefix + "newspaper_context", Seed = Prefix + "family_seed";
        public const string Breakfast = Prefix + "breakfast_completed", Support = Prefix + "breakfast_support", Business = Prefix + "breakfast_business";
        public const string Social = Prefix + "social_call", Personal = Prefix + "personal_call";
        public const string OfficeRequired = "day1.office.required", Ready = Prefix + "completed";
        public const string BreakfastParticipants = "Brecken, Elena, Charlie, Victoria, Name[1], Name[2], Name[3]";
        public const string AgendaText = "Marie, your recurring aide: Family Breakfast is required. Charlie, your older cousin and business mentor, will help with business context. Richard Hale is the CEO. Office/board business and funeral follow-ups are on the agenda. [Provisional wording]";

        private static Condition At(string phase) => Condition.All(Condition.Phase(phase), Condition.Time(TimeBlock.Morning));
        public static readonly Consequence DeliverCoffee = new Consequence(Condition.All(At(Wake), Condition.Fact(Coffee, false)),
            ActionCost.Free, Effect.SetFact(Coffee, true));
        public static readonly Consequence ReadAgenda = new Consequence(Condition.All(At(Wake), Condition.Fact(Coffee), Condition.Fact(Agenda, false)),
            ActionCost.Free, Effect.SetFact(Agenda, true), Effect.SetFact(OfficeRequired, true),
            Effect.GrantKnowledge(Brecken, OfficeRequired), Effect.SetPhase(First));
        public static readonly Consequence Stay = new Consequence(At(First), ActionCost.Fixed(1),
            Effect.SetFact(Stayed, true), Effect.SetFact(Preparation, true), Effect.GrantKnowledge(Brecken, Coverage),
            Effect.GrantKnowledge(Brecken, StockDrop), Effect.GrantKnowledge(Brecken, Preparation), Effect.SetPhase(BreakfastPhase));
        public static readonly Consequence Leave = new Consequence(At(First), ActionCost.Fixed(1),
            Effect.SetFact(Left, true), Effect.SetFact(Seed, true), Effect.ModifyRelationship(Family1, Rapport, 1),
            Effect.GrantKnowledge(Brecken, Seed), Effect.GrantKnowledge(Family1, Seed), Effect.SetPhase(BreakfastPhase));
        // One authored breakfast scene, with a small mutually exclusive topic variation inside its single cost.
        public static readonly Consequence BreakfastSupport = BreakfastAction(true);
        public static readonly Consequence BreakfastBusiness = BreakfastAction(false);
        private static Consequence BreakfastAction(bool support) => new Consequence(At(BreakfastPhase), ActionCost.Fixed(1),
            Effect.SetFact(Breakfast, true), Effect.SetFact(support ? Support : Business, true),
            Effect.ModifyRelationship(support ? FuneralNightContent.Elena : FuneralNightContent.Charlie, Rapport, 1),
            Effect.GrantKnowledge(Brecken, support ? Support : Business),
            Effect.GrantKnowledge(support ? FuneralNightContent.Elena : FuneralNightContent.Charlie, support ? Support : Business),
            Effect.SetPhase(Final));
        public static readonly Consequence SocialDuty = FinalAction(true);
        public static readonly Consequence PersonalLife = FinalAction(false);
        private static Consequence FinalAction(bool social) => new Consequence(Condition.All(At(Final), Condition.Fact(Breakfast), Condition.Fact(OfficeRequired)),
            ActionCost.Fixed(1), Effect.SetFact(social ? Social : Personal, true),
            Effect.ModifyRelationship(social ? Adrian : FormerGirlfriend, Rapport, 1),
            Effect.GrantKnowledge(Brecken, social ? Social : Personal),
            Effect.GrantKnowledge(social ? Adrian : FormerGirlfriend, social ? Social : Personal),
            Effect.SetFact(Ready, true), Effect.SetPhase(OfficePending));
        public static readonly Condition ReadyForOffice = Condition.All(Condition.Phase(OfficePending), Condition.Time(TimeBlock.Afternoon),
            Condition.Fact(Ready), Condition.Fact(OfficeRequired), Condition.Fact(Breakfast));

        public static ContentCatalog Catalog()
        {
            var entries = new List<ContentEntry>();
            foreach (var id in new[] { Brecken, Marie, Richard, Adrian }) entries.Add(new ContentEntry(ContentKind.Character, id));
            foreach (var id in new[] { FuneralNightContent.Elena, FuneralNightContent.Charlie, FuneralNightContent.Victoria })
                entries.Add(new ContentEntry(ContentKind.Character, id, legacySpelling: true));
            foreach (var id in new[] { Family1, Family2, Family3, FormerGirlfriend })
                entries.Add(new ContentEntry(ContentKind.Character, id, provisionalReason: Notice));
            entries.Add(new ContentEntry(ContentKind.Attribute, Rapport, provisionalReason: Notice));
            foreach (var id in new[] { Wake, First, BreakfastPhase, Final, OfficePending }) entries.Add(new ContentEntry(ContentKind.Phase, id));
            foreach (var id in new[] { Coffee, Agenda, Stayed, Left, Coverage, StockDrop, Preparation, Seed, Breakfast, Support, Business, Social, Personal, OfficeRequired, Ready })
                entries.Add(new ContentEntry(ContentKind.Fact, id, provisionalReason: Notice));
            var actions = new[] { DeliverCoffee, ReadAgenda, Stay, Leave, BreakfastSupport, BreakfastBusiness, SocialDuty, PersonalLife };
            var names = new[] { "deliver_coffee", "read_agenda", "stay", "leave", "breakfast_support", "breakfast_business", "social_duty", "personal_life" };
            for (int i = 0; i < actions.Length; i++) entries.Add(ContentEntry.Binding(Prefix + "action." + names[i], actions[i], Notice));
            entries.Add(ContentEntry.Binding(Prefix + "ready_for_office", ReadyForOffice));
            return new ContentCatalog(entries);
        }
        public static GameState CreateState()
        {
            Catalog().RequireValid(ContentValidationMode.Runtime);
            var snapshot = new GameState(Wake).Capture(); snapshot.Day = 1;
            var state = GameState.Restore(snapshot);
            // Objective premises exist on BOTH routes; reading grants awareness only on Stay.
            state.SetFact(Coverage, true); state.SetFact(StockDrop, true);
            return state;
        }
        // Bounded authored history validation, not a scheduler or a generic engine rule.
        // Replay the recorded one-shot choices to reject contradictory, skipped or fabricated saved state.
        public static void Validate(GameState state)
        {
            var expected = CreateState();
            Replay(state, expected, Coffee, DeliverCoffee); Replay(state, expected, Agenda, ReadAgenda);
            Replay(state, expected, Stayed, Stay); Replay(state, expected, Left, Leave);
            Replay(state, expected, Support, BreakfastSupport); Replay(state, expected, Business, BreakfastBusiness);
            Replay(state, expected, Social, SocialDuty); Replay(state, expected, Personal, PersonalLife);
            if (StatePersistence.Serialize(expected) != StatePersistence.Serialize(state))
                throw new InvalidDataException("Day 1 Morning save does not match an authored history.");
        }
        private static void Replay(GameState source, GameState target, string fact, Consequence action)
        {
            if (source.GetFact(fact) && !action.TryApply(target)) throw new InvalidDataException("Invalid Morning choice order: " + fact);
        }
    }
}
