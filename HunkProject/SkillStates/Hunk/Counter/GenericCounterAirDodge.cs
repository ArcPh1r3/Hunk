namespace HunkMod.SkillStates.Hunk.Counter
{
    public class GenericCounterAirDodge : AirDodge
    {
        public bool fastLanding = false;
        public bool allowCounter = true;
        public bool resetCooldown = true;
        protected override bool forcePerfect => true;
        protected override bool forceFastLanding => false;

        protected override void SetNextState()
        {
            if (this.fastLanding) this.outer.SetNextState(new PerfectLanding
            {
                allowCounter = this.allowCounter,
                resetCooldown = this.resetCooldown
            });
            else this.outer.SetNextState(new SlowRoll());
        }
    }
}