using RoR2;
using UnityEngine;
using EntityStates;

namespace HunkMod.SkillStates.Hunk.Weapon.Railgun
{
    public class Charge : BaseHunkSkillState
    {
        public float baseDuration = 3f;

        private uint playID;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.hunk.railgunCharge = 0f;
            this.hunk.railgunMaxCharge = this.duration;

            this.playID = Util.PlaySound("sfx_hunk_railgun_charge", this.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            this.hunk.railgunCharge = 0f;
            AkSoundEngine.StopPlayingID(this.playID);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            AkSoundEngine.SetRTPCValue("Hunk_RailgunCharge", Util.Remap(base.fixedAge, 0f, this.duration, 0f, 1f));

            if (base.isAuthority)
            {
                this.hunk.railgunCharge = base.fixedAge;

                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextState(new Shoot());
                    return;
                }

                if (!this.hunk.isAiming || !this.inputBank.skill1.down)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}