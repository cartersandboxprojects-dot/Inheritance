using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Inheritance.State
{
    public enum ContentKind { Character, Fact, Opportunity, Message, Attribute, Phase, Binding }
    public enum ContentValidationMode { Development, Strict, Runtime }

    // Category plus readable spelling. No implicit conversion or automatic case correction.
    public sealed class ContentReference
    {
        public ContentKind Kind { get; }
        public string Id { get; }
        public ContentReference(ContentKind kind, string id) { Kind = kind; Id = id; }
        public override string ToString() => Kind + ":" + Id;
    }

    public sealed class ContentEntry
    {
        public ContentReference Key { get; }
        public bool LegacySpelling { get; }
        public string ProvisionalReason { get; }
        internal ContentReference[] References { get; }
        internal string[] Errors { get; private set; } = new string[0];
        public static ContentEntry Opportunity(OpportunityDefinition definition, string provisionalReason = null) =>
            new ContentEntry(ContentKind.Opportunity, definition.Id, false, provisionalReason, definition.References) { Errors = definition.Errors };
        public static ContentEntry Message(MessageDefinition definition, string provisionalReason = null) =>
            new ContentEntry(ContentKind.Message, definition.Id, false, provisionalReason, definition.References) { Errors = definition.Errors };
        public static ContentEntry Binding(string id, Consequence definition, string provisionalReason = null) =>
            new ContentEntry(ContentKind.Binding, id, false, provisionalReason, definition.References) { Errors = definition.Errors };
        public static ContentEntry Binding(string id, Condition definition, string provisionalReason = null) =>
            new ContentEntry(ContentKind.Binding, id, false, provisionalReason, definition.References) { Errors = definition.Errors };
        public ContentEntry(ContentKind kind, string id, bool legacySpelling = false,
            string provisionalReason = null, params ContentReference[] references)
        {
            Key = new ContentReference(kind, id); LegacySpelling = legacySpelling;
            ProvisionalReason = provisionalReason; References = references?.ToArray() ?? new ContentReference[0];
        }
    }

    public sealed class ContentDiagnostic
    {
        public bool IsError { get; }
        public string Message { get; }
        internal ContentDiagnostic(bool error, string message) { IsError = error; Message = message; }
        public override string ToString() => (IsError ? "Error: " : "Warning: ") + Message;
    }

    // One-hop replacement, or an explicitly archived fact. No silent deletion/defaults.
    public sealed class SavedIdRule
    {
        public ContentKind Kind { get; }
        public string OldId { get; }
        public string Replacement { get; }
        public string Reason { get; }
        public bool ArchiveFact { get; }
        public SavedIdRule(ContentKind kind, string oldId, string replacement, string reason, bool archiveFact = false)
        { Kind = kind; OldId = oldId; Replacement = replacement; Reason = reason; ArchiveFact = archiveFact; }
    }

    public sealed class ContentCatalog
    {
        private readonly ContentEntry[] entries;
        private readonly SavedIdRule[] rules;
        private readonly MessageDefinition[] messages;
        public ContentCatalog(IEnumerable<ContentEntry> entries, IEnumerable<SavedIdRule> rules = null,
            IEnumerable<MessageDefinition> messages = null)
        {
            this.entries = entries.ToArray(); this.rules = rules?.ToArray() ?? new SavedIdRule[0];
            this.messages = messages?.ToArray() ?? new MessageDefinition[0];
        }
        public static bool IsCanonical(string id) => id != null && id.Length <= 128 &&
            Regex.IsMatch(id, @"\A[a-z][a-z0-9]*(?:[._][a-z0-9]+)*\z");
        private static bool Legacy(string id) => id != null && id.Length <= 128 &&
            Regex.IsMatch(id, @"\A[A-Za-z][A-Za-z0-9]*(?:[._][A-Za-z0-9]+)*\z");
        private bool Has(ContentKind kind, string id) => entries.Any(e => e.Key.Kind == kind && e.Key.Id == id);

        public IReadOnlyList<ContentDiagnostic> Validate(ContentValidationMode mode)
        {
            var result = entries.SelectMany(e => e.Errors.Select(error => e.Key + ": " + error)).Select(e => new ContentDiagnostic(true, e)).ToList();
            foreach (var entry in entries)
            {
                string owner = entry.Key.ToString();
                if (!Enum.IsDefined(typeof(ContentKind), entry.Key.Kind) || !(IsCanonical(entry.Key.Id) || (entry.LegacySpelling && Legacy(entry.Key.Id))))
                    result.Add(new ContentDiagnostic(true, owner + " has malformed spelling; use lowercase dot/underscore segments."));
                if (entries.Count(e => e.Key.Kind == entry.Key.Kind && e.Key.Id == entry.Key.Id) != 1)
                    result.Add(new ContentDiagnostic(true, owner + " is duplicated; keep exactly one declaration."));
                bool provisional = !string.IsNullOrWhiteSpace(entry.ProvisionalReason);
                if (provisional) result.Add(new ContentDiagnostic(false, owner + " is provisional: " + entry.ProvisionalReason));
                foreach (var reference in entry.References.Concat(messages.Where(m => entry.Key.Kind == ContentKind.Message && m.Id == entry.Key.Id).SelectMany(m => m.References)))
                {
                    bool malformed = reference == null || !Enum.IsDefined(typeof(ContentKind), reference.Kind) ||
                        !(IsCanonical(reference.Id) || entries.Any(e => e.LegacySpelling && e.Key.Kind == reference.Kind && e.Key.Id == reference.Id));
                    if (malformed) result.Add(new ContentDiagnostic(true, owner + " has malformed reference " + reference + "; use a canonical ID or declared legacy target."));
                    else if (!Has(reference.Kind, reference.Id))
                        result.Add(new ContentDiagnostic(!(mode == ContentValidationMode.Development && provisional),
                            owner + " references missing " + reference + "; declare/fix the target" +
                            (provisional ? " (provisional: " + entry.ProvisionalReason + ")." : ".")));
                }
            }
            foreach (var message in messages)
            {
                result.AddRange(message.Errors.Select(e => new ContentDiagnostic(true, "Message " + message.Id + ": " + e)));
                if (!Has(ContentKind.Message, message.Id) || messages.Count(m => m.Id == message.Id) != 1)
                    result.Add(new ContentDiagnostic(true, "Message definition " + message.Id + " needs exactly one matching catalog entry and definition."));
            }
            foreach (var entry in entries.Where(e => e.Key.Kind == ContentKind.Message))
                if (messages.Count(m => m.Id == entry.Key.Id) != 1)
                    result.Add(new ContentDiagnostic(true, entry.Key + " has no unique runtime message definition."));
            foreach (var rule in rules)
                if (!Enum.IsDefined(typeof(ContentKind), rule.Kind) || !Legacy(rule.OldId) || string.IsNullOrWhiteSpace(rule.Reason) ||
                    Has(rule.Kind, rule.OldId) || rules.Count(r => r.Kind == rule.Kind && r.OldId == rule.OldId) != 1 ||
                    (rule.ArchiveFact ? rule.Kind != ContentKind.Fact || rule.Replacement != null :
                        rule.Replacement == null || !Has(rule.Kind, rule.Replacement)))
                    result.Add(new ContentDiagnostic(true, "Invalid saved-ID rule " + rule.Kind + ":" + rule.OldId + "; require a unique retired source, reason and direct live target, or archived fact."));
            return result;
        }
        public void RequireValid(ContentValidationMode mode = ContentValidationMode.Strict)
        {
            var errors = Validate(mode).Where(d => d.IsError).ToArray();
            if (errors.Length != 0) throw new InvalidDataException(string.Join("\n", errors.Select(d => d.ToString())));
        }
        private string Resolve(ContentKind kind, string id, bool historicalFact = false)
        {
            if (Has(kind, id)) return id;
            var rule = rules.SingleOrDefault(r => r.Kind == kind && r.OldId == id);
            if (rule != null && (!rule.ArchiveFact || historicalFact)) return rule.Replacement ?? id;
            throw new InvalidDataException("Unknown or removed " + kind + " ID '" + id + "'; add a reviewed saved-ID rule or repair the save explicitly.");
        }
        // Input and live state are untouched. Restore catches alias collisions rather than merging values.
        public GameState Restore(StateSave input)
        {
            RequireValid(ContentValidationMode.Runtime);
            var save = GameState.Restore(input).Capture();
            save.Phase = Resolve(ContentKind.Phase, save.Phase);
            foreach (var e in save.Facts) e.Id = Resolve(ContentKind.Fact, e.Id, true);
            foreach (var e in save.Relationships) { e.Character = Resolve(ContentKind.Character, e.Character); e.Attribute = Resolve(ContentKind.Attribute, e.Attribute); }
            foreach (var e in save.Knowledge) { e.Character = Resolve(ContentKind.Character, e.Character); e.Fact = Resolve(ContentKind.Fact, e.Fact, true); }
            foreach (var e in save.Opportunities) e.Id = Resolve(ContentKind.Opportunity, e.Id);
            foreach (var e in save.Messages)
            {
                e.Id = Resolve(ContentKind.Message, e.Id); e.Sender = Resolve(ContentKind.Character, e.Sender);
                if (e.RelatedOpportunity != null) e.RelatedOpportunity = Resolve(ContentKind.Opportunity, e.RelatedOpportunity);
                var definition = messages.SingleOrDefault(m => m.Id == e.Id);
                if (definition == null || definition.SenderId != e.Sender || definition.RelatedOpportunityId != e.RelatedOpportunity)
                    throw new InvalidDataException("Saved message metadata contradicts definition: " + e.Id);
            }
            try { return GameState.Restore(save); }
            catch (ArgumentException error) { throw new InvalidDataException("Saved IDs collide after migration.", error); }
        }
        public void ValidateSaved(GameState state)
        {
            var migrated = Restore(state.Capture());
            // Saving must not emit aliases; only load may migrate them.
            if (StatePersistence.Serialize(migrated) != StatePersistence.Serialize(state))
                throw new InvalidDataException("State contains retired IDs; migrate it through catalog Restore before saving.");
        }
    }
}
