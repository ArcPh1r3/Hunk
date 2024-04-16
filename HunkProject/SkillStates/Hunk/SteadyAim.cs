using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;
using RoR2.HudOverlay;

namespace HunkMod.SkillStates.Hunk
{
    public class SteadyAim : BaseHunkSkillState
    {
        public CameraParamsOverrideHandle camParamsOverrideHandle;
        private OverlayController overlayController;
        //private GameObject lightEffectInstance;
        //private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.hideCrosshair = false;
            this.hunk.isAiming = true;
            //this.animator = this.GetModelAnimator();
            if (!this.camParamsOverrideHandle.isValid) this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, HunkCameraParams.AIM, 0.5f);

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.PlayAnim();
            Util.PlaySound("sfx_driver_aim_foley", this.gameObject);

            base.PlayCrossfade("AimPitch", "AimPitchAiming", 0.1f);

            //this.FindModelChild("PistolSight").gameObject.SetActive(true);

            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
            {
                prefab = Modules.Assets.headshotOverlay,
                childLocatorEntry = "ScopeContainer"
            });

            //this.lightEffectInstance = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("GunLight"));
        }

        protected virtual void PlayAnim()
        {
            base.PlayCrossfade("Gesture, Override", "AimIn", "Action.playbackRate", 0.25f, 0.05f);
        }

        protected virtual void PlayExitAnim()
        {
            base.PlayCrossfade("Gesture, Override", "AimOut", "Action.playbackRate", 0.6f, 0.05f);
        }

        /*private void UpdateLightEffect()
        {
            Ray ray = this.GetAimRay();
            RaycastHit raycastHit;
            if (Physics.Raycast(ray.origin, ray.direction, out raycastHit, Shoot.range, LayerIndex.CommonMasks.bullet))
            {
                this.lightEffectInstance.SetActive(true);
                this.lightEffectInstance.transform.position = raycastHit.point + (ray.direction * -0.3f);
            }
            else this.lightEffectInstance.SetActive(false);
        }*/

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            base.characterBody.SetAimTimer(0.2f);

            if (this.hunk.ammo <= 0)
            {
                this.skillLocator.primary.UnsetSkillOverride(this, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
                this.skillLocator.primary.SetSkillOverride(this, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);
            }
            else
            {
                this.skillLocator.primary.SetSkillOverride(this, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
                this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);
            }

            //this.UpdateLightEffect();

            if (!this.inputBank.skill2.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }

            if (this.inputBank.skill4.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            this.characterBody.hideCrosshair = true;
            this.hunk.isAiming = false;
            //if (this.lightEffectInstance) Destroy(this.lightEffectInstance);

            if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            this.PlayExitAnim();
            base.PlayCrossfade("AimPitch", "AimPitch", 0.1f);
            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }

            this.skillLocator.primary.UnsetSkillOverride(this, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);

            //this.FindModelChild("PistolSight").gameObject.SetActive(false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}