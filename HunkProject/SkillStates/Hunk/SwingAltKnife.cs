using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.SkillStates.Hunk
{
    public class SwingAltKnife : BaseMeleeAttack
    {
        protected override string prop => "HiddenKnifeModel";
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.characterBody.canPerformBackstab = true;
            this.hitboxName = "Knife";

            this.swingIndex = Random.Range(0, 3);

            this.damageCoefficient = 3.5f;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            this.baseEarlyExitTime = 0.55f;
            this.attackRecoil = 5f / this.attackSpeedStat;

            this.attackStartTime = 0.265f;
            this.attackEndTime = 0.5f;

            this.hitStopDuration = 0.12f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_hunk_swing_knife";
            this.swingEffectPrefab = Modules.Assets.knifeSwingEffectRed;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.knifeImpactEffectRed;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = DamageType.Generic;

            switch (this.swingIndex)
            {
                case 0:
                    this.muzzleString = "KnifeSwingMuzzle";
                    break;
                case 1:
                    this.muzzleString = "KnifeSwingMuzzle2";
                    break;
                case 2:
                    this.muzzleString = "KnifeSwingMuzzle3";
                    break;
            }

            base.OnEnter();

            Util.PlaySound("sfx_hunk_foley_knife", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.hunk.isAiming && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            this.characterBody.canPerformBackstab = false;
            base.OnExit();
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
                    if (this.swingIndex == 1) this.swingEffectInstance.transform.localScale *= 0.75f;
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
            base.PlayCrossfade("Gesture, Override", "SwingKnife" + (1 + this.swingIndex), "Knife.playbackRate", this.duration, 0.1f);
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