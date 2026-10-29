using System;
using System.Collections.Generic;

namespace Inheritance.State
{
    public enum OpportunityState { Unavailable = 0, Available = 1, Missed = 2, Deferred = 3, Completed = 4, Accepted = 5, Expired = 6 }
    public enum TimeBlock { Morning, Afternoon, Evening, Night }

    // Backend-owned state. Content owns identifiers and decides when consequences apply.
    // Knowledge records awareness of an event, independently of its current truth value.
    public sealed partial class GameState
    {
        private readonly Dictionary<string, bool> facts = new Dictionary<string, bool>();
        private readonly Dictionary<(string character, string attribute), int> relationships =
            new Dictionary<(string, string), int>();
        private readonly HashSet<(string character, string fact)> knowledge =
            new HashSet<(string, string)>();
        private readonly Dictionary<string, OpportunityState> opportunities =
            new Dictionary<string, OpportunityState>();

        public const int TimeUnitsPerBlock = 3;
        public int TimeUnitsRemaining { get; private set; } = TimeUnitsPerBlock;
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
            TimeUnitsRemaining = TimeUnitsPerBlock;
        }

        public bool CanAfford(ActionCost cost)
        {
            if (cost == null) throw new ArgumentNullException(nameof(cost));
            return cost.Mode != ActionCostMode.Units || cost.Units <= TimeUnitsRemaining;
        }

        public bool TryApplyCost(ActionCost cost)
        {
            if (!CanAfford(cost)) return false;
            if (cost.Mode != ActionCostMode.Units || cost.Units == TimeUnitsRemaining)
                AdvanceTime(); // Overflow is checked before any mutation.
            else TimeUnitsRemaining -= cost.Units;
            return true;
        }

        // Commit a fully validated, detached result while keeping this state authoritative.
        internal void CommitFrom(GameState result)
        {
            facts.Clear(); foreach (var item in result.facts) facts.Add(item.Key, item.Value);
            relationships.Clear(); foreach (var item in result.relationships) relationships.Add(item.Key, item.Value);
            knowledge.Clear(); foreach (var item in result.knowledge) knowledge.Add(item);
            opportunities.Clear(); foreach (var item in result.opportunities) opportunities.Add(item.Key, item.Value);
            messages.Clear(); foreach (var item in result.messages) messages.Add(item.Key, item.Value);
            Day = result.Day; CurrentTimeBlock = result.CurrentTimeBlock;
            TimeUnitsRemaining = result.TimeUnitsRemaining; StoryPhase = result.StoryPhase;
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
