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
            ActionCost.HardTransition,
            Effect.SetFact(FuneralNightContent.BreckenComfortedElena, true),
            Effect.SetFact(FuneralNightContent.BreckenMetCharlie, false),
            Effect.ModifyRelationship(FuneralNightContent.Elena, FuneralNightContent.Resentment, -1),
            Effect.SetOpportunity(FuneralNightContent.CharlieBusiness, OpportunityState.Missed),
            Effect.SetPhase(FuneralNightContent.AfterFuneralNight));

        // The same evening opportunity is consumed by either mutually exclusive choice.
        public static readonly Consequence ChooseCharlie = new Consequence(
            Condition.All(
                Condition.Phase(FuneralNightContent.FuneralNight),
                Condition.Time(TimeBlock.Evening),
                Condition.Opportunity(FuneralNightContent.CharlieBusiness, OpportunityState.Available),
                Condition.Fact(FuneralNightContent.BreckenComfortedElena, false),
                Condition.Fact(FuneralNightContent.BreckenMetCharlie, false)),
            ActionCost.HardTransition,
            Effect.SetFact(FuneralNightContent.BreckenMetCharlie, true),
            Effect.ModifyRelationship(FuneralNightContent.Elena, FuneralNightContent.Resentment, 1),
            Effect.ModifyRelationship(FuneralNightContent.Charlie, FuneralNightContent.Resentment, -1),
            Effect.SetOpportunity(FuneralNightContent.CharlieBusiness, OpportunityState.Completed),
            Effect.SetPhase(FuneralNightContent.AfterFuneralNight));

        public static readonly Consequence InformVictoriaAboutCharlie = new Consequence(
            Condition.All(
                Condition.Fact(FuneralNightContent.BreckenMetCharlie),
                Condition.Fact(FuneralNightContent.BreckenComfortedElena, false),
                Condition.Phase(FuneralNightContent.AfterFuneralNight),
                Condition.Knowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenMetCharlie, false)),
            Effect.GrantKnowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenMetCharlie));

        public static readonly Condition VictoriaBusinessBranch = Condition.All(
            Condition.Fact(FuneralNightContent.BreckenMetCharlie),
            Condition.Fact(FuneralNightContent.BreckenComfortedElena, false),
            Condition.Knowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenMetCharlie),
            Condition.Phase(FuneralNightContent.AfterFuneralNight));

        public static readonly Consequence InformVictoria = new Consequence(
            Condition.All(
                Condition.Fact(FuneralNightContent.BreckenComfortedElena),
            Condition.Fact(FuneralNightContent.BreckenMetCharlie, false),
                Condition.Phase(FuneralNightContent.AfterFuneralNight),
                Condition.Knowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenComfortedElena, false)),
            Effect.GrantKnowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenComfortedElena));

        public static readonly Condition VictoriaBranch = Condition.All(
            Condition.Fact(FuneralNightContent.BreckenComfortedElena),
            Condition.Fact(FuneralNightContent.BreckenMetCharlie, false),
            Condition.Knowledge(FuneralNightContent.Victoria, FuneralNightContent.BreckenComfortedElena),
            Condition.Phase(FuneralNightContent.AfterFuneralNight));
    }
}
