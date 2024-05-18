using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk
{
    public class Roll : BaseHunkSkillState
    {
        protected override bool turningAllowed => false;
        protected Vector3 slipVector = Vector3.zero;
        public float duration = 1.1f;
        //private Vector3 cachedForward;
        private bool peepee;
        private float coeff = 24f;
        private bool skidibi;
        private bool slowing;
        private float currentTimeScale = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.lockOnTimer = 1.5f;
            this.hunk.TriggerDodge();
            this.hunk.isRolling = true;
            this.slipVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            //this.cachedForward = this.characterDirection.forward;

            /*Animator anim = this.GetModelAnimator();

            Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.slipVector;
            Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
            float num = Vector3.Dot(this.slipVector, rhs);
            float num2 = Vector3.Dot(this.slipVector, rhs2);
            anim.SetFloat("dashF", num);
            anim.SetFloat("dashR", num2);*/

            base.PlayCrossfade("FullBody, Override", "DodgeRoll", "Dodge.playbackRate", this.duration * 1.4f, 0.05f);
            //base.PlayAnimation("Gesture, Override", "BufferEmpty");

            //Util.PlaySound("sfx_driver_dash", this.gameObject);
            Util.PlaySound("sfx_hunk_roll2", this.gameObject);
            if (base.isAuthority) Util.PlaySound("sfx_hunk_dodge_perfect", this.gameObject);

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            this.skillLocator.utility.AddOneStock();
            this.skillLocator.utility.stock = this.skillLocator.utility.maxStock;

            this.skillLocator.primary.SetSkillOverride(this, Modules.Survivors.Hunk.counterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            this.hunk.desiredYOffset = 0.6f;
            this.hunk.immobilized = true;
            this.hunk.iFrames = 0.75f;

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
                    //base.characterDirection.forward = this.cachedForward;
                    base.characterDirection.forward = this.slipVector;
                }
            }

            if (!this.peepee && base.fixedAge >= (0.4f * this.duration))
            {
                this.peepee = true;
                this.coeff = 4f;

                // ended up feeling bad
                /*if (RoR2Application.isInSinglePlayer && this.inputBank.skill2.down)
                {
                    this.slowing = true;
                    this.currentTimeScale = 0.1f;
                }*/
            }

            if (!this.skidibi && base.fixedAge >= (0.85f * this.duration))
            {
                this.skidibi = true;
                this.hunk.desiredYOffset = this.hunk.defaultYOffset;
            }

            if (this.slowing)
            {
                this.currentTimeScale += Time.unscaledDeltaTime * 1.1f;
                if (this.currentTimeScale >= 1f)
                {
                    this.slowing = false;
                    this.currentTimeScale = 1f;
                }
                Time.timeScale = this.currentTimeScale;
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
            this.hunk.immobilized = false;

            this.DampenVelocity();
            this.hunk.isRolling = false;
            this.characterMotor.jumpCount = 0;
            this.hunk.desiredYOffset = this.hunk.defaultYOffset;
            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.counterSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            base.OnExit();

            /*if (RoR2Application.isInSinglePlayer)
            {
                Time.timeScale = 1f;
            }*/

            if (base.isAuthority && this.inputBank.moveVector != Vector3.zero) this.characterBody.isSprinting = true;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.peepee) return InterruptPriority.Any;
            return InterruptPriority.Frozen;
        }
    }
}