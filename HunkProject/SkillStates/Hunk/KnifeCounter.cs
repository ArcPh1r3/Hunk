using RoR2;
using EntityStates;
using HunkMod.SkillStates.BaseStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2.Skills;

namespace HunkMod.SkillStates.Hunk
{
    public class KnifeCounter : BaseMeleeAttack
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

            this.damageCoefficient = 8f;
            this.pushForce = 200f;
            this.baseDuration = 1.25f;
            this.baseEarlyExitTime = 0.55f;
            this.attackRecoil = 5f / this.attackSpeedStat;

            this.attackStartTime = 0.165f;
            this.attackEndTime = 0.25f;

            this.hitStopDuration = 0.24f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_hunk_swing_knife";
            this.swingEffectPrefab = Modules.Assets.knifeSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.knifeImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = DamageType.Stun1s | DamageType.ClayGoo;
            this.muzzleString = "KnifeSwingMuzzle";

            base.OnEnter();

            if (this.hunk.variant)
            {
                Util.PlaySound("sfx_hunk_kick_foley", this.gameObject);
                this.hitStopDuration *= 1.25f;
                this.swingSoundString = "sfx_jacket_swing_bat";
                this.attack.impactSound = Modules.Assets.batImpactSoundDef.index;
                this.attack.hitEffectPrefab = Modules.Assets.batImpactEffect;
                this.swingEffectPrefab = Modules.Assets.batSwingEffect;
            }
            else Util.PlaySound("sfx_hunk_foley_knife", this.gameObject);

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;
            this.hunk.lockOnTimer = -1f;
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
            this.characterMotor.velocity = this.characterDirection.forward * -25f;

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

            this.hunk.TriggerCounter();

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
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
            base.PlayCrossfade("FullBody, Override", "KnifeCounter", "Knife.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch >= (0.5f * this.duration)) return InterruptPriority.Any;
            else return InterruptPriority.Frozen;
        }
    }
}