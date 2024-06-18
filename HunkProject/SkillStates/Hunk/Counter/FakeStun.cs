using UnityEngine;
using EntityStates;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class FakeStun : BaseState
    {
        public float duration;

        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = this.GetModelAnimator();
            base.PlayCrossfade("Body", "Idle", 0.2f);
            base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.2f);

            if (this.animator)
            {
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 0f);
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 0f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.animator)
            {
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 1f);
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}