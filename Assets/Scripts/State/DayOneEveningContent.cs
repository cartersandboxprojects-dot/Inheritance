using System;
using System.Collections.Generic;
using System.IO;

namespace Inheritance.State
{
    // Day 1 only. Dialogue and interpretations remain provisional.
    public static class DayOneEveningContent
    {
        public const string Prefix = "day1.evening.";
        public const string External = Prefix + "external", Mansion = Prefix + "mansion", Close = Prefix + "victoria";
        public const string CompletePhase = "tutorial.day1.complete", Complete = "day1.completed";
        public const string Entered = Prefix + "entered", Taken = Prefix + "external_taken", Deferred = Prefix + "external_deferred";
        public const string Personal = Prefix + "personal_taken", Social = Prefix + "social_taken";
        public const string Elena = Prefix + "elena", Charlie = Prefix + "charlie", Relative = Prefix + "relative";
        public const string ElenaLater = Prefix + "elena_delayable", CharlieLater = Prefix + "charlie_delayable";
        public const string Caution = Prefix + "victoria_board_caution", Acknowledged = Prefix + "victoria_board_acknowledged", Unbriefed = Prefix + "victoria_unbriefed";
        public const string PersonalInvitation = "day1.invitation.personal", SocialInvitation = "day1.invitation.adrian";
        public const string MinorRelative = "provisional.family.evening_relative";
        private static Condition At(string phase) => Condition.All(Condition.Phase(phase), Condition.Time(TimeBlock.Evening), Condition.Fact(Complete, false));
        private static Consequence Entry(bool social) => new Consequence(Condition.All(At(DayOneAfternoonContent.EveningPending),
            Condition.Fact(DayOneAfternoonContent.BoardCompleted), Condition.Fact(social ? DayOneMorningContent.Social : DayOneMorningContent.Personal)),
            ActionCost.Free, Effect.SetFact(Entered, true), Effect.SetOpportunity(social ? SocialInvitation : PersonalInvitation, OpportunityState.Available), Effect.SetPhase(External));
        public static bool Enter(GameState state) => Entry(state.GetFact(DayOneMorningContent.Social)).TryApply(state);
        public static OpportunityDefinition Invitation(bool social) => new OpportunityDefinition(social ? SocialInvitation : PersonalInvitation, ActionCost.Fixed(2),
            Condition.All(At(External), Condition.Fact(Entered), Condition.Fact(social ? DayOneMorningContent.Social : DayOneMorningContent.Personal)),
            Effect.SetFact(Taken, true), Effect.SetFact(social ? Social : Personal, true),
            Effect.GrantKnowledge(DayOneMorningContent.Brecken, social ? Social : Personal),
            Effect.GrantKnowledge(social ? DayOneMorningContent.Adrian : DayOneMorningContent.FormerGirlfriend, social ? Social : Personal),
            Effect.ModifyRelationship(social ? DayOneMorningContent.Adrian : DayOneMorningContent.FormerGirlfriend, DayOneMorningContent.Rapport, 1), Effect.SetPhase(Mansion));
        public static bool TakeExternal(GameState state) => Invitation(state.GetFact(DayOneMorningContent.Social)).TryComplete(state);
        private static Consequence Deferral(bool social) => new Consequence(Condition.All(At(External),
            Condition.Opportunity(social ? SocialInvitation : PersonalInvitation, OpportunityState.Available)), ActionCost.Free,
            Effect.SetOpportunity(social ? SocialInvitation : PersonalInvitation, OpportunityState.Deferred), Effect.SetFact(Deferred, true), Effect.SetPhase(Mansion));
        public static bool DeferExternal(GameState state) => Deferral(state.GetFact(DayOneMorningContent.Social)).TryApply(state);
        public static string ChoiceFact(int choice) => choice == 0 ? Elena : choice == 1 ? Charlie : choice == 2 ? Relative : null;
        private static Consequence Final(int choice)
        {
            string fact = ChoiceFact(choice), person = choice == 0 ? FuneralNightContent.Elena : choice == 1 ? FuneralNightContent.Charlie : MinorRelative;
            var effects = new List<Effect> { Effect.SetFact(fact, true), Effect.GrantKnowledge(person, fact),
                Effect.GrantKnowledge(DayOneMorningContent.Brecken, fact), Effect.ModifyRelationship(person, DayOneMorningContent.Rapport, 1),
                Effect.SetPhase(Close) };
            if (choice != 0) effects.Add(Effect.SetFact(ElenaLater, true));
            if (choice != 1) effects.Add(Effect.SetFact(CharlieLater, true));
            return new Consequence(At(Mansion), ActionCost.Fixed(1), effects.ToArray());
        }
        // Explicit tutorial wind-down, not another interaction or an implicit time tick.
        private static readonly Consequence WindDown = new Consequence(At(Close), ActionCost.ConsumeRemainder);
        public static bool ChooseFinal(GameState state, int choice)
        {
            if (ChoiceFact(choice) == null) return false;
            var candidate = GameState.Restore(state.Capture());
            if (!Final(choice).TryApply(candidate)) return false;
            if (candidate.CurrentTimeBlock == TimeBlock.Evening && !WindDown.TryApply(candidate)) return false;
            state.CommitFrom(candidate); return true;
        }
        // Only board impressions she witnessed qualify; private preparation, calls and external attendance never do.
        public static string Reaction(GameState state) => state.Knows(FuneralNightContent.Victoria, DayOneAfternoonContent.Concern) ? Caution :
            state.Knows(FuneralNightContent.Victoria, DayOneAfternoonContent.Informed) || state.Knows(FuneralNightContent.Victoria, DayOneAfternoonContent.Listening) ? Acknowledged : Unbriefed;
        private static Consequence Closing(string reaction, int choice) => new Consequence(Condition.All(Condition.Phase(Close), Condition.Time(TimeBlock.Night),
            Condition.Fact(Complete, false), Condition.Fact(ChoiceFact(choice))), ActionCost.Free,
            // Brecken explicitly tells Victoria whom he spent time with during this close.
            Effect.GrantKnowledge(FuneralNightContent.Victoria, ChoiceFact(choice)), Effect.SetFact(reaction, true),
            Effect.GrantKnowledge(FuneralNightContent.Victoria, reaction), Effect.GrantKnowledge(DayOneMorningContent.Brecken, reaction),
            Effect.SetFact(Complete, true), Effect.SetPhase(CompletePhase));
        public static bool CloseDay(GameState state)
        {
            for (int i = 0; i < 3; i++) if (state.GetFact(ChoiceFact(i))) return Closing(Reaction(state), i).TryApply(state);
            return false;
        }
        public static string CloseText(GameState state) => "[Provisional] Brecken tells Victoria about his final Mansion conversation. " +
            (state.GetFact(Elena) ? "She hears that he spent time reassuring Elena. " : state.GetFact(Charlie) ? "She hears that he spent time reviewing business with Charlie. " : "She hears that he spent time with an extended-family relative. ") +
            (Reaction(state) == Caution ? "Victoria recalls his dismissive board response and urges care with obligations. " :
            Reaction(state) == Acknowledged ? "Victoria acknowledges his constructive board response. " : "Victoria makes no claim about the board. ") +
            "She cautions that family, business, obligations and attention will all shape how he is judged. Day 1 complete; no Day 2 content.";
        public static ContentCatalog Catalog()
        {
            var entries = new List<ContentEntry>();
            foreach (string phase in new[] { External, Mansion, Close, CompletePhase }) entries.Add(new ContentEntry(ContentKind.Phase, phase));
            entries.Add(new ContentEntry(ContentKind.Character, MinorRelative, provisionalReason: DayOneMorningContent.Notice));
            foreach (string fact in new[] { Entered, Taken, Deferred, Personal, Social, Elena, Charlie, Relative, ElenaLater, CharlieLater, Caution, Acknowledged, Unbriefed, Complete })
                entries.Add(new ContentEntry(ContentKind.Fact, fact, provisionalReason: DayOneMorningContent.Notice));
            foreach (bool social in new[] { false, true })
            {
                string suffix = social ? "social" : "personal";
                entries.Add(ContentEntry.Binding(Prefix + "action.enter_" + suffix, Entry(social), DayOneMorningContent.Notice));
                entries.Add(ContentEntry.Binding(Prefix + "action.defer_" + suffix, Deferral(social), DayOneMorningContent.Notice));
                entries.Add(ContentEntry.Opportunity(Invitation(social), DayOneMorningContent.Notice));
            }
            entries.Add(ContentEntry.Binding(Prefix + "action.wind_down", WindDown));
            for (int i = 0; i < 3; i++)
            {
                entries.Add(ContentEntry.Binding(Prefix + "action.final_" + i, Final(i), DayOneMorningContent.Notice));
                foreach (string reaction in new[] { Caution, Acknowledged, Unbriefed })
                    entries.Add(ContentEntry.Binding(Prefix + "action.close_" + i + "_" + reaction.Substring(Prefix.Length), Closing(reaction, i), DayOneMorningContent.Notice));
            }
            return DayOneAfternoonContent.Catalog(entries);
        }
        public static void Validate(GameState state)
        {
            var expected = DayOneAfternoonContent.ReplayHistory(state);
            Replay(state, Entered, () => Enter(expected));
            Replay(state, Taken, () => TakeExternal(expected)); Replay(state, Deferred, () => DeferExternal(expected));
            for (int i = 0; i < 3; i++) { int choice = i; Replay(state, ChoiceFact(i), () => ChooseFinal(expected, choice)); }
            Replay(state, Complete, () => CloseDay(expected));
            if (StatePersistence.Serialize(expected) != StatePersistence.Serialize(state)) throw new InvalidDataException("Day 1 save does not match authored Morning/Afternoon/Evening history.");
        }
        private static void Replay(GameState source, string fact, Func<bool> action)
        { if (source.GetFact(fact) && !action()) throw new InvalidDataException("Invalid Evening choice order: " + fact); }
    }
}
