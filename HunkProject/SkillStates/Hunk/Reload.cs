using EntityStates;
using UnityEngine;
using RoR2;

namespace HunkMod.SkillStates.Hunk
{
    public class Reload : BaseHunkSkillState
    {
        public float baseDuration = 2.4f;
        public string animString = "Reload";
        public InterruptPriority interruptPriority = InterruptPriority.Skill;

        private float duration;
        private bool wasAiming;
        private bool success;
        private bool cummed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.hunk.isReloading = true;
            this.wasAiming = this.hunk.isAiming;

            if (this.hunk.weaponTracker.weaponData[this.hunk.weaponTracker.equippedIndex].totalAmmo <= 0)
            {
                this.duration = 0.3f;
                this.success = false;
                Util.PlaySound("sfx_hunk_gun_click", this.gameObject);
            }
            else
            {
                base.PlayCrossfade("Gesture, Override", this.animString, "Reload.playbackRate", this.duration, 0.05f);
                this.success = true;
                Util.PlaySound("sfx_hunk_smg_reload_01", this.gameObject);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            this.hunk.isReloading = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;
            this.hunk.ammoKillTimer = 3f;

            if (!this.hunk.isAiming) this.wasAiming = false;

            if (base.isAuthority && this.inputBank.skill2.down && !this.wasAiming) // aiming to cancel a passive reload
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= (0.84f * this.duration) && !this.cummed)
            {
                this.cummed = true;
                if (this.success) this.hunk.FinishReload();
            }

            if (base.isAuthority && this.inputBank.skill3.down && this.skillLocator.utility.stock > 0) // roll cancel
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return this.interruptPriority;
        }
    }
}