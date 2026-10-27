namespace Inheritance.State
{
    // Authored definitions, separate from the generic condition/effect machinery.
    public static class FuneralNightConsequences
    {
        public static readonly Consequence ChooseElena = new Consequence(
            Condition.All(
                Condition.Phase(FuneralNightContent.FuneralNight),
                Condition.Time(TimeBlock.Evening),
                Condition.Opportunity(FuneralNightContent.CharlieBusiness, OpportunityState.Available),
                Condition.Fact(FuneralNightContent.BreckenComfortedElena, false),
                Condition.Fact(FuneralNightContent.BreckenMetCharlie, false)),
            Effect.SetFact(FuneralNightContent.BreckenComfortedElena, true),
            Effect.SetFact(FuneralNightContent.BreckenMetCharlie, false),
            Effect.ModifyRelationship(FuneralNightContent.Elena, FuneralNightContent.Resentment, -1),
            Effect.SetOpportunity(FuneralNightContent.CharlieBusiness, OpportunityState.Missed),
            Effect.AdvanceTime(),
            Effect.SetPhase(FuneralNightContent.AfterFuneralNight));

        public static readonly Consequence InformVictoria = new Consequence(
            Condition.All(
                Condition.Fact(FuneralNightContent.BreckenComfortedElena),
                Condition.Phase(FuneralNightContent.AfterFuneralNight),
                Condition.Knowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenComfortedElena, false)),
            Effect.GrantKnowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenComfortedElena));

        public static readonly Condition VictoriaBranch = Condition.All(
            Condition.Fact(FuneralNightContent.BreckenComfortedElena),
            Condition.Knowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenComfortedElena),
            Condition.Phase(FuneralNightContent.AfterFuneralNight));
    }
}
