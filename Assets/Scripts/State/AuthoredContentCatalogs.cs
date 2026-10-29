using System.Collections.Generic;

namespace Inheritance.State
{
    // Explicit catalogs stay with authored content. No reflection over text or scene names.
    public static class AuthoredContentCatalogs
    {
        private static ContentEntry Entry(ContentKind kind, string id, params ContentReference[] references) =>
            new ContentEntry(kind, id, false, null, references);
        private static ContentEntry LegacyEntry(ContentKind kind, string id) => new ContentEntry(kind, id, legacySpelling: true);
        public static ContentCatalog FuneralNight()
        {
            var entries = new List<ContentEntry> {
                LegacyEntry(ContentKind.Character, FuneralNightContent.Elena), LegacyEntry(ContentKind.Character, FuneralNightContent.Charlie),
                LegacyEntry(ContentKind.Character, FuneralNightContent.Victoria), LegacyEntry(ContentKind.Attribute, FuneralNightContent.Resentment),
                LegacyEntry(ContentKind.Fact, FuneralNightContent.BreckenComfortedElena), LegacyEntry(ContentKind.Fact, FuneralNightContent.BreckenMetCharlie),
                LegacyEntry(ContentKind.Opportunity, FuneralNightContent.CharlieBusiness),
                LegacyEntry(ContentKind.Phase, FuneralNightContent.FuneralNight), LegacyEntry(ContentKind.Phase, FuneralNightContent.AfterFuneralNight)
            };
            entries.Add(ContentEntry.Binding("funeral.choice_elena", FuneralNightConsequences.ChooseElena));
            entries.Add(ContentEntry.Binding("funeral.choice_charlie", FuneralNightConsequences.ChooseCharlie));
            entries.Add(ContentEntry.Binding("funeral.inform_family", FuneralNightConsequences.InformVictoria));
            entries.Add(ContentEntry.Binding("funeral.inform_business", FuneralNightConsequences.InformVictoriaAboutCharlie));
            entries.Add(ContentEntry.Binding("funeral.react_family", FuneralNightConsequences.VictoriaBranch));
            entries.Add(ContentEntry.Binding("funeral.react_business", FuneralNightConsequences.VictoriaBusinessBranch));
            return new ContentCatalog(entries, new[] {
                new SavedIdRule(ContentKind.Character, "Elena_FuneralPrototype", FuneralNightContent.Elena, "Pre-release character spelling example; retain old saves."),
                new SavedIdRule(ContentKind.Opportunity, "Charlie_FuneralBusiness", FuneralNightContent.CharlieBusiness, "Pre-release opportunity spelling example; retain old saves."),
                new SavedIdRule(ContentKind.Fact, "archive.funeral.old_greeting", null, "Retired prototype greeting is historical only; never drives current content.", true)
            });
        }
        public static ContentCatalog SchedulingProof()
        {
            var entries = new[] {
                LegacyEntry(ContentKind.Character, FuneralNightContent.Elena), LegacyEntry(ContentKind.Phase, SchedulingProofContent.Phase),
                Entry(ContentKind.Fact, SchedulingProofContent.FamilyFact), Entry(ContentKind.Fact, SchedulingProofContent.ReactionFact),
                ContentEntry.Opportunity(SchedulingProofContent.Family),
                ContentEntry.Opportunity(SchedulingProofContent.Business),
                ContentEntry.Message(SchedulingProofContent.Thanks),
                ContentEntry.Binding("proof.reaction", SchedulingProofContent.React)
            };
            return new ContentCatalog(entries, messages: new[] { SchedulingProofContent.Thanks });
        }
    }
}
