using Inheritance.State;
using NUnit.Framework;
using static Inheritance.State.FuneralNightContent;
using static Inheritance.State.FuneralNightConsequences;

namespace Inheritance.Tests
{
    public sealed class ConsequenceTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void EitherChoiceConsumesEveningAndRejectsBothRepeats(bool family)
        {
            var state = CreateState();
            Assert.That((family ? ChooseElena : ChooseCharlie).TryApply(state), Is.True);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.EqualTo(family));
            Assert.That(state.GetFact(BreckenMetCharlie), Is.EqualTo(!family));
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(family ? -1 : 1));
            Assert.That(state.GetRelationship(Charlie, Resentment), Is.EqualTo(family ? 0 : -1));
            Assert.That(state.GetOpportunityState(CharlieBusiness),
                Is.EqualTo(family ? OpportunityState.Missed : OpportunityState.Completed));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(state.Day, Is.Zero);
            Assert.That(state.StoryPhase, Is.EqualTo(AfterFuneralNight));
            Assert.That(ChooseElena.TryApply(state), Is.False);
            Assert.That(ChooseCharlie.TryApply(state), Is.False);
            Assert.That(state.GetRelationship(Elena, Resentment), Is.EqualTo(family ? -1 : 1));
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
        }

        [TestCase(false, true, AfterFuneralNight)]
        [TestCase(true, false, AfterFuneralNight)]
        [TestCase(true, true, FuneralNight)]
        public void CharlieVerificationRequiresTruthKnowledgeAndLaterPhase(bool truth, bool knowledge, string phase)
        {
            var state = new GameState(phase);
            state.SetFact(BreckenMetCharlie, truth);
            if (knowledge) state.GrantKnowledge(Victoria, BreckenMetCharlie);
            Assert.That(VictoriaBusinessBranch.IsMet(state), Is.False);
        }

        [Test]
        public void CharliePropagationIsExplicitGuardedAndSpecific()
        {
            var state = CreateState();
            Assert.That(InformVictoriaAboutCharlie.TryApply(state), Is.False);
            ChooseCharlie.TryApply(state);
            Assert.That(VictoriaBusinessBranch.IsMet(state), Is.False);
            state.SetStoryPhase(FuneralNight);
            Assert.That(InformVictoriaAboutCharlie.TryApply(state), Is.False);
            state.SetStoryPhase(AfterFuneralNight);
            Assert.That(InformVictoria.TryApply(state), Is.False);
            Assert.That(InformVictoriaAboutCharlie.TryApply(state), Is.True);
            Assert.That(InformVictoriaAboutCharlie.TryApply(state), Is.False);
            Assert.That(VictoriaBusinessBranch.IsMet(state), Is.True);
            Assert.That(VictoriaBranch.IsMet(state), Is.False);
            Assert.That(state.Knows(Victoria, BreckenComfortedElena), Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InkChoiceAndVictoriaLineUseBackendAndKnowledge(bool family)
        {
            var ink = UnityEditor.AssetDatabase.LoadAssetAtPath<Ink.UnityIntegration.InkFile>(
                "Assets/Dialogue/ElenaPrototype.ink");
            var state = CreateState();
            var conversation = new Inheritance.Dialogue.ElenaInkConversation(ink.storyJson, state,
                family ? "elena" : "charlie");
            conversation.ContinueToChoice();
            Assert.That(conversation.Choices.Count, Is.EqualTo(2));
            conversation.Choose(0);
            Assert.That(state.GetFact(family ? BreckenComfortedElena : BreckenMetCharlie), Is.True);
            var uninformed = new Inheritance.Dialogue.ElenaInkConversation(ink.storyJson, state, "victoria");
            Assert.That(uninformed.ContinueToChoice(), Does.Contain("not heard"));
            (family ? InformVictoria : InformVictoriaAboutCharlie).TryApply(state);
            var informed = new Inheritance.Dialogue.ElenaInkConversation(ink.storyJson, state, "victoria");
            Assert.That(informed.ContinueToChoice(), Does.Contain(family
                ? "I see you have chosen family over business."
                : "Your mind is on business instead of family."));
            var other = new Inheritance.Dialogue.ElenaInkConversation(ink.storyJson, state,
                family ? "charlie" : "elena");
            other.ContinueToChoice();
            Assert.That(other.Choices.Count, Is.Zero);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InkLeavingDoesNotSpendTheChoice(bool family)
        {
            var ink = UnityEditor.AssetDatabase.LoadAssetAtPath<Ink.UnityIntegration.InkFile>(
                "Assets/Dialogue/ElenaPrototype.ink");
            var state = CreateState();
            var conversation = new Inheritance.Dialogue.ElenaInkConversation(ink.storyJson, state,
                family ? "elena" : "charlie");
            conversation.ContinueToChoice();
            conversation.Choose(1);
            Assert.That(state.GetFact(BreckenComfortedElena), Is.False);
            Assert.That(state.GetFact(BreckenMetCharlie), Is.False);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ContradictoryTruthCannotInformOrVerifyVictoria(bool family)
        {
            var state = new GameState(AfterFuneralNight);
            state.SetFact(BreckenComfortedElena, true);
            state.SetFact(BreckenMetCharlie, true);
            Assert.That((family ? InformVictoria : InformVictoriaAboutCharlie).TryApply(state), Is.False);
            state.GrantKnowledge(Victoria, BreckenComfortedElena);
            state.GrantKnowledge(Victoria, BreckenMetCharlie);
            Assert.That((family ? VictoriaBranch : VictoriaBusinessBranch).IsMet(state), Is.False);
        }
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
            // Keep opposite truth false so each case isolates its missing prerequisite.
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
