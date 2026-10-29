using System.IO;
using System.Linq;
using Inheritance.State;
using NUnit.Framework;

namespace Inheritance.Tests
{
    public class SandboxDayTests
    {
        private static GameState Morning()
        {
            var state = SandboxDayContent.CreateState();
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Morning));
            Assert.That(SandboxDayContent.Family.CanStart(state), Is.False);
            Assert.That(SandboxDayContent.Greet.TryApply(state), Is.True);
            Assert.That(SandboxDayContent.Brief.TryApply(state), Is.True);
            SandboxDayContent.Evaluate(state);
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3), "Both conversations are free");
            return state;
        }
        [TestCase(true)]
        [TestCase(false)]
        public void FullDayBranchesSaveReloadAndReact(bool family)
        {
            var state = Morning();
            var choice = family ? SandboxDayContent.Family : SandboxDayContent.Business;
            var other = family ? SandboxDayContent.Business : SandboxDayContent.Family;
            var message = family ? SandboxDayContent.FamilyMessage : SandboxDayContent.BusinessMessage;
            var reaction = family ? SandboxDayContent.FamilyReaction : SandboxDayContent.BusinessReaction;
            Assert.That(choice.TryComplete(state), Is.True);
            SandboxDayContent.Evaluate(state);
            Assert.That(state.GetOpportunityState(other.Id), Is.EqualTo(family ? OpportunityState.Missed : OpportunityState.Deferred));
            Assert.That(other.TryComplete(state), Is.False);
            Assert.That(state.CurrentTimeBlock, Is.EqualTo(family ? TimeBlock.Morning : TimeBlock.Afternoon));
            Assert.That(state.TimeUnitsRemaining, Is.EqualTo(family ? 1 : 3));
            Assert.That(state.GetMessageStatus(message.Id), Is.EqualTo(family ? MessageStatus.Queued : MessageStatus.Delivered));
            Assert.That(state.Knows(FuneralNightContent.Victoria, family ? SandboxDayContent.FamilyFact : SandboxDayContent.BusinessFact), Is.False);
            // Real disk save/load through runtime catalog; compare every persisted field.
            string path = Path.Combine(Path.GetTempPath(), "inheritance-day-" + System.Guid.NewGuid().ToString("N") + ".json");
            try
            {
                StatePersistence.Save(path, state, catalog: SandboxDayContent.Catalog());
                string before = StatePersistence.Serialize(state);
                state = StatePersistence.Load(path, catalog: SandboxDayContent.Catalog());
                SandboxDayContent.Evaluate(state);
                Assert.That(StatePersistence.Serialize(state), Is.EqualTo(before));
                if (family)
                {
                    Assert.That(message.TryRead(state), Is.False, "Queued is not readable");
                    Assert.That(SandboxDayContent.FinishMorning.TryApply(state), Is.True);
                    Assert.That(state.GetMessageStatus(message.Id), Is.EqualTo(MessageStatus.Queued), "No implicit scheduler");
                    SandboxDayContent.Evaluate(state);
                }
                Assert.That(reaction.TryApply(state), Is.False, "Too early and unread");
                Assert.That(SandboxDayContent.FinishAfternoon.TryApply(state), Is.True);
                Assert.That(reaction.TryApply(state), Is.False, "Evening still needs read message");
                Assert.That(message.TryRead(state), Is.True, "Delivered message remains readable in evening");
                Assert.That(state.TimeUnitsRemaining, Is.EqualTo(3));
                Assert.That(reaction.TryApply(state), Is.True);
                Assert.That(state.Knows(FuneralNightContent.Victoria, family ? SandboxDayContent.FamilyFact : SandboxDayContent.BusinessFact), Is.True);
                Assert.That(reaction.TryApply(state), Is.False);
                Assert.That(other.TryComplete(state), Is.False);
                Assert.That(SandboxDayContent.EndDay.TryApply(state), Is.True);
                Assert.That(state.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
                Assert.That(state.GetFact(SandboxDayContent.EndFact), Is.True);
                Assert.That(SandboxDayContent.EndDay.TryApply(state), Is.False);
                StatePersistence.Save(path, state, catalog: SandboxDayContent.Catalog());
                Assert.That(StatePersistence.Serialize(StatePersistence.Load(path, catalog: SandboxDayContent.Catalog())), Is.EqualTo(StatePersistence.Serialize(state)));
            }
            finally { if (File.Exists(path)) File.Delete(path); }
        }
        [Test]
        public void ProvisionalCatalogResolvesAllReferencesInEveryMode()
        {
            foreach (ContentValidationMode mode in System.Enum.GetValues(typeof(ContentValidationMode)))
            {
                var diagnostics = SandboxDayContent.Catalog().Validate(mode);
                Assert.That(diagnostics.Any(d => d.IsError), Is.False);
                Assert.That(diagnostics.All(d => d.Message.Contains("provisional")), Is.True);
                Assert.That(diagnostics.Count, Is.GreaterThan(0));
            }
        }
        [Test]
        public void ReactionRequiresTruthKnowledgeAndCompetitionOutcome()
        {
            var state = Morning(); SandboxDayContent.Family.TryComplete(state); SandboxDayContent.Evaluate(state);
            SandboxDayContent.FinishMorning.TryApply(state); SandboxDayContent.Evaluate(state);
            SandboxDayContent.FamilyMessage.TryRead(state); SandboxDayContent.FinishAfternoon.TryApply(state);
            Assert.That(SandboxDayContent.FamilyReaction.TryApply(GameState.Restore(state.Capture())), Is.True);
            var withoutKnowledge = state.Capture(); withoutKnowledge.Knowledge = new KnowledgeSave[0];
            Assert.That(SandboxDayContent.FamilyReaction.TryApply(GameState.Restore(withoutKnowledge)), Is.False);
            var falseFact = GameState.Restore(state.Capture()); falseFact.SetFact(SandboxDayContent.FamilyFact, false);
            Assert.That(SandboxDayContent.FamilyReaction.TryApply(falseFact), Is.False);
            state.SetOpportunityState(SandboxDayContent.BusinessId, OpportunityState.Available);
            Assert.That(SandboxDayContent.FamilyReaction.TryApply(state), Is.False);
        }
    }
}
