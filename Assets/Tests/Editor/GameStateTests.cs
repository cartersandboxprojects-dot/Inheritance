using System;
using Inheritance.State;
using NUnit.Framework;
using static Inheritance.State.FuneralNightContent;

namespace Inheritance.Tests
{
    public sealed class GameStateTests
    {
        [Test]
        public void WorldTruthDoesNotGrantKnowledge()
        {
            var state = CreateState();
            state.SetFact(BreckenComfortedElena, true);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.True);
            Assert.That(state.GetFact(BreckenMetCharlie), Is.False);
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.False);
            state.SetFact(BreckenMetCharlie, true);
            state.SetFact(BreckenComfortedElena, false);
            Assert.That(state.GetFact(BreckenMetCharlie), Is.True);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.False);
        }

        [Test]
        public void KnowledgeIsPerCharacterAndDoesNotWriteTruth()
        {
            var state = CreateState();
            state.GrantKnowledge(Victoria, BreckenComfortedElena);
            state.GrantKnowledge(Victoria, BreckenComfortedElena);
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.True);
            Assert.That(state.Knows(Elena, BreckenComfortedElena), Is.False);
            Assert.That(state.Knows(Victoria, BreckenMetCharlie), Is.False);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.False);
        }

        [Test]
        public void RelationshipAttributesAndCharactersAreIndependent()
        {
            var state = CreateState();
            state.ModifyRelationship(Elena, Resentment, 2);
            state.ModifyRelationship(Elena, Resentment, -1);
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(1));
            Assert.That(state.GetRelationship(Victoria, Resentment), Is.Zero);
            Assert.That(state.GetRelationship(Elena, "Trust"), Is.Zero);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.False);
        }

        [TestCase(OpportunityState.Unavailable)]
        [TestCase(OpportunityState.Available)]
        [TestCase(OpportunityState.Missed)]
        [TestCase(OpportunityState.Deferred)]
        [TestCase(OpportunityState.Completed)]
        public void OpportunityDoesNotAdvanceTimeOrPhase(OpportunityState value)
        {
            var state = CreateState();
            state.SetOpportunityState(CharlieBusiness, value);
            Assert.That(state.GetOpportunityState(CharlieBusiness), Is.EqualTo(value));
            Assert.That(state.GetOpportunityState("OtherOpportunity"), Is.EqualTo(OpportunityState.Unavailable));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.StoryPhase, Is.EqualTo(FuneralNight));
        }

        [Test]
        public void TimeAndStoryPhaseChangeIndependently()
        {
            var state = CreateState();
            state.AdvanceTime();
            Assert.That(state.Day, Is.EqualTo(1));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Morning));
            Assert.That(state.StoryPhase, Is.EqualTo(FuneralNight));
            state.AdvanceTime();
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Afternoon));
            state.SetStoryPhase(AfterFuneralNight);
            Assert.That(state.StoryPhase, Is.EqualTo(AfterFuneralNight));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Afternoon));
            Assert.That(state.GetOpportunityState(CharlieBusiness), Is.EqualTo(OpportunityState.Available));
        }

        [Test]
        public void FuneralNightWritesRemainSeparateAndInstancesDoNotLeak()
        {
            var state = CreateState();
            state.SetFact(BreckenComfortedElena, true);
            state.ModifyRelationship(Elena, Resentment, -1);
            state.AdvanceTime();
            state.SetOpportunityState(CharlieBusiness, OpportunityState.Missed);
            state.SetStoryPhase(AfterFuneralNight);
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.False);
            state.GrantKnowledge(Victoria, BreckenComfortedElena);
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.True);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.True);
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(-1));
            var fresh = CreateState();
            Assert.That(fresh.GetFact(BreckenComfortedElena), Is.False);
            Assert.That(fresh.GetRelationship(Elena, Resentment), Is.Zero);
            Assert.That(fresh.Knows(Victoria, BreckenComfortedElena), Is.False);
        }

        [Test]
        public void InvalidInputsFailBeforeChangingState()
        {
            var state = CreateState();
            Assert.Throws<ArgumentException>(() => state.SetFact(" ", true));
            Assert.Throws<ArgumentException>(() => state.GrantKnowledge(null, BreckenComfortedElena));
            Assert.Throws<ArgumentException>(() => state.SetStoryPhase(""));
            Assert.Throws<ArgumentOutOfRangeException>(() => state.SetOpportunityState(CharlieBusiness, (OpportunityState)99));
            Assert.Throws<ArgumentOutOfRangeException>(() => new GameState(FuneralNight, (TimeBlock)99));
            Assert.That(state.StoryPhase, Is.EqualTo(FuneralNight));
            Assert.That(state.GetOpportunityState(CharlieBusiness), Is.EqualTo(OpportunityState.Available));
        }
    }
}
