using System.IO;
using static Inheritance.State.FuneralNightContent;

namespace Inheritance.State
{
    // Authored invariants stay outside the generic state/serialization layer.
    public static class FuneralNightSaveValidation
    {
        public static void Validate(GameState state)
        {
            bool family = state.GetFact(BreckenComfortedElena);
            bool business = state.GetFact(BreckenMetCharlie);
            bool knowsFamily = state.Knows(Victoria, BreckenComfortedElena);
            bool knowsBusiness = state.Knows(Victoria, BreckenMetCharlie);
            bool pending = state.StoryPhase == FuneralNight;
            bool decided = state.StoryPhase == AfterFuneralNight;
            var opportunity = state.GetOpportunityState(CharlieBusiness);
            if (state.Day != 0 || (!pending && !decided) || (family && business) ||
                (knowsFamily && !family) || (knowsBusiness && !business) ||
                (pending && (family || business || knowsFamily || knowsBusiness ||
                    state.CurrentTimeBlock != TimeBlock.Evening || opportunity != OpportunityState.Available ||
                    state.GetRelationship(Elena, Resentment) != 0 ||
                    state.GetRelationship(Charlie, Resentment) != 0)) ||
                (decided && ((!family && !business) || state.CurrentTimeBlock != TimeBlock.Night ||
                    opportunity != (family ? OpportunityState.Missed : OpportunityState.Completed) ||
                    state.GetRelationship(Elena, Resentment) != (family ? -1 : 1) ||
                    state.GetRelationship(Charlie, Resentment) != (family ? 0 : -1))))
                throw new InvalidDataException("Save contradicts the funeral-night story state.");
        }
    }
}
