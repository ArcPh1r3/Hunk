using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk.Counter
{
    public class Kick : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.hitboxName = "Knife";

            this.damageCoefficient = 7f;
            this.pushForce = 0f;
            this.bonusForce = this.GetAimRay().direction * 5000f + (Vector3.up * 1000f);
            this.baseDuration = 1.4f;
            this.baseEarlyExitTime = 0.65f;
            this.attackRecoil = 15f / this.attackSpeedStat;

            this.attackStartTime = 0.24f;
            this.attackEndTime = 0.37f;

            this.hitStopDuration = 0.4f;
            this.smoothHitstop = false;

            this.swingSoundString = "sfx_hunk_swing_knife";
            this.swingEffectPrefab = Modules.Assets.kickSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.kickImpactEffect;
            this.impactSound = Modules.Assets.kickImpactSoundDef.index;

            this.damageType = DamageType.Stun1s | DamageType.ClayGoo;
            this.muzzleString = "KickSwingMuzzle";

            base.OnEnter();

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;
            this.hunk.lockOnTimer = -1f;

            Util.PlaySound("sfx_hunk_foley_knife", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterMotor.moveDirection *= 0.2f;

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
            base.PlayCrossfade("FullBody, Override", "CounterKick", "Knife.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch >= (0.75f * this.duration)) return InterruptPriority.Any;
            else return InterruptPriority.Frozen;
        }
    }
}