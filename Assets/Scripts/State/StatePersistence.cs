using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace Inheritance.State
{
    // Required members prevent truncated/partial JSON from silently becoming default state.
    [DataContract]
    public sealed class StateSave
    {
        [DataMember(IsRequired = true)] public int Version;
        [DataMember] public int? TimeUnitsRemaining;
        [DataMember] public MessageSave[] Messages;
        [DataMember(IsRequired = true)] public int Day;
        [DataMember(IsRequired = true)] public int Time;
        [DataMember(IsRequired = true)] public string Phase;
        [DataMember(IsRequired = true)] public FactSave[] Facts;
        [DataMember(IsRequired = true)] public RelationshipSave[] Relationships;
        [DataMember(IsRequired = true)] public KnowledgeSave[] Knowledge;
        [DataMember(IsRequired = true)] public OpportunitySave[] Opportunities;
    }

    [DataContract]
    public sealed class FactSave
    {
        [DataMember(IsRequired = true)] public string Id;
        [DataMember(IsRequired = true)] public bool Value;
    }

    [DataContract]
    public sealed class RelationshipSave
    {
        [DataMember(IsRequired = true)] public string Character;
        [DataMember(IsRequired = true)] public string Attribute;
        [DataMember(IsRequired = true)] public int Value;
    }

    [DataContract]
    public sealed class KnowledgeSave
    {
        [DataMember(IsRequired = true)] public string Character;
        [DataMember(IsRequired = true)] public string Fact;
    }

    [DataContract]
    public sealed class OpportunitySave
    {
        [DataMember(IsRequired = true)] public string Id;
        [DataMember(IsRequired = true)] public int Value;
    }

    [DataContract]
    public sealed class MessageSave
    {
        [DataMember(IsRequired = true)] public string Id;
        [DataMember(IsRequired = true)] public string Sender;
        [DataMember] public string RelatedOpportunity;
        [DataMember(IsRequired = true)] public int Status;
    }

    public sealed partial class GameState
    {
        // Detached, ordered snapshot; callers cannot mutate the live dictionaries.
        public StateSave Capture() => new StateSave
        {
            Version = StatePersistence.CurrentVersion,
            Day = Day, Time = (int)CurrentTimeBlock, Phase = StoryPhase,
            TimeUnitsRemaining = TimeUnitsRemaining,
            Messages = messages.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => new MessageSave {
                Id = x.Key, Sender = x.Value.SenderId, RelatedOpportunity = x.Value.RelatedOpportunityId,
                Status = (int)x.Value.Status }).ToArray(),
            Facts = facts.OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => new FactSave { Id = x.Key, Value = x.Value }).ToArray(),
            Relationships = relationships.OrderBy(x => x.Key.character, StringComparer.Ordinal)
                .ThenBy(x => x.Key.attribute, StringComparer.Ordinal)
                .Select(x => new RelationshipSave { Character = x.Key.character,
                    Attribute = x.Key.attribute, Value = x.Value }).ToArray(),
            Knowledge = knowledge.OrderBy(x => x.character, StringComparer.Ordinal)
                .ThenBy(x => x.fact, StringComparer.Ordinal)
                .Select(x => new KnowledgeSave { Character = x.character, Fact = x.fact }).ToArray(),
            Opportunities = opportunities.OrderBy(x => x.Key, StringComparer.Ordinal)
                .Select(x => new OpportunitySave { Id = x.Key, Value = (int)x.Value }).ToArray()
        };

        public static GameState Restore(StateSave save)
        {
            if (save == null || (save.Version < 1 || save.Version > StatePersistence.CurrentVersion))
                throw new InvalidDataException("Unsupported or missing save version.");
            if (save.Day < 0 || save.Facts == null || save.Relationships == null ||
                save.Knowledge == null || save.Opportunities == null || (save.Version >= 3 && save.Messages == null))
                throw new InvalidDataException("Incomplete or invalid state.");
            int capacity = save.Version == 1 ? TimeUnitsPerBlock : save.TimeUnitsRemaining ?? -1;
            if (capacity < 1 || capacity > TimeUnitsPerBlock)
                throw new InvalidDataException("Invalid or missing time capacity.");
            var state = new GameState(save.Phase, (TimeBlock)save.Time)
                { Day = save.Day, TimeUnitsRemaining = capacity };
            foreach (var entry in save.Facts)
            {
                if (entry == null) throw new InvalidDataException("Null fact.");
                state.facts.Add(Id(entry.Id), entry.Value);
            }
            foreach (var entry in save.Relationships)
            {
                if (entry == null) throw new InvalidDataException("Null relationship.");
                state.relationships.Add((Id(entry.Character), Id(entry.Attribute)), entry.Value);
            }
            foreach (var entry in save.Knowledge)
            {
                if (entry == null || !state.knowledge.Add((Id(entry.Character), Id(entry.Fact))))
                    throw new InvalidDataException("Null or duplicate knowledge.");
            }
            foreach (var entry in save.Opportunities)
            {
                if (entry == null || !Enum.IsDefined(typeof(OpportunityState), entry.Value))
                    throw new InvalidDataException("Invalid opportunity.");
                state.opportunities.Add(Id(entry.Id), (OpportunityState)entry.Value);
            }
            // Legacy schemas had no phone state. Migration introduces an empty inbox.
            if (save.Version >= 3) foreach (var entry in save.Messages)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.Id) || string.IsNullOrWhiteSpace(entry.Sender) ||
                    (entry.RelatedOpportunity != null && string.IsNullOrWhiteSpace(entry.RelatedOpportunity)) ||
                    entry.Status < (int)MessageStatus.Queued || entry.Status > (int)MessageStatus.Read || state.messages.ContainsKey(entry.Id))
                    throw new InvalidDataException("Invalid or duplicate message.");
                state.messages.Add(entry.Id, new PhoneMessage(entry.Id, entry.Sender, entry.RelatedOpportunity, (MessageStatus)entry.Status));
            }
            return state;
        }
    }

    // Pure .NET persistence; no Ink, scene, transform, or Unity ownership.
    public static class StatePersistence
    {
        public const int CurrentVersion = 3;
        public const int MaximumBytes = 1024 * 1024;

        public static string Serialize(GameState state, Action<GameState> validate = null, ContentCatalog catalog = null)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            catalog?.ValidateSaved(state);
            validate?.Invoke(state);
            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(StateSave)).WriteObject(stream, state.Capture());
                if (stream.Length > MaximumBytes) throw new InvalidDataException("Save is too large.");
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static GameState Deserialize(string json, Action<GameState> validate = null, ContentCatalog catalog = null)
        {
            if (string.IsNullOrWhiteSpace(json) || Encoding.UTF8.GetByteCount(json) > MaximumBytes)
                throw new InvalidDataException("Save is empty or too large.");
            RequireSingleObject(json);
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max))
            {
                var save = (StateSave)new DataContractJsonSerializer(typeof(StateSave)).ReadObject(reader);
                if (reader.MoveToContent() != XmlNodeType.None)
                    throw new InvalidDataException("Unexpected data after the save object.");
                var state = catalog == null ? GameState.Restore(save) : catalog.Restore(save);
                validate?.Invoke(state);
                return state;
            }
        }

        // The platform JSON reader accepts concatenated root objects. Check the root boundary
        // explicitly, respecting quoted/escaped braces; the serializer validates JSON syntax.
        private static void RequireSingleObject(string json)
        {
            string text = json.Trim();
            if (text.Length == 0 || text[0] != '{')
                throw new InvalidDataException("Expected a JSON save object.");
            int depth = 0;
            bool quoted = false;
            for (int i = 0; i < text.Length; i++)
            {
                char value = text[i];
                if (quoted)
                {
                    if (value == '\\') i++;
                    else if (value == '"') quoted = false;
                    continue;
                }
                if (value == '"') quoted = true;
                else if (value == '{' || value == '[')
                {
                    if (++depth > 64) throw new InvalidDataException("Save nesting is too deep.");
                }
                else if (value == '}' || value == ']')
                {
                    if (--depth == 0 && i != text.Length - 1)
                        throw new InvalidDataException("Unexpected data after the save object.");
                }
            }
            if (quoted || depth != 0) throw new InvalidDataException("Incomplete JSON save object.");
        }
        public static void Save(string path, GameState state, Action<GameState> validate = null, ContentCatalog catalog = null)
        {
            string json = Serialize(state, validate, catalog);
            string temporary = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                // Same-directory rename/replace keeps the previous slot intact on write failure.
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(json);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }
                if (File.Exists(path)) File.Replace(temporary, path, null);
                else File.Move(temporary, path);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }

        public static GameState Load(string path, Action<GameState> validate = null, ContentCatalog catalog = null)
        {
            if (new FileInfo(path).Length > MaximumBytes) throw new InvalidDataException("Save is too large.");
            return Deserialize(File.ReadAllText(path, Encoding.UTF8), validate, catalog);
        }
    }
}
