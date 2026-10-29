using System;
using System.IO;
using System.Linq;
using Inheritance.State;
using NUnit.Framework;
namespace Inheritance.Tests
{
    public sealed class DayOneEveningTests
    {
        private static GameState Evening(bool social = false, bool measured = true)
        {
            var s = DayOneMorningContent.CreateState();
            Assert.That(DayOneMorningContent.DeliverCoffee.TryApply(s) && DayOneMorningContent.ReadAgenda.TryApply(s) &&
                DayOneMorningContent.Stay.TryApply(s) && DayOneMorningContent.BreakfastSupport.TryApply(s) &&
                (social ? DayOneMorningContent.SocialDuty : DayOneMorningContent.PersonalLife).TryApply(s) &&
                DayOneAfternoonContent.ChooseOffice(s, 2) && DayOneAfternoonContent.CompleteBoard(s, measured), Is.True);
            return s;
        }
        private static GameState RoundTrip(GameState s)
        {
            string path = Path.Combine(Path.GetTempPath(), "evening-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                StatePersistence.Save(path, s, DayOneEveningContent.Validate, DayOneEveningContent.Catalog());
                var loaded = StatePersistence.Load(path, DayOneEveningContent.Validate, DayOneEveningContent.Catalog());
                Assert.That(StatePersistence.Serialize(loaded), Is.EqualTo(StatePersistence.Serialize(s)));
                Assert.That(loaded.Capture().Version, Is.EqualTo(3)); return loaded;
            }
            finally { if (File.Exists(path)) File.Delete(path); }
        }
        [TestCase(false, true, 0)] [TestCase(false, true, 1)] [TestCase(false, true, 2)]
        [TestCase(true, true, 0)] [TestCase(true, true, 1)] [TestCase(true, true, 2)]
        [TestCase(false, false, 0)] [TestCase(false, false, 1)] [TestCase(false, false, 2)]
        [TestCase(true, false, 0)] [TestCase(true, false, 1)] [TestCase(true, false, 2)]
        public void AuthoredRoutesPersistAndComplete(bool social, bool take, int choice)
        {
            var s = RoundTrip(Evening(social));
            Assert.That(DayOneEveningContent.ChooseFinal(s, choice) || DayOneEveningContent.CloseDay(s), Is.False);
            Assert.That(DayOneEveningContent.Enter(s), Is.True);
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3));
            Assert.That(s.GetOpportunityState(social ? DayOneEveningContent.SocialInvitation : DayOneEveningContent.PersonalInvitation), Is.EqualTo(OpportunityState.Available));
            Assert.That(s.GetOpportunityState(social ? DayOneEveningContent.PersonalInvitation : DayOneEveningContent.SocialInvitation), Is.EqualTo(OpportunityState.Unavailable));
            s = RoundTrip(s);
            Assert.That(take ? DayOneEveningContent.TakeExternal(s) : DayOneEveningContent.DeferExternal(s), Is.True);
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(take ? 1 : 3));
            Assert.That(s.GetOpportunityState(social ? DayOneEveningContent.SocialInvitation : DayOneEveningContent.PersonalInvitation), Is.EqualTo(take ? OpportunityState.Completed : OpportunityState.Deferred));
            Assert.That(s.GetFact(DayOneEveningContent.Taken), Is.EqualTo(take));
            Assert.That(s.GetFact(DayOneEveningContent.Deferred), Is.EqualTo(!take));
            s = RoundTrip(s);
            Assert.That(DayOneEveningContent.Enter(s) || DayOneEveningContent.TakeExternal(s) || DayOneEveningContent.DeferExternal(s), Is.False);
            Assert.That(DayOneEveningContent.ChooseFinal(s, choice), Is.True);
            Assert.That(s.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night));
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3)); Assert.That(s.Day, Is.EqualTo(1));
            Assert.That(s.GetFact(DayOneEveningContent.ElenaLater), Is.EqualTo(choice != 0));
            Assert.That(s.GetFact(DayOneEveningContent.CharlieLater), Is.EqualTo(choice != 1));
            Assert.That(s.Knows(FuneralNightContent.Victoria, DayOneEveningContent.ChoiceFact(choice)), Is.False);
            Assert.That(s.GetFact(DayOneEveningContent.Complete), Is.False);
            s = RoundTrip(s);
            Assert.That(DayOneEveningContent.CloseDay(s), Is.True);
            Assert.That(s.Knows(FuneralNightContent.Victoria, DayOneEveningContent.ChoiceFact(choice)), Is.True);
            Assert.That(s.GetFact(DayOneEveningContent.Acknowledged), Is.True);
            Assert.That(s.GetFact(DayOneEveningContent.Complete), Is.True);
            Assert.That(s.StoryPhase, Is.EqualTo(DayOneEveningContent.CompletePhase));
            Assert.That(s.CurrentTimeBlock, Is.EqualTo(TimeBlock.Night)); Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3));
            Assert.That(s.GetOpportunityState(DayOneAfternoonContent.Merger), Is.EqualTo(OpportunityState.Available));
            Assert.That(DayOneEveningContent.ChooseFinal(s, choice) || DayOneEveningContent.CloseDay(s), Is.False);
            RoundTrip(s);
        }
        [Test]
        public void VictoriaUsesWitnessedConcernAndExplicitReportOnly()
        {
            var s = Evening(true, false); DayOneEveningContent.Enter(s); DayOneEveningContent.TakeExternal(s); DayOneEveningContent.ChooseFinal(s, 1);
            Assert.That(DayOneEveningContent.Reaction(s), Is.EqualTo(DayOneEveningContent.Caution));
            DayOneEveningContent.CloseDay(s);
            foreach (string fact in new[] { DayOneMorningContent.Preparation, DayOneMorningContent.Social, DayOneEveningContent.Social, DayOneEveningContent.Taken, DayOneMorningContent.Support })
                Assert.That(s.Knows(FuneralNightContent.Victoria, fact), Is.False);
            Assert.That(s.Knows(FuneralNightContent.Victoria, DayOneEveningContent.Charlie), Is.True);
            RoundTrip(s);
        }
        [Test]
        public void VictoriaDoesNotInferKnowledgeFromObjectiveFacts()
        {
            var s = DayOneMorningContent.CreateState();
            s.SetFact(DayOneAfternoonContent.Concern, true); s.SetFact(DayOneAfternoonContent.Informed, true);
            s.SetFact(DayOneEveningContent.Social, true); s.SetFact(DayOneEveningContent.Elena, true);
            Assert.That(DayOneEveningContent.Reaction(s), Is.EqualTo(DayOneEveningContent.Unbriefed));
            s.GrantKnowledge(FuneralNightContent.Victoria, DayOneAfternoonContent.Informed);
            Assert.That(DayOneEveningContent.Reaction(s), Is.EqualTo(DayOneEveningContent.Acknowledged));
        }
        [Test]
        public void ValidationRejectsFabricatedKnowledgeCapacityAndCompletion()
        {
            var s = Evening(); DayOneEveningContent.Enter(s);
            var forged = GameState.Restore(s.Capture()); forged.GrantKnowledge(FuneralNightContent.Victoria, DayOneMorningContent.Preparation);
            Assert.Throws<InvalidDataException>(() => DayOneEveningContent.Validate(forged));
            forged = GameState.Restore(s.Capture()); forged.SetFact(DayOneEveningContent.Complete, true);
            Assert.Throws<InvalidDataException>(() => DayOneEveningContent.Validate(forged));
            var snapshot = s.Capture(); snapshot.TimeUnitsRemaining = 1;
            Assert.Throws<InvalidDataException>(() => DayOneEveningContent.Validate(GameState.Restore(snapshot)));
        }
        [Test]
        public void MetadataRemainsProvisionalAndValidationStrict()
        {
            var catalog = DayOneEveningContent.Catalog();
            Assert.That(catalog.Validate(ContentValidationMode.Runtime).Any(d => d.IsError), Is.False);
            foreach (ContentValidationMode mode in Enum.GetValues(typeof(ContentValidationMode)))
            {
                var diagnostics = catalog.Validate(mode);
                Assert.That(diagnostics.Any(d => d.IsError), Is.False);
                foreach (string id in new[] { DayOneEveningContent.MinorRelative, DayOneEveningContent.PersonalInvitation, DayOneEveningContent.SocialInvitation })
                    Assert.That(diagnostics.Any(d => d.Message.Contains(id) && d.Message.Contains("provisional")), Is.True);
            }
            var s = Evening(); s.SetFact("unknown.evening", true);
            Assert.Throws<InvalidDataException>(() => StatePersistence.Serialize(s, catalog: catalog));
        }
        [Test]
        public void InvalidOrderAndChoiceDoNotMutate()
        {
            var s = Evening(); var before = StatePersistence.Serialize(s);
            Assert.That(DayOneEveningContent.TakeExternal(s) || DayOneEveningContent.DeferExternal(s) || DayOneEveningContent.ChooseFinal(s, -1) || DayOneEveningContent.CloseDay(s), Is.False);
            Assert.That(StatePersistence.Serialize(s), Is.EqualTo(before));
        }
    }
}
