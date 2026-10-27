namespace Inheritance.State
{
    // Prototype content vocabulary; GameState has no character or scene-specific branches.
    public static class FuneralNightContent
    {
        public const string BreckenComfortedElena = "BRECKEN_COMFORTED_ELENA";
        public const string BreckenMetCharlie = "BRECKEN_MET_CHARLIE_FUNERAL_NIGHT";
        public const string Elena = "Elena";
        public const string Victoria = "Victoria";
        public const string Resentment = "Resentment";
        public const string CharlieBusiness = "Charlie_FuneralNightBusiness";
        public const string FuneralNight = "FuneralNight";
        public const string AfterFuneralNight = "AfterFuneralNight";

        public static GameState CreateState()
        {
            var state = new GameState(FuneralNight, TimeBlock.Evening);
            state.SetOpportunityState(CharlieBusiness, OpportunityState.Available);
            return state;
        }
    }
}
