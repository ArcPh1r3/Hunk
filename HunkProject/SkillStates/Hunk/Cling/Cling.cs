using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Skills;

namespace HunkMod.SkillStates.Hunk.Cling
{
    public class Cling : GenericCharacterMain
    {
        public HurtBox targetHurtbox;
        public Vector3 offset;
        public GameObject anchor;

        private Transform modelTransform;
        private Modules.Components.HunkController hunk;
        private bool cancelling;
        private bool knifeWasActive;

        private GameObject knifeInstance;
        private bool knifeHidden;
        private string prop;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterDirection.enabled = false;
            this.modelLocator.enabled = false;
            this.modelTransform = this.GetModelTransform();
            this.animator = this.GetModelAnimator();
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

            //this.hunk.clingTimer = 8f;

            this.skillLocator.primary.SetSkillOverride(this, Modules.Survivors.Hunk.clingSlashSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            //this.skillLocator.secondary.SetSkillOverride(this, Content.Survivors.RedGuy.clingStabSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            //this.skillLocator.special.SetSkillOverride(this, Content.Survivors.RedGuy.clingFlourishSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            /*if (this.skillLocator.utility.skillDef.skillNameToken == Content.Survivors.RedGuy.healNameToken)
            {
                this.skillLocator.utility.SetSkillOverride(this, Content.Survivors.RedGuy.clingHealSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            else
            {
                this.skillLocator.utility.SetSkillOverride(this, Content.Survivors.RedGuy.clingBeamSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }

            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, RavagerCameraParams.CLING, 1f);
            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody, Modules.Assets.clingCrosshair, CrosshairUtils.OverridePriority.Skill);*/

            base.gameObject.layer = LayerIndex.ignoreRaycast.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            if (this.prop != "") this.FindModelChild(this.prop).gameObject.SetActive(true);

            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 0f);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 0f);

            this.GetModelChildLocator().FindChild("Weapon").gameObject.SetActive(false);
            this.knifeWasActive = this.FindModelChild("KnifeModel").gameObject.activeSelf;
            this.FindModelChild("KnifeModel").gameObject.SetActive(false);
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

        public override void OnExit()
        {
            if (!this.cancelling) this.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (!this.knifeHidden)
            {
                this.knifeHidden = true;
                if (this.prop != "") this.FindModelChild(this.prop).gameObject.SetActive(false);
                if (this.knifeInstance) Destroy(this.knifeInstance);
            }

            this.GetModelChildLocator().FindChild("Weapon").gameObject.SetActive(true);
            this.FindModelChild("KnifeModel").gameObject.SetActive(this.knifeWasActive);

            this.skillLocator.utility.AddOneStock();
            this.skillLocator.utility.stock = this.skillLocator.utility.maxStock;

            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimYaw"), 1f);
            this.animator.SetLayerWeight(this.animator.GetLayerIndex("AimPitch"), 1f);

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
            this.characterDirection.enabled = true;
            this.modelLocator.enabled = true;

            //this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
            //if (this.crosshairOverrideRequest != null) this.crosshairOverrideRequest.Dispose();

            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.clingSlashSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            /*this.skillLocator.secondary.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingStabSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.special.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingFlourishSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (this.skillLocator.utility.skillDef.skillNameToken == Content.Survivors.RedGuy.healNameToken)
            {
                this.skillLocator.utility.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingHealSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            else
            {
                this.skillLocator.utility.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingBeamSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }*/
        }

        public override void HandleMovements()
        {
        }

        public override void ProcessJump()
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.characterBody.isSprinting = false;

            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = -0.1f;
            this.skillLocator.utility.stock = 0;
            this.skillLocator.utility.rechargeStopwatch = -0.1f;
            this.skillLocator.special.stock = 0;
            this.skillLocator.special.rechargeStopwatch = -0.1f;

            if (base.isAuthority)
            {
                if (this.targetHurtbox && this.anchor && this.targetHurtbox.healthComponent.alive)
                {
                    this.characterMotor.Motor.SetPosition(this.anchor.transform.position);

                    if (this.modelTransform)
                    {
                        this.modelTransform.rotation = Util.QuaternionSafeLookRotation((this.targetHurtbox.transform.position - this.transform.position).normalized);
                        this.modelTransform.position = this.transform.position;
                    }
                }
                else
                {
                    this.cancelling = true;
                }

                if (this.inputBank.jump.justPressed) this.cancelling = true;
                //if (this.penis.clingTimer <= 0f) this.cancelling = true;

                this.characterMotor.velocity = Vector3.zero;
            }

            if (this.cancelling)
            {
                this.hunk.immobilized = false;
                this.outer.SetNextState(new SkillStates.Hunk.Counter.GenericCounterAirDodge());
                return;
            }
        }
    }
}