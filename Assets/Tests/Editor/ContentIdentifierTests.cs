using System;
using System.IO;
using System.Linq;
using Inheritance.State;
using NUnit.Framework;

namespace Inheritance.Tests
{
    public sealed class ContentIdentifierTests
    {
        [TestCase("elena")]
        [TestCase("chapter1.funeral.comfort_elena")]
        [TestCase("proof.message.elena_thanks")]
        public void CanonicalSpelling(string id) => Assert.That(ContentCatalog.IsCanonical(id), Is.True);
        [TestCase(null)] [TestCase("")] [TestCase("Elena")] [TestCase("a..b")]
        [TestCase("a__b")] [TestCase("a.")] [TestCase(" a")] [TestCase("a b")]
        [TestCase("a/b")] [TestCase("1a")] [TestCase("Ã©lena")] [TestCase("a\n")]
        public void MalformedSpelling(string id) => Assert.That(ContentCatalog.IsCanonical(id), Is.False);
        private static ContentEntry Fact(string id, string provisional = null, params ContentReference[] refs) =>
            new ContentEntry(ContentKind.Fact, id, provisionalReason: provisional, references: refs);
        [Test]
        public void ExistingCatalogsValidateStrictly()
        {
            AuthoredContentCatalogs.FuneralNight().RequireValid();
            AuthoredContentCatalogs.SchedulingProof().RequireValid();
        }
        [Test]
        public void LegacySpellingRequiresExplicitDeclaration()
        {
            Assert.Throws<InvalidDataException>(() => new ContentCatalog(new[] { Fact("MixedCase") }).RequireValid());
            new ContentCatalog(new[] { new ContentEntry(ContentKind.Character, "Elena", true) }).RequireValid();
        }
        [Test]
        public void DuplicateIdsCannotBeProvisional()
        {
            var catalog = new ContentCatalog(new[] { Fact("duplicate", "experiment"), Fact("duplicate") });
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid(ContentValidationMode.Development));
        }
        [TestCase(ContentKind.Character)] [TestCase(ContentKind.Opportunity)]
        [TestCase(ContentKind.Message)] [TestCase(ContentKind.Fact)]
        public void MissingTargetIsActionable(ContentKind kind)
        {
            var catalog = new ContentCatalog(new[] { Fact("source", null, new ContentReference(kind, "missing")) });
            Assert.That(catalog.Validate(ContentValidationMode.Strict).Single().Message, Does.Contain("source").And.Contain("missing"));
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid());
        }
        [Test]
        public void ProvisionalMissingReferenceOnlyWarnsInDevelopment()
        {
            var catalog = new ContentCatalog(new[] { Fact("experiment", "story sketch", new ContentReference(ContentKind.Character, "future_character")) });
            Assert.That(catalog.Validate(ContentValidationMode.Development).All(d => !d.IsError), Is.True);
            catalog.RequireValid(ContentValidationMode.Development);
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid(ContentValidationMode.Strict));
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid(ContentValidationMode.Runtime));
        }
        [Test]
        public void MalformedProvisionalStillFailsDevelopment()
        {
            Assert.Throws<InvalidDataException>(() => new ContentCatalog(new[] { Fact("bad id", "sketch") }).RequireValid(ContentValidationMode.Development));
        }
        [Test]
        public void ActualDefinitionReferencesAreValidated()
        {
            var opportunity = new OpportunityDefinition("visit", ActionCost.Free, Condition.Phase("missing_phase"), Effect.GrantKnowledge("missing_character", "missing_fact"));
            var catalog = new ContentCatalog(new[] { ContentEntry.Opportunity(opportunity) });
            Assert.That(catalog.Validate(ContentValidationMode.Strict).Count(d => d.IsError), Is.EqualTo(3));
        }
        [Test]
        public void ContradictoryNestedPredicatesAndInvalidEnumFail()
        {
            var condition = Condition.All(Condition.Fact("event"), Condition.All(Condition.Fact("event", false)), Condition.Time((TimeBlock)99));
            var catalog = new ContentCatalog(new[] { Fact("event"), ContentEntry.Binding("binding", condition) });
            Assert.That(catalog.Validate(ContentValidationMode.Strict).Count(d => d.IsError), Is.EqualTo(2));
        }
        [Test]
        public void ContradictoryCompetitorsRejectedByDefinition()
        {
            Assert.Throws<ArgumentException>(() => new OpportunityDefinition("visit", ActionCost.Free, Condition.All(), null,
                ActionCost.Free, new[] { new OpportunityCompetition("visit", OpportunityState.Missed) }));
        }
        [Test]
        public void RenameMigratesCharacterAndOpportunityWithoutMutatingSource()
        {
            var original = FuneralNightContent.CreateState().Capture();
            original.Relationships = new[] { new RelationshipSave { Character = "Elena_FuneralPrototype", Attribute = FuneralNightContent.Resentment, Value = 0 } };
            original.Opportunities[0].Id = "Charlie_FuneralBusiness";
            var migrated = AuthoredContentCatalogs.FuneralNight().Restore(original);
            FuneralNightSaveValidation.Validate(migrated);
            Assert.That(migrated.Capture().Relationships[0].Character, Is.EqualTo(FuneralNightContent.Elena));
            Assert.That(migrated.GetOpportunityState(FuneralNightContent.CharlieBusiness), Is.EqualTo(OpportunityState.Available));
            Assert.That(original.Opportunities[0].Id, Is.EqualTo("Charlie_FuneralBusiness"));
        }
        [Test]
        public void AliasCollisionsRejectInsteadOfMerging()
        {
            var save = FuneralNightContent.CreateState().Capture();
            save.Opportunities = new[] { save.Opportunities[0], new OpportunitySave { Id = "Charlie_FuneralBusiness", Value = 1 } };
            Assert.Throws<InvalidDataException>(() => AuthoredContentCatalogs.FuneralNight().Restore(save));
        }
        [TestCase(ContentKind.Character)] [TestCase(ContentKind.Fact)] [TestCase(ContentKind.Opportunity)]
        [TestCase(ContentKind.Message)] [TestCase(ContentKind.Attribute)] [TestCase(ContentKind.Phase)]
        public void UnknownSavedIdsFailBeforeReplacement(ContentKind kind)
        {
            var live = FuneralNightContent.CreateState();
            var before = StatePersistence.Serialize(live);
            var bad = GameState.Restore(live.Capture());
            switch (kind)
            {
                case ContentKind.Character: bad.GrantKnowledge("typo", FuneralNightContent.BreckenComfortedElena); break;
                case ContentKind.Fact: bad.SetFact("typo", true); break;
                case ContentKind.Opportunity: bad.SetOpportunityState("typo", OpportunityState.Completed); break;
                case ContentKind.Message: new MessageDefinition("typo", FuneralNightContent.Elena, "text", Condition.All()).TryQueue(bad); break;
                case ContentKind.Attribute: bad.ModifyRelationship(FuneralNightContent.Elena, "typo", 1); break;
                case ContentKind.Phase: bad.SetStoryPhase("typo"); break;
            }
            Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(StatePersistence.Serialize(bad), catalog: AuthoredContentCatalogs.FuneralNight()));
            Assert.That(StatePersistence.Serialize(live), Is.EqualTo(before));
        }
        [Test]
        public void RetiredHistoricalFactAndKnowledgeSurviveWithoutBecomingLiveContent()
        {
            var state = FuneralNightContent.CreateState();
            state.SetFact("archive.funeral.old_greeting", false);
            state.GrantKnowledge(FuneralNightContent.Elena, "archive.funeral.old_greeting");
            var catalog = AuthoredContentCatalogs.FuneralNight();
            var loaded = StatePersistence.Deserialize(StatePersistence.Serialize(state, catalog: catalog), catalog: catalog);
            Assert.That(loaded.Capture().Facts.Single().Value, Is.False);
            Assert.That(loaded.Knows(FuneralNightContent.Elena, "archive.funeral.old_greeting"), Is.True);
            Assert.Throws<InvalidDataException>(() => new ContentCatalog(new[] { Fact("active", null, new ContentReference(ContentKind.Fact, "archive.funeral.old_greeting")) }).RequireValid());
        }
        [Test]
        public void RemovedOpportunityRequiresReplacementOrRejects()
        {
            var save = FuneralNightContent.CreateState().Capture();
            save.Opportunities[0].Id = "removed_without_replacement";
            Assert.Throws<InvalidDataException>(() => AuthoredContentCatalogs.FuneralNight().Restore(save));
            save.Opportunities[0].Id = "Charlie_FuneralBusiness";
            Assert.DoesNotThrow(() => AuthoredContentCatalogs.FuneralNight().Restore(save));
        }
        [Test]
        public void CyclicOrDanglingAliasRulesFail()
        {
            var catalog = new ContentCatalog(new ContentEntry[0], new[] {
                new SavedIdRule(ContentKind.Fact, "old", "other", "rename"), new SavedIdRule(ContentKind.Fact, "other", "old", "rename") });
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid());
        }
        [Test]
        public void MessageMetadataMismatchFailsAtLoad()
        {
            var state = SchedulingProofContent.CreateState();
            var save = state.Capture();
            Assert.That(save.Messages.Length, Is.EqualTo(1));
            save.Messages[0].RelatedOpportunity = null;
            state = GameState.Restore(save);
            Assert.That(state.GetMessage(SchedulingProofContent.MessageId).RelatedOpportunityId, Is.Null);
            Assert.Throws<InvalidDataException>(() => StatePersistence.Deserialize(StatePersistence.Serialize(state), catalog: AuthoredContentCatalogs.SchedulingProof()));
        }
        [Test]
        public void MalformedReferenceCannotBeWaivedAsProvisional()
        {
            var catalog = new ContentCatalog(new[] { Fact("experiment", "sketch", new ContentReference(ContentKind.Character, "bad id")) });
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid(ContentValidationMode.Development));
        }
        [Test]
        public void InvalidAuthoredEffectEnumIsRejected()
        {
            var definition = new Consequence(Condition.All(), Effect.SetOpportunity("offer", (OpportunityState)99));
            var catalog = new ContentCatalog(new[] { new ContentEntry(ContentKind.Opportunity, "offer"), ContentEntry.Binding("action", definition) });
            Assert.Throws<InvalidDataException>(() => catalog.RequireValid());
        }
        [Test]
        public void FileLoadMigratesAndFailedSavePreservesSlot()
        {
            string path = Path.Combine(Path.GetTempPath(), "inheritance-ids-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                var old = FuneralNightContent.CreateState().Capture();
                old.Opportunities[0].Id = "Charlie_FuneralBusiness";
                StatePersistence.Save(path, GameState.Restore(old));
                string before = File.ReadAllText(path);
                var catalog = AuthoredContentCatalogs.FuneralNight();
                var loaded = StatePersistence.Load(path, FuneralNightSaveValidation.Validate, catalog);
                Assert.That(loaded.GetOpportunityState(FuneralNightContent.CharlieBusiness), Is.EqualTo(OpportunityState.Available));
                Assert.That(loaded.Capture().Version, Is.EqualTo(3));
                loaded.SetFact("unknown_fact", true);
                Assert.Throws<InvalidDataException>(() => StatePersistence.Save(path, loaded, catalog: catalog));
                Assert.That(File.ReadAllText(path), Is.EqualTo(before));
            }
            finally { if (File.Exists(path)) File.Delete(path); }
        }
        [Test]
        public void ArchivedRuleCannotArchiveCharacterOrHideLiveId()
        {
            Assert.Throws<InvalidDataException>(() => new ContentCatalog(new ContentEntry[0], new[] {
                new SavedIdRule(ContentKind.Character, "old", null, "removed", true) }).RequireValid());
            Assert.Throws<InvalidDataException>(() => new ContentCatalog(new[] { Fact("event") }, new[] {
                new SavedIdRule(ContentKind.Fact, "event", null, "removed", true) }).RequireValid());
        }
        [TestCase(true)] [TestCase(false)]
        public void FuneralChoicesAndVictoriaSurviveValidatedV3(bool family)
        {
            var state = FuneralNightContent.CreateState();
            Assert.That((family ? FuneralNightConsequences.ChooseElena : FuneralNightConsequences.ChooseCharlie).TryApply(state), Is.True);
            (family ? FuneralNightConsequences.InformVictoria : FuneralNightConsequences.InformVictoriaAboutCharlie).TryApply(state);
            var catalog = AuthoredContentCatalogs.FuneralNight();
            var loaded = StatePersistence.Deserialize(StatePersistence.Serialize(state, FuneralNightSaveValidation.Validate, catalog), FuneralNightSaveValidation.Validate, catalog);
            Assert.That((family ? FuneralNightConsequences.VictoriaBranch : FuneralNightConsequences.VictoriaBusinessBranch).IsMet(loaded), Is.True);
        }
        [Test]
        public void SchedulingDelayAndReactionSurviveValidatedV3()
        {
            var state = SchedulingProofContent.CreateState();
            Assert.That(SchedulingProofContent.Family.TryComplete(state), Is.True);
            var catalog = AuthoredContentCatalogs.SchedulingProof();
            var loaded = StatePersistence.Deserialize(StatePersistence.Serialize(state, catalog: catalog), catalog: catalog);
            SchedulingProofContent.Evaluate(loaded);
            Assert.That(SchedulingProofContent.Thanks.TryRead(loaded), Is.True);
            Assert.That(SchedulingProofContent.React.TryApply(loaded), Is.True);
            Assert.That(SchedulingProofContent.React.TryApply(loaded), Is.False);
        }
    }
}
