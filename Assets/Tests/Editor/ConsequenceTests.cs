using Inheritance.State;
using NUnit.Framework;
using static Inheritance.State.FuneralNightContent;
using static Inheritance.State.FuneralNightConsequences;

namespace Inheritance.Tests
{
    public sealed class ConsequenceTests
    {
        [Test]
        public void AllInspectsEveryStateCategoryWithoutMutation()
        {
            var state = CreateState();
            state.SetFact(BreckenComfortedElena, true);
            state.GrantKnowledge(Victoria, BreckenComfortedElena);
            state.ModifyRelationship(Elena, Resentment, -1);
            var conditions = new[] {
                Condition.Fact(BreckenComfortedElena),
                Condition.Knowledge(Victoria, BreckenComfortedElena),
                Condition.RelationshipAtMost(Elena, Resentment, -1),
                Condition.Opportunity(CharlieBusiness, OpportunityState.Available),
                Condition.Time(TimeBlock.Evening), Condition.Phase(FuneralNight) };
            Assert.That(Condition.All(conditions).IsMet(state), Is.True);
            conditions[0] = Condition.Fact(BreckenMetCharlie);
            Assert.That(Condition.All(conditions).IsMet(state), Is.False);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(-1));
        }

        [Test]
        public void ElenaChoiceAppliesImmediateEffectsOnce()
        {
            var state = CreateState();
            Assert.That(InformVictoria.TryApply(state), Is.False);
            Assert.That(ChooseElena.TryApply(state), Is.True);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.True);
            Assert.That(state.GetFact(BreckenMetCharlie), Is.False);
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(-1));
            Assert.That(state.GetOpportunityState(CharlieBusiness), Is.EqualTo(OpportunityState.Missed));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.Day, Is.Zero);
            Assert.That(state.StoryPhase, Is.EqualTo(AfterFuneralNight));
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.False);
            Assert.That(ChooseElena.TryApply(state), Is.False);
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(-1));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
        }

        [Test]
        public void LaterInformationEnablesVictoriaBranch()
        {
            var state = CreateState();
            ChooseElena.TryApply(state);
            Assert.That(VictoriaBranch.IsMet(state), Is.False);
            Assert.That(InformVictoria.TryApply(state), Is.True);
            Assert.That(VictoriaBranch.IsMet(state), Is.True);
            Assert.That(InformVictoria.TryApply(state), Is.False);
            Assert.That(state.Knows(Elena, BreckenComfortedElena), Is.False);
        }

        [TestCase(false, true, AfterFuneralNight)]
        [TestCase(true, false, AfterFuneralNight)]
        [TestCase(true, true, FuneralNight)]
        public void EachRequiredVictoriaBranchTermMustHold(bool fact, bool knowledge, string phase)
        {
            var state = new GameState(phase);
            state.SetFact(BreckenComfortedElena, fact);
            if (knowledge) state.GrantKnowledge(Victoria, BreckenComfortedElena);
            state.SetFact(BreckenMetCharlie, true);
            state.GrantKnowledge(Elena, BreckenComfortedElena);
            state.GrantKnowledge(Victoria, BreckenMetCharlie);
            state.ModifyRelationship(Victoria, Resentment, -5);
            state.SetOpportunityState(CharlieBusiness, OpportunityState.Completed);
            Assert.That(VictoriaBranch.IsMet(state), Is.False);
        }

        [Test]
        public void FailedChoiceDoesNotApplyAnyEffects()
        {
            var state = CreateState();
            state.SetOpportunityState(CharlieBusiness, OpportunityState.Completed);
            Assert.That(ChooseElena.TryApply(state), Is.False);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.False);
            Assert.That(state.GetRelationship(Elena, Resentment), Is.Zero);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(state.StoryPhase, Is.EqualTo(FuneralNight));
            Assert.That(state.GetOpportunityState(CharlieBusiness), Is.EqualTo(OpportunityState.Completed));
        }

        [Test]
        public void PropagationRequiresTruthAndLaterPhase()
        {
            var state = new GameState(AfterFuneralNight);
            Assert.That(InformVictoria.TryApply(state), Is.False);
            state.SetFact(BreckenComfortedElena, true);
            state.SetStoryPhase(FuneralNight);
            Assert.That(InformVictoria.TryApply(state), Is.False);
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.False);
        }

        [Test]
        public void DefinitionsCopyArraysAndRejectNullMembers()
        {
            var state = CreateState();
            var conditions = new[] { Condition.Fact(BreckenComfortedElena, false) };
            var all = Condition.All(conditions);
            conditions[0] = Condition.Fact(BreckenComfortedElena);
            Assert.That(all.IsMet(state), Is.True);
            var effects = new[] { Effect.SetFact(BreckenComfortedElena, true) };
            var consequence = new Consequence(all, effects);
            effects[0] = Effect.SetFact(BreckenMetCharlie, true);
            Assert.That(consequence.TryApply(state), Is.True);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.True);
            Assert.That(state.GetFact(BreckenMetCharlie), Is.False);
            Assert.Throws<System.ArgumentException>(() => Condition.All(new Condition[] { null }));
            Assert.Throws<System.ArgumentException>(() => new Consequence(all, new Effect[] { null }));
        }
    }
}
