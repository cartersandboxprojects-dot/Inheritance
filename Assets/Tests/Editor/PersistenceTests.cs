using System;
using System.IO;
using System.Runtime.Serialization;
using Inheritance.State;
using NUnit.Framework;
using static Inheritance.State.FuneralNightContent;

namespace Inheritance.Tests
{
    public sealed class PersistenceTests
    {
        private static GameState RoundTrip(GameState state) =>
            StatePersistence.Deserialize(StatePersistence.Serialize(state));

        [Test]
        public void RoundTripPreservesAllGenericStateAndDoesNotAlias()
        {
            var original = new GameState("OtherPhase", TimeBlock.Night);
            original.AdvanceTime();
            original.SetFact("FalseFact", false);
            original.SetFact("Unicode_é", true);
            original.ModifyRelationship("A", "Trust", int.MinValue);
            original.ModifyRelationship("B", "Trust", int.MaxValue);
            original.GrantKnowledge("A", "FalseFact");
            foreach (OpportunityState value in Enum.GetValues(typeof(OpportunityState)))
                original.SetOpportunityState(value.ToString(), value);
            string json = StatePersistence.Serialize(original);
            var loaded = RoundTrip(original);
            Assert.That(StatePersistence.Serialize(loaded), Is.EqualTo(json));
            Assert.That(loaded.Day, Is.EqualTo(1));
            Assert.That(loaded.CurrentTimeBlock, Is.EqualTo(TimeBlock.Morning));
            Assert.That(loaded.Knows("A", "FalseFact"), Is.True);
            Assert.That(loaded.GetFact("FalseFact"), Is.False);
            loaded.SetFact("Unicode_é", false);
            Assert.That(original.GetFact("Unicode_é"), Is.True);
            var snapshot = original.Capture();
            snapshot.Facts[0].Value = true;
            Assert.That(original.GetFact("FalseFact"), Is.False);
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void BothBranchesRetainTruthAndKnowledgeAndRejectReplay(bool business, bool reported)
        {
            var state = CreateState();
            Assert.That((business ? FuneralNightConsequences.ChooseCharlie :
                FuneralNightConsequences.ChooseElena).TryApply(state), Is.True);
            var inform = business ? FuneralNightConsequences.InformVictoriaAboutCharlie :
                FuneralNightConsequences.InformVictoria;
            var branch = business ? FuneralNightConsequences.VictoriaBusinessBranch :
                FuneralNightConsequences.VictoriaBranch;
            if (reported) inform.TryApply(state);
            var loaded = StatePersistence.Deserialize(StatePersistence.Serialize(state,
                FuneralNightSaveValidation.Validate), FuneralNightSaveValidation.Validate);
            Assert.That(branch.IsMet(loaded), Is.EqualTo(reported));
            Assert.That(FuneralNightConsequences.ChooseElena.TryApply(loaded), Is.False);
            Assert.That(FuneralNightConsequences.ChooseCharlie.TryApply(loaded), Is.False);
            Assert.That(inform.TryApply(loaded), Is.EqualTo(!reported));
            Assert.That(branch.IsMet(loaded), Is.True);
            Assert.That(inform.TryApply(loaded), Is.False);
        }

        [Test]
        public void InitialStateRoundTripsAndRemainsEligible()
        {
            var loaded = StatePersistence.Deserialize(StatePersistence.Serialize(CreateState()),
                FuneralNightSaveValidation.Validate);
            Assert.That(FuneralNightConsequences.ChooseElena.TryApply(loaded), Is.True);
        }

        [TestCase("")]
        [TestCase("null")]
        [TestCase("{")]
        [TestCase("{}")]
        [TestCase("{\"Version\":1}")]
        public void MalformedOrIncompleteJsonIsRejected(string json)
        {
            Assert.Catch(() => StatePersistence.Deserialize(json));
        }

        [Test]
        public void UnsupportedVersionAndInvalidGenericValuesAreRejected()
        {
            var save = CreateState().Capture();
            save.Version = 2;
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
            save.Version = 1; save.Day = -1;
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
            save.Day = 0; save.Time = 99;
            Assert.Throws<ArgumentOutOfRangeException>(() => GameState.Restore(save));
            save.Time = 2; save.Phase = " ";
            Assert.Throws<ArgumentException>(() => GameState.Restore(save));
            save.Phase = FuneralNight; save.Facts = null;
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
        }

        [TestCase("fact")]
        [TestCase("relationship")]
        [TestCase("knowledge")]
        [TestCase("opportunity")]
        public void DuplicateIdentifiersAreRejected(string kind)
        {
            var state = CreateState();
            state.SetFact("f", false); state.ModifyRelationship("c", "a", 1);
            state.GrantKnowledge("c", "f");
            var save = state.Capture();
            if (kind == "fact") save.Facts = new[] { save.Facts[0], save.Facts[0] };
            if (kind == "relationship") save.Relationships = new[] { save.Relationships[0], save.Relationships[0] };
            if (kind == "knowledge") save.Knowledge = new[] { save.Knowledge[0], save.Knowledge[0] };
            if (kind == "opportunity") save.Opportunities = new[] { save.Opportunities[0], save.Opportunities[0] };
            Assert.Catch(() => GameState.Restore(save));
        }

        [TestCase("both")]
        [TestCase("knowledge")]
        [TestCase("phase")]
        [TestCase("time")]
        [TestCase("opportunity")]
        [TestCase("relationship")]
        [TestCase("day")]
        public void ContradictoryStoryStatesAreRejected(string kind)
        {
            var state = CreateState();
            FuneralNightConsequences.ChooseElena.TryApply(state);
            if (kind == "both") state.SetFact(BreckenMetCharlie, true);
            if (kind == "knowledge") state.GrantKnowledge(Victoria, BreckenMetCharlie);
            if (kind == "phase") state.SetStoryPhase(FuneralNight);
            if (kind == "time") state.AdvanceTime();
            if (kind == "opportunity") state.SetOpportunityState(CharlieBusiness, OpportunityState.Completed);
            if (kind == "relationship") state.ModifyRelationship(Elena, Resentment, 2);
            if (kind == "day") { state.AdvanceTime(); state.AdvanceTime(); state.AdvanceTime(); state.AdvanceTime(); }
            string json = StatePersistence.Serialize(state);
            Assert.Throws<InvalidDataException>(() =>
                StatePersistence.Deserialize(json, FuneralNightSaveValidation.Validate));
        }

        [Test]
        public void NullEntriesBlankIdentifiersAndInvalidOpportunityAreRejected()
        {
            var save = CreateState().Capture();
            save.Facts = new FactSave[] { null };
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
            save.Facts = new[] { new FactSave { Id = "" } };
            Assert.Throws<ArgumentException>(() => GameState.Restore(save));
            save.Facts = Array.Empty<FactSave>();
            save.Opportunities[0].Value = 99;
            Assert.Throws<InvalidDataException>(() => GameState.Restore(save));
        }

        [TestCase("garbage")]
        [TestCase("{}")]
        public void TrailingDataIsRejected(string suffix)
        {
            string json = StatePersistence.Serialize(CreateState());
            Assert.Catch(() => StatePersistence.Deserialize(json + suffix));
        }
        [Test]
        public void OversizedJsonIsRejected()
        {
            Assert.Throws<InvalidDataException>(() =>
                StatePersistence.Deserialize(new string('x', StatePersistence.MaximumBytes + 1)));
        }

        [Test]
        public void FileRoundTripOverwriteAndFailedSavePreservePreviousSlot()
        {
            string directory = Path.Combine(Path.GetTempPath(), "InheritanceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            string path = Path.Combine(directory, "save.json");
            try
            {
                Assert.Throws<FileNotFoundException>(() => StatePersistence.Load(path));
                var state = CreateState();
                StatePersistence.Save(path, state, FuneralNightSaveValidation.Validate);
                FuneralNightConsequences.ChooseCharlie.TryApply(state);
                StatePersistence.Save(path, state, FuneralNightSaveValidation.Validate);
                Assert.That(StatePersistence.Load(path, FuneralNightSaveValidation.Validate)
                    .GetFact(BreckenMetCharlie), Is.True);
                string previous = File.ReadAllText(path);
                state.SetFact(BreckenComfortedElena, true);
                Assert.Throws<InvalidDataException>(() =>
                    StatePersistence.Save(path, state, FuneralNightSaveValidation.Validate));
                Assert.That(File.ReadAllText(path), Is.EqualTo(previous));
                using (var locked = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    Assert.Catch<IOException>(() => StatePersistence.Save(path, CreateState()));
                Assert.That(File.ReadAllText(path), Is.EqualTo(previous));
                Assert.That(Directory.GetFiles(directory).Length, Is.EqualTo(1));
                File.WriteAllText(path, "{");
                Assert.Catch(() => StatePersistence.Load(path));
            }
            finally { Directory.Delete(directory, true); }
        }
    }
}
