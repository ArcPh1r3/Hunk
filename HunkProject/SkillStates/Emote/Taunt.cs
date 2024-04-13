namespace HunkMod.SkillStates.Emote
{
    public class Taunt : BaseEmote
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.PlayEmote("Taunt", "", 1.5f);

            RoR2.Util.PlaySound("sfx_driver_boom", this.gameObject);
        }
    }
}