using System;
using System.IO;
using System.Text.RegularExpressions;
using Inheritance.State;
using NUnit.Framework;
using static Inheritance.State.SchedulingProofContent;

namespace Inheritance.Tests
{
    public sealed class SchedulingTests
    {
        private static GameState RoundTrip(GameState state) => StatePersistence.Deserialize(StatePersistence.Serialize(state));
        private static OpportunityDefinition Offer(ActionCost cost, ActionCost accept = null, params Effect[] effects) =>
            new OpportunityDefinition("test.offer", cost, Condition.Fact("ready"), TimeBlock.Afternoon,
                accept ?? ActionCost.Free, new[] { new OpportunityCompetition("test.other", OpportunityState.Missed) }, effects);
        private static GameState Ready()
        {
            var state = new GameState("p", TimeBlock.Afternoon); state.SetFact("ready", true);
            state.SetOpportunityState("test.other", OpportunityState.Available); return state;
        }

        [Test]
        public void OfferRequiresConditionsAndMatchingBlock()
        {
            var state = new GameState("p"); var offer = Offer(ActionCost.Free);
            Assert.That(offer.TryMakeAvailable(state), Is.False);
            state.SetFact("ready", true);
            Assert.That(offer.TryMakeAvailable(state), Is.False);
            state.AdvanceTime(); state.SetFact("ready", false);
            Assert.That(offer.TryMakeAvailable(state), Is.False);
            state.SetFact("ready", true);
            Assert.That(offer.TryMakeAvailable(state), Is.True);
            Assert.That(offer.TryMakeAvailable(state), Is.False);
        }

