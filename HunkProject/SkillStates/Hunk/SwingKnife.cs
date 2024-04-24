using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.SkillStates.Hunk
{
    public class SwingKnife : BaseMeleeAttack
    {
        protected override string prop => "KnifeModel";
        private GameObject swingEffectInstance;
        private bool knifeHidden;

        public override void OnEnter()
        {
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
            this.hitSoundString = "";
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

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            Util.PlaySound("sfx_hunk_foley_knife", this.gameObject);

            if (this.isCrit)
            {
                this.swingEffectPrefab = Modules.Assets.knifeSwingEffectRed;
                this.hitEffectPrefab = Modules.Assets.knifeImpactEffectRed;
                this.attack.hitEffectPrefab = this.hitEffectPrefab;
                this.attack.impactSound = this.impactSound;
            }
            else
            {
                this.swingEffectPrefab = Modules.Assets.knifeSwingEffect;
                this.hitEffectPrefab = Modules.Assets.knifeImpactEffect;
                this.attack.hitEffectPrefab = this.hitEffectPrefab;
                this.attack.impactSound = this.impactSound;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.hunk.isAiming && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (!this.knifeHidden && this.stopwatch >= (0.85f * this.duration))
            {
                this.knifeHidden = true;
                this.FindModelChild(this.prop).gameObject.SetActive(false);
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
            else return InterruptPriority.PrioritySkill;
        }
    }
}