using EntityStates;
using RoR2;
using UnityEngine;

namespace HunkMod.SkillStates.Hunk
{
    public class Dodge : BaseHunkSkillState
    {
        protected Vector3 slipVector = Vector3.zero;
        public float duration = 1.1f;
        private Vector3 cachedForward;
        private bool peepee;
        private float coeff = 24f;
        private bool skidibi;

        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.isRolling = true;
            this.slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            this.cachedForward = this.characterDirection.forward;

            Animator anim = this.GetModelAnimator();

            Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.slipVector;
            Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
            float num = Vector3.Dot(this.slipVector, rhs);
            float num2 = Vector3.Dot(this.slipVector, rhs2);
            anim.SetFloat("dashF", num);
            anim.SetFloat("dashR", num2);

            base.PlayAnimation("Reload", "BufferEmpty");
            base.PlayCrossfade("FullBody, Override", "DodgeRoll", "Dodge.playbackRate", this.duration * 1.4f, 0.05f);
            //base.PlayAnimation("Gesture, Override", "BufferEmpty");

            Util.PlaySound("sfx_driver_dash", this.gameObject);

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            this.hunk.desiredYOffset = 0.6f;

            this.ApplyBuff();
            this.CreateDashEffect();
        }

        public virtual void ApplyBuff()
        {
            //if (base.isAuthority) base.healthComponent.AddBarrierAuthority(StaticValues.dashBarrierAmount * base.healthComponent.fullBarrier);
        }

        public virtual void CreateDashEffect()
        {
            /*EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.slipVector);
            effectData.origin = base.characterBody.corePosition;

            EffectManager.SpawnEffect(Modules.Assets.dashFX, effectData, false);*/
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.characterBody.aimTimer = -1f;
            this.hunk.reloadTimer = 1f;
            this.inputBank.moveVector = Vector3.zero;
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion = this.slipVector * (this.coeff * Time.fixedDeltaTime) * Mathf.Cos(base.fixedAge / this.duration * 1.57079637f);

            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    //base.characterDirection.forward = this.cachedForward;
                    base.characterDirection.forward = this.slipVector;
                }
            }

            if (!this.peepee && base.fixedAge >= (0.4f * this.duration))
            {
                this.peepee = true;
                this.coeff = 4f;
            }

            if (!this.skidibi && base.fixedAge >= (0.85f * this.duration))
            {
                this.skidibi = true;
                this.hunk.desiredYOffset = this.hunk.defaultYOffset;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public virtual void DampenVelocity()
        {
            base.characterMotor.velocity *= 0.75f;
        }

        public override void OnExit()
        {
            this.DampenVelocity();
            this.hunk.isRolling = false;
            //base.PlayAnimation("FullBody, Override", "BufferEmpty");
            this.hunk.desiredYOffset = this.hunk.defaultYOffset;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.peepee) return InterruptPriority.Any;
            return InterruptPriority.Frozen;
        }
    }
}