        [Test]
        public void AcceptAndCompleteUseIndependentAuthoredCosts()
        {
            var state = Ready(); var offer = Offer(ActionCost.Fixed(1), ActionCost.Fixed(1));
            offer.TryMakeAvailable(state);
            Assert.That(offer.TryAccept(state), Is.True);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(2));
            Assert.That(state.GetOpportunityState("test.offer"), Is.EqualTo(OpportunityState.Accepted));
            Assert.That(state.GetOpportunityState("test.other"), Is.EqualTo(OpportunityState.Missed));
            Assert.That(offer.TryAccept(state), Is.False);
            Assert.That(offer.TryComplete(state), Is.True);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(1));
            Assert.That(offer.TryComplete(state), Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OverBudgetActionsLeaveTimeAndCompetitorsUntouched(bool accepting)
        {
            var state = Ready(); var offer = Offer(ActionCost.Fixed(4), ActionCost.Fixed(4));
            offer.TryMakeAvailable(state); string before = StatePersistence.Serialize(state);
            Assert.That(accepting ? offer.TryAccept(state) : offer.TryComplete(state), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [Test]
        public void FailedCompletionEffectsAreAtomicIncludingMessagesAndCompetition()
        {
            var state = Ready(); Thanks.TryQueue(state);
            state.ModifyRelationship("a", "trust", int.MaxValue);
            var offer = Offer(ActionCost.Fixed(1), null, Effect.SetFact("partial", true), Effect.ModifyRelationship("a", "trust", 1));
            offer.TryMakeAvailable(state); string before = StatePersistence.Serialize(state);
            Assert.Throws<OverflowException>(() => offer.TryComplete(state));
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [TestCase(OpportunityState.Accepted)]
        [TestCase(OpportunityState.Completed)]
        public void CompetingCommitmentBlocksOfferAndActions(OpportunityState other)
        {
            var state = Ready(); var offer = Offer(ActionCost.Free);
            state.SetOpportunityState("test.other", other);
            Assert.That(offer.TryMakeAvailable(state), Is.False);
            state.SetOpportunityState("test.offer", OpportunityState.Available);
            string before = StatePersistence.Serialize(state);
            Assert.That(offer.CanStart(state), Is.False);
            Assert.That(offer.TryAccept(state), Is.False);
            Assert.That(offer.TryComplete(state), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [Test]
        public void DeferReofferAndAuthoredExpiryDoNotSpendTime()
        {
            var state = Ready(); var offer = Offer(ActionCost.Free);
            offer.TryMakeAvailable(state);
            Assert.That(offer.TryDefer(state), Is.True);
            Assert.That(offer.TryComplete(state), Is.False);
            Assert.That(offer.TryMakeAvailable(state), Is.True);
            Assert.That(offer.TryExpire(state, Condition.Time(TimeBlock.Evening)), Is.False);
            state.AdvanceTime();
            Assert.That(offer.TryExpire(state, Condition.Time(TimeBlock.Evening)), Is.True);
            Assert.That(offer.TryMakeAvailable(state), Is.False);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
        }

        [Test]
        public void FamilyLoopEndsAfternoonMissesBusinessDeliversAndReactsOnce()
        {
            var state = CreateState();
            Assert.That(state.GetOpportunityState(FamilyId), Is.EqualTo(OpportunityState.Available));
            Assert.That(state.GetOpportunityState(BusinessId), Is.EqualTo(OpportunityState.Available));
            Assert.That(Thanks.TryDeliver(state), Is.False);
            Assert.That(Thanks.TryRead(state), Is.False);
            Assert.That(Family.TryAccept(state), Is.True);
            Assert.That(Business.TryComplete(state), Is.False);
            Assert.That(Family.TryComplete(state), Is.True);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(state.GetOpportunityState(BusinessId), Is.EqualTo(OpportunityState.Missed));
            Assert.That(state.GetMessageStatus(MessageId), Is.EqualTo(MessageStatus.Queued));
            Assert.That(LaterReaction.IsMet(state), Is.False);
            Evaluate(state);
            Assert.That(state.GetMessageStatus(MessageId), Is.EqualTo(MessageStatus.Delivered));
            Assert.That(LaterReaction.IsMet(state), Is.False);
            Assert.That(Thanks.TryRead(state), Is.True);
            Assert.That(LaterReaction.IsMet(state), Is.True);
            Assert.That(React.TryApply(state), Is.True);
            string after = StatePersistence.Serialize(state);
            Evaluate(state);
            Assert.That(Thanks.TryDeliver(state), Is.False);
            Assert.That(Thanks.TryQueue(state), Is.False);
            Assert.That(Thanks.TryRead(state), Is.False);
            Assert.That(React.TryApply(state), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(after));
        }

        [Test]
        public void BusinessLoopDefersFamilyAndDoesNotSendThanks()
        {
            var state = CreateState();
            Assert.That(Business.TryComplete(state), Is.True);
            Assert.That(state.GetOpportunityState(FamilyId), Is.EqualTo(OpportunityState.Deferred));
            Evaluate(state);
            Assert.That(Family.TryMakeAvailable(state), Is.False);
            Assert.That(state.GetMessageStatus(MessageId), Is.EqualTo(MessageStatus.Queued));
            Assert.That(LaterReaction.IsMet(state), Is.False);
        }

        [Test]
        public void DelayedDeliveryRequiresEveryAuthoredConditionAndRunsOnce()
        {
            var state = CreateState();
            Assert.That(Thanks.TryDeliver(state), Is.False);
            state.SetOpportunityState(FamilyId, OpportunityState.Completed); state.SetFact(FamilyFact, true);
            Assert.That(Thanks.TryDeliver(state), Is.False); // Still afternoon.
            state.AdvanceTime(); state.SetFact(FamilyFact, false);
            Assert.That(Thanks.TryDeliver(state), Is.False);
            state.SetFact(FamilyFact, true); state.SetStoryPhase("other");
            Assert.That(Thanks.TryDeliver(state), Is.False);
            state.SetStoryPhase(Phase);
            Assert.That(Thanks.TryDeliver(state), Is.True);
            string after = StatePersistence.Serialize(state);
            Assert.That(Thanks.TryDeliver(state), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(after));
        }

        [Test]
        public void RenderingAndFreeReadDoNotSpendPartialCapacity()
        {
            var state = CreateState(); Family.TryComplete(state); Evaluate(state);
            state.TryApplyCost(ActionCost.Fixed(1));
            string before = StatePersistence.Serialize(state);
            Assert.That(state.GetMessage(MessageId).SenderId, Is.EqualTo("Elena"));
            Assert.That(state.GetMessage(MessageId).RelatedOpportunityId, Is.EqualTo(FamilyId));
            Assert.That(Thanks.Text, Does.Contain("afternoon"));
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
            Assert.That(Thanks.TryRead(state), Is.True);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(2));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
        }

        [Test]
        public void RejectedReadConsequenceLeavesMessageUnreadAndStateUntouched()
        {
            var state = new GameState("p");
            var message = new MessageDefinition("m", "a", "text", Condition.All(), null,
                new Consequence(Condition.Fact("guard"), ActionCost.Free, Effect.SetFact("changed", true)));
            message.TryQueue(state); message.TryDeliver(state);
            string before = StatePersistence.Serialize(state);
            Assert.That(message.TryRead(state), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
            state.SetFact("guard", true);
            Assert.That(message.TryRead(state), Is.True);
            Assert.That(state.GetFact("changed"), Is.True);
        }

        [Test]
        public void ThrowingLegacyReadConsequenceCannotPartiallyMutateLiveState()
        {
            var state = new GameState("p"); state.ModifyRelationship("a", "trust", int.MaxValue);
            var message = new MessageDefinition("m", "a", "text", Condition.All(), null,
                new Consequence(Condition.All(), Effect.SetFact("partial", true), Effect.ModifyRelationship("a", "trust", 1)));
            message.TryQueue(state); message.TryDeliver(state); string before = StatePersistence.Serialize(state);
            Assert.Throws<OverflowException>(() => message.TryRead(state));
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [TestCase("outcome")]
        [TestCase("message")]
        [TestCase("knowledge")]
        [TestCase("fact")]
        [TestCase("competitor")]
        public void LaterReactionRequiresEachIndependentPrerequisite(string missing)
        {
            var state = CreateState(); Family.TryComplete(state); Evaluate(state);
            Thanks.TryRead(state);
            Assert.That(LaterReaction.IsMet(state), Is.True);
            if (missing == "knowledge")
            {
                var save = state.Capture(); save.Knowledge = new KnowledgeSave[0]; state = GameState.Restore(save);
            }
            if (missing == "outcome") state.SetOpportunityState(FamilyId, OpportunityState.Accepted);
            if (missing == "message")
            {
                var save = state.Capture(); save.Messages[0].Status = (int)MessageStatus.Delivered; state = GameState.Restore(save);
            }
            if (missing == "fact") state.SetFact(FamilyFact, false);
            if (missing == "competitor") state.SetOpportunityState(BusinessId, OpportunityState.Available);
            Assert.That(LaterReaction.IsMet(state), Is.False);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void SaveLoadResumesQueuedAcceptedDeliveredAndReadProgress(int step)
        {
            var state = CreateState();
            if (step >= 1) Family.TryAccept(state);
            if (step >= 2) { Family.TryComplete(state); Evaluate(state); }
            if (step >= 3) Thanks.TryRead(state);
            string before = StatePersistence.Serialize(state); var loaded = RoundTrip(state);
            Assert.That(StatePersistence.Serialize(loaded), Is.EqualTo(before));
            if (step <= 1) { Assert.That(Family.TryComplete(loaded), Is.True); Evaluate(loaded); }
            if (step < 3) Assert.That(Thanks.TryRead(loaded), Is.True);
            Assert.That(LaterReaction.IsMet(loaded), Is.True);
            Assert.That(Thanks.TryDeliver(loaded), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void LegacySchemasMigrateWithEmptyInbox(int version)
        {
            var original = CreateState();
            Assert.That(original.TryApplyCost(ActionCost.Fixed(2)), Is.True);
            var save = original.Capture(); save.Version = version; save.Messages = null;
            if (version == 1) save.TimeUnitsRemaining = null;
            var loaded = GameState.Restore(save);
            Assert.That(loaded.GetMessageStatus(MessageId), Is.EqualTo(MessageStatus.Absent));
            Assert.That(loaded.TimeUnitsRemaining, Is.EqualTo(version == 1 ? 3 : 1));
            Assert.That(loaded.Capture().Version, Is.EqualTo(3));
        }

        [TestCase("missing")]
        [TestCase("nullentry")]
        [TestCase("duplicate")]
        [TestCase("id")]
        [TestCase("sender")]
        [TestCase("related")]
        [TestCase("absent")]
        [TestCase("enum")]
        public void InvalidLoadedPhoneStateIsRejected(string kind)
        {
            var save = CreateState().Capture();
            switch (kind)
            {
                case "missing": save.Messages = null; break;
                case "nullentry": save.Messages[0] = null; break;
                case "duplicate": save.Messages = new[] { save.Messages[0], save.Messages[0] }; break;
                case "id": save.Messages[0].Id = " "; break;
                case "sender": save.Messages[0].Sender = null; break;
                case "related": save.Messages[0].RelatedOpportunity = ""; break;
                case "absent": save.Messages[0].Status = 0; break;
                case "enum": save.Messages[0].Status = 99; break;
            }
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
        }

        [TestCase(1)]
        [TestCase(2)]
        public void LegacyJsonWithoutMessagesMigratesAndV2StillRequiresCapacity(int version)
        {
            string json = StatePersistence.Serialize(new GameState("p"));
            json = Regex.Replace(json, "\"Messages\":\\[\\],?", "").Replace("\"Version\":3", "\"Version\":" + version);
            Assert.That(StatePersistence.Deserialize(json).Capture().Messages, Is.Empty);
            string missingCapacity = Regex.Replace(json, "\"TimeUnitsRemaining\":3,?", "");
            if (version == 2) Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(missingCapacity));
            else Assert.That(StatePersistence.Deserialize(missingCapacity).TimeUnitsRemaining, Is.EqualTo(3));
        }

        [Test]
        public void VersionThreeJsonRequiresMessagesAndRequiredRecordFields()
        {
            string json = StatePersistence.Serialize(new GameState("p"));
            Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(Regex.Replace(json, "\"Messages\":\\[\\],?", "")));
            json = StatePersistence.Serialize(CreateState());
            Assert.Catch(() => StatePersistence.Deserialize(Regex.Replace(json, "\"Status\":1,?", "")));
        }

        [Test]
        public void InvalidOpportunityEnumIsRejectedOnLoad()
        {
            var save = CreateState().Capture(); save.Opportunities[0].Value = 99;
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
        }

        [Test]
        public void LoadedMetadataMustMatchDefinitionBeforeDeliveryOrRead()
        {
            var save = CreateState().Capture(); save.Messages[0].Sender = "other";
            var state = GameState.Restore(save); Family.TryComplete(state);
            Assert.That(Thanks.TryDeliver(state), Is.False);
            save = state.Capture(); save.Messages[0].Status = (int)MessageStatus.Delivered;
            Assert.That(Thanks.TryRead(GameState.Restore(save)), Is.False);
        }
    }
}
