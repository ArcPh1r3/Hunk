using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class Punch : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;
        private bool sprintBuffer;

        public override void OnEnter()
        {
            this.hitboxName = "Knife";

            this.damageCoefficient = 15f;
            this.pushForce = 0f;
            this.bonusForce = this.GetAimRay().direction * 1000f + (Vector3.up * 500f);
            this.baseDuration = 1f;
            this.baseEarlyExitTime = 0.65f;
            this.attackRecoil = 15f / this.attackSpeedStat;

            this.attackStartTime = 0.13f;
            this.attackEndTime = 0.15f;

            this.hitStopDuration = 0.4f;
            this.smoothHitstop = false;

            this.swingSoundString = "sfx_hunk_kick_swing";
            this.swingEffectPrefab = Modules.Assets.kickSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.kickImpactEffect;
            this.impactSound = Modules.Assets.kickImpactSoundDef.index;

            this.damageType = DamageType.Stun1s | DamageType.ClayGoo;
            this.muzzleString = "PunchSwingMuzzle";

            base.OnEnter();

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;
            this.hunk.lockOnTimer = -1f;

            Util.PlaySound("sfx_hunk_kick_foley", this.gameObject);
        }

        public override void FixedUpdate()
        {
            if (this.attack != null) this.attack.forceVector = this.GetAimRay().direction * 1000f + (Vector3.up * 500f);
            base.FixedUpdate();
            this.characterBody.isSprinting = false;

            if (this.stopwatch >= (this.attackStartTime * this.duration)) this.characterMotor.moveDirection *= 0.05f;
            else this.characterMotor.moveDirection *= 0f;

            if (base.isAuthority && this.inputBank.sprint.justPressed) this.sprintBuffer = true;

            if (this.hunk.isAiming && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("MeleePivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }

            base.FireAttack();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.sprintBuffer || this.inputBank.moveVector != Vector3.zero) this.characterBody.isSprinting = true;
        }

        protected override void PlaySwingEffect()
        {
            this.characterMotor.velocity = this.characterDirection.forward * 15f;

            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }
        }

        protected override void TriggerHitStop()
        {
            base.TriggerHitStop();

            if (NetworkServer.active) this.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, this.hitStopDuration);

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
        }

        protected override void ClearHitStop()
        {
            base.ClearHitStop();

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = fuck.initialDuration;
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            base.PlayCrossfade("FullBody, Override", "CounterPunch", "Knife.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch >= (0.25f * this.duration)) return InterruptPriority.Any;
            else return InterruptPriority.Frozen;
        }
    }
}