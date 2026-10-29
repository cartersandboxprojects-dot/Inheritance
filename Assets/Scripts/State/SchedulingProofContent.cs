namespace Inheritance.State
{
    // Isolated systems proof. No scene hook and no Chapter 1 or funeral-night changes.
    public static class SchedulingProofContent
    {
        public const string Phase = "SchedulingProof";
        public const string FamilyId = "proof.family.visit_elena";
        public const string BusinessId = "proof.business.charlie_call";
        public const string MessageId = "proof.message.elena_thanks";
        public const string FamilyFact = "proof.family.visited";
        public const string ReactionFact = "proof.elena.reacted";
        public static readonly OpportunityDefinition Family = new OpportunityDefinition(FamilyId,
            ActionCost.ConsumeRemainder, Condition.Phase(Phase), TimeBlock.Afternoon, ActionCost.Free,
            new[] { new OpportunityCompetition(BusinessId, OpportunityState.Missed) },
            Effect.SetFact(FamilyFact, true), Effect.GrantKnowledge("Elena", FamilyFact));
        public static readonly OpportunityDefinition Business = new OpportunityDefinition(BusinessId,
            ActionCost.ConsumeRemainder, Condition.Phase(Phase), TimeBlock.Afternoon, ActionCost.Free,
            new[] { new OpportunityCompetition(FamilyId, OpportunityState.Deferred) });
        public static readonly MessageDefinition Thanks = new MessageDefinition(MessageId, "Elena",
            "Thank you for spending the afternoon with me.",
            Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Evening),
                Condition.Opportunity(FamilyId, OpportunityState.Completed), Condition.Fact(FamilyFact)), FamilyId);
        public static readonly Condition LaterReaction = Condition.All(Condition.Phase(Phase), Condition.Time(TimeBlock.Evening),
            Condition.Opportunity(FamilyId, OpportunityState.Completed), Condition.Opportunity(BusinessId, OpportunityState.Missed),
            Condition.Message(MessageId, MessageStatus.Read), Condition.Fact(FamilyFact), Condition.Knowledge("Elena", FamilyFact));
        public static readonly Consequence React = new Consequence(Condition.All(LaterReaction, Condition.Fact(ReactionFact, false)),
            ActionCost.Free, Effect.SetFact(ReactionFact, true));
        public static GameState CreateState()
        {
            var state = new GameState(Phase, TimeBlock.Afternoon);
            Evaluate(state); return state;
        }
        // Explicit call after actions/load at authored boundaries. No pending-event simulator.
        public static void Evaluate(GameState state)
        {
            Family.TryMakeAvailable(state); Business.TryMakeAvailable(state);
            Thanks.TryQueue(state); Thanks.TryDeliver(state);
        }
    }
}
