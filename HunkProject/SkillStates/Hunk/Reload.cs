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

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.GetModelAnimator().SetFloat("aimBlend", 1f);
            base.PlayCrossfade("Gesture, Override", this.animString, "Action.playbackRate", this.duration * 1.1f, 0.1f);
            Util.PlaySound("sfx_driver_reload_01", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.hunk.FinishReload();

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