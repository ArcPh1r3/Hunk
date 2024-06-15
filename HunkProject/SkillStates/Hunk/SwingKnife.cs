using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using RoR2.Skills;

namespace HunkMod.SkillStates.Hunk
{
    public class SwingKnife : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;
        private GameObject knifeInstance;
        private bool knifeHidden;

        public override void OnEnter()
        {
            this.hunk = this.GetComponent<Modules.Components.HunkController>();

            SkillDef knifeSkinDef = this.hunk.knifeSkin;
            if (knifeSkinDef)
            {
                if (Modules.Survivors.Hunk.knifeSkins.ContainsKey(knifeSkinDef))
                {
                    this.prop = "";
                    this.knifeInstance = this.CreateKnife(Modules.Survivors.Hunk.knifeSkins[knifeSkinDef]);
                }
                else
                {
                    // hidden blade is the only case for now.
                    this.prop = "HiddenKnifeModel";
                }
            }

            this.hitboxName = "Knife";

            this.swingIndex = Random.Range(0, 3);

            this.damageCoefficient = 3.5f;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            this.baseEarlyExitTime = 0.55f;
            this.attackRecoil = 5f / this.attackSpeedStat;
            this.hitHopVelocity = 8f;

            this.attackStartTime = 0.265f;
            this.attackEndTime = 0.5f;

            this.hitStopDuration = 0.12f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_hunk_swing_knife";
            this.hitSoundString = "";
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = DamageType.ApplyMercExpose | DamageType.ClayGoo;

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

            if (this.hunk.variant)
            {
                this.damageCoefficient = 4.4f;
            }

            base.OnEnter();

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

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

        private GameObject CreateKnife(GameObject modelPrefab)
        {
            if (this.hunk.variant) return null;

            GameObject newKnife = GameObject.Instantiate(modelPrefab);

            newKnife.transform.parent = this.FindModelChild("KnifeBase");
            newKnife.transform.localPosition = Vector3.zero;
            newKnife.transform.localRotation = Quaternion.identity;
            newKnife.transform.localScale = Vector3.one;

            return newKnife;
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
                if (this.prop != "") this.FindModelChild(this.prop).gameObject.SetActive(false);
                if (this.knifeInstance) Destroy(this.knifeInstance);
            }
        }

        public override void OnExit()
        {
            if (!this.knifeHidden)
            {
                this.knifeHidden = true;
                if (this.prop != "") this.FindModelChild(this.prop).gameObject.SetActive(false);
                if (this.knifeInstance) Destroy(this.knifeInstance);
            }

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
            else return InterruptPriority.PrioritySkill;
        }
    }
}