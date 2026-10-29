using System;
using System.Collections.Generic;
using System.IO;

namespace Inheritance.State
{
    // Bounded authored flow; no environment, scheduler or general competence score.
    public static class DayOneAfternoonContent
    {
        public const string Prefix = "day1.afternoon.";
        public const string BoardPending = "day1.board.pending", EveningPending = "day1.evening.pending";
        public const string Secretary = "provisional.office.secretary", VP = "provisional.office.acquisitions_vp";
        public const string Selected = Prefix + "office_selected", SecretarySeed = Prefix + "secretary_seed";
        public const string SecretaryDelayable = Prefix + "secretary_delayable", Hale = Prefix + "hale_met";
        public const string HalePrepared = Prefix + "hale_prepared", HaleBasic = Prefix + "hale_basic";
        public const string Acquisitions = Prefix + "acquisitions_met", Merger = "business.acquisition.provisional";
        public const string BoardCompleted = Prefix + "board_completed", Measured = Prefix + "board_measured";
        public const string Dismissive = Prefix + "board_dismissive", Informed = Prefix + "board_informed";
        public const string BoardBriefed = Prefix + "board_hale_prepared";
        public const string Listening = Prefix + "board_listening", Concern = Prefix + "board_concern";
        public const string Participants = "Brecken, Charlie, CEO Richard Hale, Victoria";
        public const string Context = "Succession uncertainty, market reaction, investor confidence, Brecken's authority and immediate operational continuity. First impressions only; no board conflict resolution.";
        private static Condition Office => Condition.All(DayOneMorningContent.ReadyForOffice, Condition.Fact(Selected, false));
        public static readonly Consequence MeetSecretary = OfficeAction(SecretarySeed,
            Effect.ModifyRelationship(Secretary, DayOneMorningContent.Rapport, 1),
            Effect.GrantKnowledge(Secretary, SecretarySeed), Effect.GrantKnowledge(DayOneMorningContent.Brecken, SecretarySeed));
        public static readonly Consequence MeetHalePrepared = HaleAction(true);
        public static readonly Consequence MeetHaleBasic = HaleAction(false);
        public static readonly Consequence MeetAcquisitions = OfficeAction(Acquisitions,
            Effect.SetOpportunity(Merger, OpportunityState.Available), Effect.GrantKnowledge(VP, Acquisitions),
            Effect.GrantKnowledge(DayOneMorningContent.Brecken, Acquisitions));
        // Opening persists across days. Resolution requires an unwritten later authored phase.
        public static readonly OpportunityDefinition AcquisitionOpportunity = new OpportunityDefinition(Merger, ActionCost.Fixed(1),
            Condition.All(Condition.Fact(Acquisitions), Condition.Phase("business.acquisition.followup")));
        private static Consequence OfficeAction(string route, params Effect[] extra)
        {
            var effects = new List<Effect> { Effect.SetFact(Selected, true), Effect.SetFact(route, true),
                Effect.SetFact(SecretaryDelayable, true), Effect.SetPhase(BoardPending) };
            effects.AddRange(extra); return new Consequence(Office, ActionCost.Fixed(1), effects.ToArray());
        }
        private static Consequence HaleAction(bool prepared) => new Consequence(
            Condition.All(Office, Condition.Knowledge(DayOneMorningContent.Brecken, DayOneMorningContent.Preparation, prepared)),
            ActionCost.Fixed(1), Effect.SetFact(Selected, true), Effect.SetFact(Hale, true),
            Effect.SetFact(prepared ? HalePrepared : HaleBasic, true), Effect.SetFact(SecretaryDelayable, true),
            Effect.GrantKnowledge(DayOneMorningContent.Brecken, prepared ? HalePrepared : HaleBasic),
            Effect.GrantKnowledge(DayOneMorningContent.Richard, prepared ? HalePrepared : HaleBasic), Effect.SetPhase(BoardPending));
        public static bool ChooseOffice(GameState state, int route)
        {
            if (route == 0) return MeetSecretary.TryApply(state);
            if (route == 1) return (state.Knows(DayOneMorningContent.Brecken, DayOneMorningContent.Preparation) ? MeetHalePrepared : MeetHaleBasic).TryApply(state);
            return route == 2 && MeetAcquisitions.TryApply(state);
        }
        private static Condition Board => Condition.All(Condition.Phase(BoardPending), Condition.Time(TimeBlock.Afternoon),
            Condition.Fact(DayOneMorningContent.OfficeRequired), Condition.Fact(DayOneMorningContent.Ready),
            Condition.Fact(Selected), Condition.Fact(BoardCompleted, false));
        public static bool HasContext(GameState state) => state.Knows(DayOneMorningContent.Brecken, DayOneMorningContent.Preparation) ||
            state.GetFact(Hale) || state.Knows(DayOneMorningContent.Brecken, DayOneMorningContent.Business);
        private static Consequence BoardAction(bool measured, bool context, bool halePrepared = false)
        {
            var effects = new List<Effect> { Effect.SetFact(BoardCompleted, true), Effect.SetFact(measured ? Measured : Dismissive, true),
                Effect.SetFact(DayOneMorningContent.OfficeRequired, false), Effect.SetPhase(EveningPending) };
            string impression = !measured ? Concern : context ? Informed : Listening;
            effects.Add(Effect.SetFact(impression, true));
            if (measured && halePrepared) effects.Add(Effect.SetFact(BoardBriefed, true));
            foreach (var character in new[] { DayOneMorningContent.Brecken, DayOneMorningContent.Richard, FuneralNightContent.Victoria, FuneralNightContent.Charlie })
                effects.Add(Effect.GrantKnowledge(character, impression));
            foreach (var character in new[] { DayOneMorningContent.Richard, FuneralNightContent.Victoria })
                effects.Add(Effect.ModifyRelationship(character, DayOneMorningContent.Rapport, measured ? 1 : -1));
            return new Consequence(Board, ActionCost.Fixed(2), effects.ToArray());
        }
        public static bool CompleteBoard(GameState state, bool measured) => BoardAction(measured, HasContext(state), state.GetFact(HalePrepared)).TryApply(state);
        public static ContentCatalog Catalog()
        {
            var entries = new List<ContentEntry>();
            foreach (var id in new[] { Secretary, VP }) entries.Add(new ContentEntry(ContentKind.Character, id, provisionalReason: DayOneMorningContent.Notice));
            foreach (var id in new[] { BoardPending, EveningPending, "business.acquisition.followup" }) entries.Add(new ContentEntry(ContentKind.Phase, id));
            foreach (var id in new[] { Selected, SecretarySeed, SecretaryDelayable, Hale, HalePrepared, HaleBasic, Acquisitions, BoardCompleted, Measured, Dismissive, Informed, Listening, Concern, BoardBriefed })
                entries.Add(new ContentEntry(ContentKind.Fact, id, provisionalReason: DayOneMorningContent.Notice));
            var actions = new[] { MeetSecretary, MeetHalePrepared, MeetHaleBasic, MeetAcquisitions, BoardAction(true, true), BoardAction(true, false), BoardAction(false, true), BoardAction(false, false), BoardAction(true, true, true), BoardAction(false, true, true) };
            var names = new[] { "secretary", "hale_prepared", "hale_basic", "acquisitions", "board_informed", "board_listening", "board_concern_context", "board_concern", "board_hale_prepared", "board_hale_prepared_concern" };
            for (int i = 0; i < actions.Length; i++) entries.Add(ContentEntry.Binding(Prefix + "action." + names[i], actions[i], DayOneMorningContent.Notice));
            entries.Add(ContentEntry.Opportunity(AcquisitionOpportunity, "VP identity, target and later resolution remain unwritten; multi-day opening only."));
            return DayOneMorningContent.Catalog(entries);
        }
        public static void Validate(GameState state)
        {
            var expected = DayOneMorningContent.ReplayHistory(state);
            Replay(state, SecretarySeed, () => ChooseOffice(expected, 0));
            Replay(state, Hale, () => ChooseOffice(expected, 1));
            Replay(state, Acquisitions, () => ChooseOffice(expected, 2));
            Replay(state, Measured, () => CompleteBoard(expected, true));
            Replay(state, Dismissive, () => CompleteBoard(expected, false));
            if (StatePersistence.Serialize(expected) != StatePersistence.Serialize(state)) throw new InvalidDataException("Day 1 save does not match an authored Morning/Afternoon history.");
        }
        private static void Replay(GameState source, string fact, Func<bool> action)
        { if (source.GetFact(fact) && !action()) throw new InvalidDataException("Invalid Afternoon choice order: " + fact); }
    }
}
