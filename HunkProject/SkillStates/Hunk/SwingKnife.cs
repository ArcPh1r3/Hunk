using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.SkillStates.Hunk
{
    public class SwingKnife : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.hitboxName = "Knife";

            this.damageCoefficient = 4.7f;
            this.pushForce = 200f;
            this.baseDuration = 1.2f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 5f / this.attackSpeedStat;

            this.attackStartTime = 0.13f;
            this.attackEndTime = 0.5f;

            this.hitStopDuration = 0.18f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_driver_swing_knife";
            this.swingEffectPrefab = Modules.Assets.knifeSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.knifeImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = DamageType.ApplyMercExpose;

            this.muzzleString = "KnifeSwingMuzzle";

            base.OnEnter();

            Util.PlaySound("sfx_driver_foley_knife", this.gameObject);
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
            base.PlayCrossfade("Gesture, Override", "SwingKnife", "Knife.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch >= (0.5f * this.duration)) return InterruptPriority.Any;
            else return InterruptPriority.Pain;
        }
    }
}