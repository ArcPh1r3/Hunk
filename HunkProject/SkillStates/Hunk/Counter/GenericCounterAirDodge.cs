namespace HunkMod.SkillStates.Hunk.Counter
{
    public class GenericCounterAirDodge : AirDodge
    {
        protected override bool forcePerfect => true;

        protected override void SetNextState()
        {
            this.outer.SetNextState(new SlowRoll());
        }
    }
}