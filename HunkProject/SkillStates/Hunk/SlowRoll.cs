using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk
{
    public class SlowRoll : BaseHunkSkillState
    {
        protected Vector3 slipVector = Vector3.zero;
        public float duration = 1.3f;
        //private Vector3 cachedForward;
        private bool peepee;
        private float coeff = 8f;
        private bool skidibi;

        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.isRolling = true;
            this.slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;

            base.PlayCrossfade("FullBody, Override", "DodgeRoll", "Dodge.playbackRate", this.duration * 1.4f, 0.05f);

            Util.PlaySound("sfx_hunk_roll", this.gameObject);

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            this.hunk.desiredYOffset = 0.6f;
            this.hunk.immobilized = true;

            this.ApplyBuff();
            this.CreateDashEffect();
        }

        public virtual void ApplyBuff()
        {
        }

        public virtual void CreateDashEffect()
        {
        }

        public override void FixedUpdate()
        {
            if (this.peepee && base.isAuthority)
            {
                this.characterMotor.jumpCount = 0;
                if (this.inputBank.jump.justPressed)
                {
                    base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.05f);
                    this.characterMotor.Motor.ForceUnground();
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                this.characterMotor.jumpCount = this.characterBody.maxJumpCount;
            }

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
            this.hunk.immobilized = false;
            this.characterMotor.jumpCount = 0;
            this.hunk.desiredYOffset = this.hunk.defaultYOffset;
            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.counterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            base.OnExit();

            if (base.isAuthority && this.inputBank.moveVector != Vector3.zero) this.characterBody.isSprinting = true;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.peepee) return InterruptPriority.Any;
            return InterruptPriority.Frozen;
        }
    }
}