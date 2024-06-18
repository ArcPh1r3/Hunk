using UnityEngine;
using EntityStates;
using UnityEngine.AddressableAssets;
using RoR2;
using HunkMod.SkillStates.BaseStates;

namespace HunkMod.SkillStates.Hunk.Cling
{
    public class ClingSlash : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.hitboxName = "Knife";

            this.damageCoefficient = 3.8f;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0.15f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_hunk_swing_knife";
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = DamageType.ApplyMercExpose | DamageType.ClayGoo;

            this.muzzleString = "KnifeSwingMuzzle2";
            //if (this.swingIndex == 0) this.muzzleString = "SwingMuzzle2";
            //else this.muzzleString = "SwingMuzzle1";

            base.OnEnter();

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

            if (this.hunk.variant)
            {
                Util.PlaySound("sfx_hunk_kick_foley", this.gameObject);
                this.hitStopDuration *= 1.5f;
                this.swingSoundString = "sfx_jacket_swing_bat";
                this.attack.impactSound = Modules.Assets.batImpactSoundDef.index;
                this.attack.hitEffectPrefab = Modules.Assets.batImpactEffect;
                this.swingEffectPrefab = Modules.Assets.batSwingEffect;
            }
            else Util.PlaySound("sfx_hunk_foley_knife", this.gameObject);
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
            if (this.swingIndex == 1) base.PlayCrossfade("FullBody, Override", "ClingSlash", "Knife.playbackRate", this.duration, 0.1f);
            else base.PlayCrossfade("FullBody, Override", "ClingSlash", "Knife.playbackRate", this.duration, 0.1f);
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
