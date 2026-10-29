using System;
using System.Collections.Generic;

namespace Inheritance.State
{
    public enum MessageStatus { Absent = 0, Queued = 1, Delivered = 2, Read = 3 }

    // Immutable value: callers can inspect it without changing backend state.
    public sealed class PhoneMessage
    {
        public string Id { get; }
        public string SenderId { get; }
        public string RelatedOpportunityId { get; }
        public MessageStatus Status { get; }
        internal PhoneMessage(string id, string senderId, string relatedOpportunityId, MessageStatus status)
        { Id = id; SenderId = senderId; RelatedOpportunityId = relatedOpportunityId; Status = status; }
    }

    public sealed partial class GameState
    {
        private readonly Dictionary<string, PhoneMessage> messages = new Dictionary<string, PhoneMessage>();
        public PhoneMessage GetMessage(string id) => messages.TryGetValue(Id(id), out var value) ? value : null;
        public MessageStatus GetMessageStatus(string id) => GetMessage(id)?.Status ?? MessageStatus.Absent;
        internal bool QueueMessage(string id, string sender, string related)
        {
            Id(id); Id(sender); if (related != null) Id(related);
            if (messages.ContainsKey(id)) return false;
            messages.Add(id, new PhoneMessage(id, sender, related, MessageStatus.Queued)); return true;
        }
        internal bool TransitionMessage(string id, MessageStatus from, MessageStatus to)
        {
            var message = GetMessage(id);
            if (message == null || message.Status != from) return false;
            messages[id] = new PhoneMessage(id, message.SenderId, message.RelatedOpportunityId, to); return true;
        }
    }

    public sealed class MessageDefinition
    {
        public string Id { get; }
        public string SenderId { get; }
        public string Text { get; }
        public string RelatedOpportunityId { get; }
        private readonly Condition delivery;
        private readonly Consequence onRead;
        public MessageDefinition(string id, string senderId, string text, Condition delivery,
            string relatedOpportunityId = null, Consequence onRead = null)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(senderId) ||
                (relatedOpportunityId != null && string.IsNullOrWhiteSpace(relatedOpportunityId)))
                throw new ArgumentException("Missing message identifier.");
            Id = id; SenderId = senderId; Text = text ?? throw new ArgumentNullException(nameof(text));
            this.delivery = delivery ?? throw new ArgumentNullException(nameof(delivery));
            RelatedOpportunityId = relatedOpportunityId; this.onRead = onRead;
        }
        private bool Matches(PhoneMessage message) => message != null && message.SenderId == SenderId && message.RelatedOpportunityId == RelatedOpportunityId;
        public bool TryQueue(GameState state) => state.QueueMessage(Id, SenderId, RelatedOpportunityId);
        public bool TryDeliver(GameState state)
        {
            if (!Matches(state.GetMessage(Id)) || state.GetMessageStatus(Id) != MessageStatus.Queued || !delivery.IsMet(state)) return false;
            return state.TransitionMessage(Id, MessageStatus.Queued, MessageStatus.Delivered);
        }
        // Rendering only reads GetMessage/Text. This explicit player action may invoke a guarded consequence.
        public bool TryRead(GameState state)
        {
            if (!Matches(state.GetMessage(Id)) || state.GetMessageStatus(Id) != MessageStatus.Delivered) return false;
            var candidate = GameState.Restore(state.Capture());
            if (onRead != null && !onRead.TryApply(candidate)) return false;
            candidate.TransitionMessage(Id, MessageStatus.Delivered, MessageStatus.Read);
            state.CommitFrom(candidate); return true;
        }
    }
}
