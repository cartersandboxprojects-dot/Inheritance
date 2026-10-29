using System;
using System.IO;
using System.Text.RegularExpressions;
using Inheritance.State;
using NUnit.Framework;

namespace Inheritance.Tests
{
    public sealed class ActionCostTests
    {
        private static Consequence Action(ActionCost cost, params Effect[] effects) =>
            new Consequence(Condition.All(), cost, effects);

        [Test]
        public void FreeEmotionalActionChangesFactsWithoutTime()
        {
            var state = new GameState("p", TimeBlock.Evening);
            state.TryApplyCost(ActionCost.Fixed(1));
            Assert.That(Action(ActionCost.Free, Effect.ModifyRelationship("e", "trust", 100)).TryApply(state), Is.True);
            Assert.That(state.GetRelationship("e", "trust"), Is.EqualTo(100));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(2));
        }

        [Test]
        public void MinorCostReducesCapacity()
        {
            var state = new GameState("p");
            Assert.That(Action(ActionCost.Fixed(1)).TryApply(state), Is.True);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(2));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Morning));
        }

        [TestCase(TimeBlock.Morning, TimeBlock.Afternoon, 0)]
        [TestCase(TimeBlock.Afternoon, TimeBlock.Evening, 0)]
        [TestCase(TimeBlock.Evening, TimeBlock.Night, 0)]
        [TestCase(TimeBlock.Night, TimeBlock.Morning, 1)]
        public void ExactExhaustionAdvancesOnceAndResets(TimeBlock before, TimeBlock after, int day)
        {
            var state = new GameState("p", before);
            state.TryApplyCost(ActionCost.Fixed(1));
            Assert.That(Action(ActionCost.Fixed(2)).TryApply(state), Is.True);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(after));
            Assert.That(state.Day, Is.EqualTo(day));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
        }

        [Test]
        public void OverBudgetRejectsWithoutAnyEffects()
        {
            var state = new GameState("p");
            state.TryApplyCost(ActionCost.Fixed(2));
            string before = StatePersistence.Serialize(state);
            Assert.That(Action(ActionCost.Fixed(2), Effect.SetFact("f", true)).TryApply(state), Is.False);
            Assert.That(state.TryApplyCost(ActionCost.Fixed(4)), Is.False);
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [TestCase(0)]
        [TestCase(2)]
        public void MajorCommitmentConsumesRemainder(int spent)
        {
            var state = new GameState("p", TimeBlock.Evening);
            state.TryApplyCost(ActionCost.Fixed(spent));
            Assert.That(Action(ActionCost.ConsumeRemainder).TryApply(state), Is.True);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
        }

        [Test]
        public void HardTransitionAndLegacyAdvanceResetCapacity()
        {
            var state = new GameState("p");
            state.TryApplyCost(ActionCost.Fixed(1));
            Assert.That(Action(ActionCost.HardTransition).TryApply(state), Is.True);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Afternoon));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
            state.TryApplyCost(ActionCost.Fixed(1)); state.AdvanceTime();
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
        }

        [Test]
        public void CostedEffectsFailureDoesNotMutateAuthoritativeState()
        {
            var state = new GameState("p");
            state.ModifyRelationship("e", "trust", int.MaxValue);
            string before = StatePersistence.Serialize(state);
            Assert.Throws<OverflowException>(() => Action(ActionCost.Fixed(1),
                Effect.SetFact("f", true), Effect.ModifyRelationship("e", "trust", 1)).TryApply(state));
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [Test]
        public void DayOverflowCannotPartiallyApplyTransition()
        {
            var save = new GameState("p", TimeBlock.Night).Capture(); save.Day = int.MaxValue;
            var state = GameState.Restore(save);
            string before = StatePersistence.Serialize(state);
            Assert.Throws<OverflowException>(() => Action(ActionCost.HardTransition, Effect.SetFact("f", true)).TryApply(state));
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
            Assert.Throws<OverflowException>(() => state.TryApplyCost(ActionCost.ConsumeRemainder));
            Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
        }

        [Test]
        public void OpportunityRequiresAvailabilityAndBudgetAndCompletesOnce()
        {
            var state = new GameState("p");
            var opportunity = new OpportunityDefinition("o", ActionCost.Fixed(2), Condition.Fact("ready"), Effect.SetFact("done", true));
            Assert.That(opportunity.CanStart(state), Is.False);
            state.SetOpportunityState("o", OpportunityState.Available);
            Assert.That(opportunity.TryComplete(state), Is.False);
            state.SetFact("ready", true); state.TryApplyCost(ActionCost.Fixed(2));
            Assert.That(opportunity.CanStart(state), Is.False);
            Assert.That(opportunity.TryComplete(state), Is.False);
            Assert.That(state.GetOpportunityState("o"), Is.EqualTo(OpportunityState.Available));
            state.AdvanceTime();
            Assert.That(opportunity.CanStart(state), Is.True);
            Assert.That(opportunity.TryComplete(state), Is.True);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(1));
            Assert.That(state.GetFact("done"), Is.True);
            Assert.That(state.GetOpportunityState("o"), Is.EqualTo(OpportunityState.Completed));
            Assert.That(opportunity.TryComplete(state), Is.False);
        }

        [Test]
        public void RemainingCapacityRoundTrips()
        {
            var state = new GameState("p", TimeBlock.Evening);
            state.TryApplyCost(ActionCost.Fixed(2));
            var loaded = StatePersistence.Deserialize(StatePersistence.Serialize(state));
            Assert.That(loaded.TimeUnitsRemaining, Is.EqualTo(1));
            Assert.That(loaded.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            loaded.TryApplyCost(ActionCost.Fixed(1));
            Assert.That(loaded.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(1));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(4)]
        public void InvalidCapacityRejectedOnLoad(int units)
        {
            string json = StatePersistence.Serialize(new GameState("p"));
            json = json.Replace("\"TimeUnitsRemaining\":3", "\"TimeUnitsRemaining\":" + units);
            Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(json));
        }

        [Test]
        public void VersionTwoRequiresCapacityAndVersionOneMigratesToFullBlock()
        {
            string json = StatePersistence.Serialize(new GameState("p", TimeBlock.Evening));
            string missing = Regex.Replace(json, "\"TimeUnitsRemaining\":3,?", "");
            Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(missing));
            Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(json.Replace("\"TimeUnitsRemaining\":3", "\"TimeUnitsRemaining\":null")));
            var loaded = StatePersistence.Deserialize(missing.Replace("\"Version\":" + StatePersistence.CurrentVersion, "\"Version\":1"));
            Assert.That(loaded.TimeUnitsRemaining, Is.EqualTo(3));
            Assert.That(loaded.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(loaded.Capture().Version, Is.EqualTo(StatePersistence.CurrentVersion));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FuneralChoiceExplicitlyTransitionsAndVictoriaReportIsFree(bool business)
        {
            var state = FuneralNightContent.CreateState();
            state.TryApplyCost(ActionCost.Fixed(2));
            Assert.That((business ? FuneralNightConsequences.ChooseCharlie : FuneralNightConsequences.ChooseElena).TryApply(state), Is.True);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
            Assert.That((business ? FuneralNightConsequences.InformVictoriaAboutCharlie : FuneralNightConsequences.InformVictoria).TryApply(state), Is.True);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
            FuneralNightSaveValidation.Validate(state);
        }

        [Test]
        public void NegativeAuthoredCostRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ActionCost.Fixed(-1));
        }
    }
}
