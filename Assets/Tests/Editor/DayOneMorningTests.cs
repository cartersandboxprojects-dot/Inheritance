using System;
using System.IO;
using System.Linq;
using Inheritance.State;
using NUnit.Framework;

namespace Inheritance.Tests
{
    public sealed class DayOneMorningTests
    {
        private static GameState Briefed()
        {
            var s = DayOneMorningContent.CreateState();
            Assert.That(s.Day, Is.EqualTo(1));
            Assert.That(DayOneMorningContent.ReadAgenda.TryApply(s), Is.False);
            Assert.That(DayOneMorningContent.Stay.TryApply(s), Is.False);
            Assert.That(DayOneMorningContent.DeliverCoffee.TryApply(s), Is.True);
            Assert.That(DayOneMorningContent.DeliverCoffee.TryApply(s), Is.False);
            Assert.That(DayOneMorningContent.ReadAgenda.TryApply(s), Is.True);
            Assert.That(DayOneMorningContent.ReadAgenda.TryApply(s), Is.False);
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3), "Zero-cost introduction stays free");
            return s;
        }
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void MorningRoutesPersistAndStopAtAfternoon(bool stay, bool support, bool social)
        {
            var s = Briefed();
            Assert.That(DayOneMorningContent.BreakfastSupport.TryApply(s), Is.False);
            Assert.That(DayOneMorningContent.SocialDuty.TryApply(s), Is.False);
            Assert.That((stay ? DayOneMorningContent.Stay : DayOneMorningContent.Leave).TryApply(s), Is.True);
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(2));
            Assert.That(s.CurrentTimeBlock, Is.EqualTo(TimeBlock.Morning));
            Assert.That(s.GetFact(DayOneMorningContent.Coverage) && s.GetFact(DayOneMorningContent.StockDrop), Is.True);
            Assert.That(s.Knows(DayOneMorningContent.Brecken, DayOneMorningContent.StockDrop), Is.EqualTo(stay));
            Assert.That(s.Knows(DayOneMorningContent.Richard, DayOneMorningContent.StockDrop), Is.False);
            Assert.That(s.GetFact(DayOneMorningContent.Seed), Is.EqualTo(!stay));
            Assert.That(s.GetRelationship(DayOneMorningContent.Family1, DayOneMorningContent.Rapport), Is.EqualTo(stay ? 0 : 1));
            Assert.That(s.Knows(FuneralNightContent.Victoria, DayOneMorningContent.Seed), Is.False);
            Assert.That(DayOneMorningContent.Stay.TryApply(s) || DayOneMorningContent.Leave.TryApply(s), Is.False);
            Assert.That(DayOneMorningContent.PersonalLife.TryApply(s), Is.False, "Breakfast cannot be skipped");
            string path = Path.Combine(Path.GetTempPath(), "inheritance-morning-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                s = RoundTrip(path, s);
                Assert.That((support ? DayOneMorningContent.BreakfastSupport : DayOneMorningContent.BreakfastBusiness).TryApply(s), Is.True);
                Assert.That(s.TimeUnitsRemaining, Is.EqualTo(1));
                Assert.That(s.GetFact(DayOneMorningContent.Breakfast), Is.True);
                Assert.That(s.GetFact(support ? DayOneMorningContent.Support : DayOneMorningContent.Business), Is.True);
                Assert.That(s.GetRelationship(support ? FuneralNightContent.Elena : FuneralNightContent.Charlie, DayOneMorningContent.Rapport), Is.EqualTo(1));
                Assert.That(DayOneMorningContent.BreakfastSupport.TryApply(s) || DayOneMorningContent.BreakfastBusiness.TryApply(s), Is.False);
                s = RoundTrip(path, s);
                Assert.That((social ? DayOneMorningContent.SocialDuty : DayOneMorningContent.PersonalLife).TryApply(s), Is.True);
                Assert.That(s.Day, Is.EqualTo(1));
                Assert.That(s.CurrentTimeBlock, Is.EqualTo(TimeBlock.Afternoon));
                Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3));
                Assert.That(DayOneMorningContent.ReadyForOffice.IsMet(s), Is.True);
                Assert.That(s.GetFact(social ? DayOneMorningContent.Social : DayOneMorningContent.Personal), Is.True);
                Assert.That(s.GetRelationship(social ? DayOneMorningContent.Adrian : DayOneMorningContent.FormerGirlfriend, DayOneMorningContent.Rapport), Is.EqualTo(1));
                Assert.That(DayOneMorningContent.SocialDuty.TryApply(s) || DayOneMorningContent.PersonalLife.TryApply(s), Is.False);
                RoundTrip(path, s);
            }
            finally { if (File.Exists(path)) File.Delete(path); }
        }
        private static GameState RoundTrip(string path, GameState state)
        {
            StatePersistence.Save(path, state, DayOneMorningContent.Validate, DayOneMorningContent.Catalog());
            var loaded = StatePersistence.Load(path, DayOneMorningContent.Validate, DayOneMorningContent.Catalog());
            Assert.That(StatePersistence.Serialize(loaded), Is.EqualTo(StatePersistence.Serialize(state)));
            Assert.That(loaded.Capture().Version, Is.EqualTo(3)); return loaded;
        }
        [Test]
        public void ProvisionalContentResolvesStrictlyAndUnknownIdsReject()
        {
            foreach (ContentValidationMode mode in Enum.GetValues(typeof(ContentValidationMode)))
            {
                var result = DayOneMorningContent.Catalog().Validate(mode);
                Assert.That(result.Any(d => d.IsError), Is.False);
                Assert.That(result.Any(d => d.Message.Contains(DayOneMorningContent.FormerGirlfriend) && d.Message.Contains("provisional")), Is.True);
            }
            var s = Briefed(); s.SetFact("unknown.fact", true);
            Assert.Throws<InvalidDataException>(() => StatePersistence.Serialize(s, catalog: DayOneMorningContent.Catalog()));
        }
        [Test]
        public void SaveHistoryRejectsSkippedBreakfastWrongCapacityAndFabricatedKnowledge()
        {
            var s = Briefed(); DayOneMorningContent.Leave.TryApply(s);
            DayOneMorningContent.Validate(s);
            var skipped = GameState.Restore(s.Capture()); skipped.SetStoryPhase(DayOneMorningContent.Final);
            Assert.Throws<InvalidDataException>(() => DayOneMorningContent.Validate(skipped));
            var wrongBudget = GameState.Restore(s.Capture()); wrongBudget.TryApplyCost(ActionCost.Fixed(1));
            Assert.Throws<InvalidDataException>(() => DayOneMorningContent.Validate(wrongBudget));
            s.GrantKnowledge(DayOneMorningContent.Brecken, DayOneMorningContent.StockDrop);
            Assert.Throws<InvalidDataException>(() => DayOneMorningContent.Validate(s));
        }
        [Test]
        public void FreeBootstrapStagesRoundTripAndUnrelatedAfternoonCannotUnlockOffice()
        {
            var s = DayOneMorningContent.CreateState();
            foreach (var action in new[] { DayOneMorningContent.DeliverCoffee, DayOneMorningContent.ReadAgenda })
            {
                s = StatePersistence.Deserialize(StatePersistence.Serialize(s), DayOneMorningContent.Validate, DayOneMorningContent.Catalog());
                Assert.That(action.TryApply(s), Is.True);
                Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3));
            }
            var unrelated = DayOneMorningContent.CreateState(); unrelated.AdvanceTime();
            Assert.That(DayOneMorningContent.ReadyForOffice.IsMet(unrelated), Is.False);
        }
    }
}
