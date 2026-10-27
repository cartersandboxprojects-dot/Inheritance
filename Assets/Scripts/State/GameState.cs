using System;
using System.Collections.Generic;

namespace Inheritance.State
{
    public enum OpportunityState { Unavailable, Available, Missed, Deferred, Completed }
    public enum TimeBlock { Morning, Afternoon, Evening, Night }

    // Session state only. Content owns identifiers and decides when consequences apply.
    // Knowledge records awareness of an event, independently of its current truth value.
    public sealed class GameState
    {
        private readonly Dictionary<string, bool> facts = new Dictionary<string, bool>();
        private readonly Dictionary<(string character, string attribute), int> relationships =
            new Dictionary<(string, string), int>();
        private readonly HashSet<(string character, string fact)> knowledge =
            new HashSet<(string, string)>();
        private readonly Dictionary<string, OpportunityState> opportunities =
            new Dictionary<string, OpportunityState>();

        public int Day { get; private set; }
        public TimeBlock CurrentTimeBlock { get; private set; }
        public string StoryPhase { get; private set; }

        public GameState(string storyPhase, TimeBlock timeBlock = TimeBlock.Morning)
        {
            StoryPhase = Id(storyPhase);
            ValidateTimeBlock(timeBlock);
            CurrentTimeBlock = timeBlock;
        }

        public bool GetFact(string fact) => facts.TryGetValue(Id(fact), out var value) && value;
        public void SetFact(string fact, bool value) => facts[Id(fact)] = value;

        // Attributes describe the player's relationship with the identified character.
        public int GetRelationship(string character, string attribute) =>
            relationships.TryGetValue((Id(character), Id(attribute)), out var value) ? value : 0;

        public void ModifyRelationship(string character, string attribute, int delta)
        {
            var key = (Id(character), Id(attribute));
            relationships[key] = checked(GetRelationship(character, attribute) + delta);
        }

        public bool Knows(string character, string fact) => knowledge.Contains((Id(character), Id(fact)));
        public void GrantKnowledge(string character, string fact) => knowledge.Add((Id(character), Id(fact)));

        public OpportunityState GetOpportunityState(string opportunity) =>
            opportunities.TryGetValue(Id(opportunity), out var value) ? value : OpportunityState.Unavailable;

        public void SetOpportunityState(string opportunity, OpportunityState value)
        {
            if (!Enum.IsDefined(typeof(OpportunityState), value))
                throw new ArgumentOutOfRangeException(nameof(value));
            opportunities[Id(opportunity)] = value;
        }

        // One call advances one broad block. Day is a relative counter starting at zero.
        public void AdvanceTime()
        {
            if (CurrentTimeBlock == TimeBlock.Night)
            {
                Day = checked(Day + 1);
                CurrentTimeBlock = TimeBlock.Morning;
            }
            else CurrentTimeBlock = (TimeBlock)((int)CurrentTimeBlock + 1);
        }

        public void SetStoryPhase(string phase) => StoryPhase = Id(phase);

        private static string Id(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("A content identifier must not be blank.", nameof(value));
            return value;
        }

        private static void ValidateTimeBlock(TimeBlock value)
        {
            if (!Enum.IsDefined(typeof(TimeBlock), value))
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}
