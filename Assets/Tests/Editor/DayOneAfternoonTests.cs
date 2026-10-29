using System;
using System.IO;
using System.Linq;
using Inheritance.State;
using NUnit.Framework;

namespace Inheritance.Tests
{
    public sealed class DayOneAfternoonTests
    {
        private static GameState Morning(bool paper, bool business = false)
        {
            var s = DayOneMorningContent.CreateState();
            DayOneMorningContent.DeliverCoffee.TryApply(s); DayOneMorningContent.ReadAgenda.TryApply(s);
            (paper ? DayOneMorningContent.Stay : DayOneMorningContent.Leave).TryApply(s);
            (business ? DayOneMorningContent.BreakfastBusiness : DayOneMorningContent.BreakfastSupport).TryApply(s);
            DayOneMorningContent.SocialDuty.TryApply(s); return s;
        }
        [TestCase(0, true)] [TestCase(0, false)]
        [TestCase(1, true)] [TestCase(1, false)]
        [TestCase(2, true)] [TestCase(2, false)]
        public void RoutesCostOneBoardCostsTwoAndRoundTrip(int route, bool paper)
        {
            var s = Morning(paper);
            Assert.That(s.CurrentTimeBlock, Is.EqualTo(TimeBlock.Afternoon));
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3));
            Assert.That(DayOneAfternoonContent.CompleteBoard(s, true), Is.False);
            Assert.That(DayOneAfternoonContent.ChooseOffice(s, route), Is.True);
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(2));
            Assert.That(s.StoryPhase, Is.EqualTo(DayOneAfternoonContent.BoardPending));
            for (int i = 0; i < 3; i++) Assert.That(DayOneAfternoonContent.ChooseOffice(s, i), Is.False);
            Assert.That(s.GetFact(DayOneAfternoonContent.SecretarySeed), Is.EqualTo(route == 0));
            Assert.That(s.GetFact(DayOneAfternoonContent.SecretaryDelayable), Is.True);
            Assert.That(s.GetRelationship(DayOneAfternoonContent.Secretary, DayOneMorningContent.Rapport), Is.EqualTo(route == 0 ? 1 : 0));
            Assert.That(s.GetFact(DayOneAfternoonContent.HalePrepared), Is.EqualTo(route == 1 && paper));
            Assert.That(s.GetFact(DayOneAfternoonContent.HaleBasic), Is.EqualTo(route == 1 && !paper));
            Assert.That(s.GetOpportunityState(DayOneAfternoonContent.Merger), Is.EqualTo(route == 2 ? OpportunityState.Available : OpportunityState.Unavailable));
            Assert.That(DayOneAfternoonContent.AcquisitionOpportunity.TryComplete(s), Is.False);
            Assert.That(s.Knows(DayOneMorningContent.Richard, DayOneMorningContent.StockDrop), Is.False);
            Assert.That(s.Knows(FuneralNightContent.Victoria, DayOneMorningContent.Preparation), Is.False);
            s = RoundTrip(s);
            Assert.That(DayOneAfternoonContent.CompleteBoard(s, true), Is.True);
            Assert.That(s.Day, Is.EqualTo(1));
            Assert.That(s.TimeUnitsRemaining, Is.EqualTo(3));
            Assert.That(s.CurrentTimeBlock, Is.EqualTo(TimeBlock.Evening));
            Assert.That(s.StoryPhase, Is.EqualTo(DayOneAfternoonContent.EveningPending));
            Assert.That(s.GetFact(DayOneMorningContent.OfficeRequired), Is.False);
            Assert.That(s.GetFact(DayOneAfternoonContent.Informed), Is.EqualTo(paper || route == 1));
            Assert.That(s.GetFact(DayOneAfternoonContent.BoardBriefed), Is.EqualTo(route == 1 && paper));
            Assert.That(s.GetFact(DayOneAfternoonContent.Listening), Is.EqualTo(!paper && route != 1));
            Assert.That(DayOneAfternoonContent.CompleteBoard(s, false), Is.False);
            RoundTrip(s);
        }
        private static GameState RoundTrip(GameState s)
        {
            string path = Path.Combine(Path.GetTempPath(), "afternoon-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                StatePersistence.Save(path, s, DayOneAfternoonContent.Validate, DayOneAfternoonContent.Catalog());
                var loaded = StatePersistence.Load(path, DayOneAfternoonContent.Validate, DayOneAfternoonContent.Catalog());
                Assert.That(StatePersistence.Serialize(loaded), Is.EqualTo(StatePersistence.Serialize(s)));
                Assert.That(loaded.Capture().Version, Is.EqualTo(3)); return loaded;
            }
            finally { if (File.Exists(path)) File.Delete(path); }
        }
        [Test]
        public void DismissalCreatesConcernDespitePreparation()
        {
            var s = Morning(true); DayOneAfternoonContent.ChooseOffice(s, 1);
            Assert.That(DayOneAfternoonContent.CompleteBoard(s, false), Is.True);
            Assert.That(s.GetFact(DayOneAfternoonContent.Concern), Is.True);
            Assert.That(s.GetFact(DayOneAfternoonContent.Informed), Is.False);
            foreach (string c in new[] { DayOneMorningContent.Richard, FuneralNightContent.Victoria })
            {
                Assert.That(s.Knows(c, DayOneAfternoonContent.Concern), Is.True);
                Assert.That(s.GetRelationship(c, DayOneMorningContent.Rapport), Is.EqualTo(-1));
                Assert.That(s.Knows(c, DayOneMorningContent.StockDrop), Is.False);
            }
            RoundTrip(s);
        }
        [Test]
        public void CharlieContextSupportsMeasuredFirstImpression()
        {
            var s = Morning(false, true); DayOneAfternoonContent.ChooseOffice(s, 0);
            DayOneAfternoonContent.CompleteBoard(s, true);
            Assert.That(s.GetFact(DayOneAfternoonContent.Informed), Is.True); RoundTrip(s);
        }
        [Test]
        public void ClockAloneAndMissingObligationDoNotUnlockOfficeOrBoard()
        {
            var s = DayOneMorningContent.CreateState(); s.AdvanceTime();
            for (int i = 0; i < 3; i++) Assert.That(DayOneAfternoonContent.ChooseOffice(s, i), Is.False);
            s = Morning(true); s.SetFact(DayOneMorningContent.OfficeRequired, false);
            Assert.That(DayOneAfternoonContent.ChooseOffice(s, 1), Is.False);
            s = Morning(true); DayOneAfternoonContent.ChooseOffice(s, 1); s.SetFact(DayOneMorningContent.OfficeRequired, false);
            Assert.That(DayOneAfternoonContent.CompleteBoard(s, true), Is.False);
        }
        [Test]
        public void CatalogProvisionalMetadataAndHistoryRemainStrict()
        {
            foreach (ContentValidationMode mode in Enum.GetValues(typeof(ContentValidationMode)))
            {
                var results = DayOneAfternoonContent.Catalog().Validate(mode);
                Assert.That(results.Any(d => d.IsError), Is.False);
                foreach (string id in new[] { DayOneAfternoonContent.Secretary, DayOneAfternoonContent.VP, DayOneAfternoonContent.Merger })
                    Assert.That(results.Any(d => d.Message.Contains(id) && d.Message.Contains("provisional")), Is.True);
            }
            var s = Morning(false); DayOneAfternoonContent.ChooseOffice(s, 0);
            var altered = GameState.Restore(s.Capture()); altered.SetFact(DayOneAfternoonContent.HalePrepared, true);
            Assert.Throws<InvalidDataException>(() => DayOneAfternoonContent.Validate(altered));
            altered = GameState.Restore(s.Capture()); altered.GrantKnowledge(FuneralNightContent.Victoria, DayOneMorningContent.Preparation);
            Assert.Throws<InvalidDataException>(() => DayOneAfternoonContent.Validate(altered));
            altered = GameState.Restore(s.Capture()); altered.TryApplyCost(ActionCost.Fixed(1));
            Assert.Throws<InvalidDataException>(() => DayOneAfternoonContent.Validate(altered));
            altered = GameState.Restore(s.Capture()); altered.SetOpportunityState(DayOneAfternoonContent.Merger, OpportunityState.Completed);
            Assert.Throws<InvalidDataException>(() => DayOneAfternoonContent.Validate(altered));
            s.SetFact("unknown.afternoon", true);
            Assert.Throws<InvalidDataException>(() => StatePersistence.Serialize(s, catalog: DayOneAfternoonContent.Catalog()));
        }
        [Test]
        public void AcquisitionOpeningPersistsIntoLaterDayWithoutImplicitResolution()
        {
            var s = Morning(false); DayOneAfternoonContent.ChooseOffice(s, 2); DayOneAfternoonContent.CompleteBoard(s, true);
            s.AdvanceTime(); s.AdvanceTime();
            Assert.That(s.Day, Is.EqualTo(2));
            Assert.That(s.GetOpportunityState(DayOneAfternoonContent.Merger), Is.EqualTo(OpportunityState.Available));
            Assert.That(DayOneAfternoonContent.AcquisitionOpportunity.TryComplete(s), Is.False);
        }
    }
}
