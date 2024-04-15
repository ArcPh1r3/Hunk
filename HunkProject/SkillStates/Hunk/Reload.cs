using EntityStates;
using UnityEngine;
using RoR2;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk
{
    public class Reload : BaseHunkSkillState
    {
        public float baseDuration = 2.4f;
        public string animString = "Reload";
        public InterruptPriority interruptPriority = InterruptPriority.PrioritySkill;
        public CameraParamsOverrideHandle camParamsOverrideHandle;

        private float duration;
        private bool wasAiming;
        private bool success;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.wasAiming = this.hunk.isAiming;

            if (this.hunk.weaponTracker.weaponData[this.hunk.weaponTracker.equippedIndex].totalAmmo <= 0)
            {
                this.duration = 0.3f;
                this.success = false;
                Util.PlaySound("sfx_driver_reload_01", this.gameObject);
            }
            else
            {
                base.PlayCrossfade("Reload", this.animString, "Action.playbackRate", this.duration * 1.1f, 0.25f);
                this.success = true;
                Util.PlaySound("sfx_driver_reload_01", this.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;

            if (this.hunk.isAiming && !this.wasAiming) // aiming to cancel a passive reload
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                if (this.success) this.hunk.FinishReload();
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