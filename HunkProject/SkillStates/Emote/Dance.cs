namespace HunkMod.SkillStates.Emote
{
    public class Dance : BaseEmote
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.PlayEmote("Dance");
        }
    }
}