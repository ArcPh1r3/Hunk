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

            base.PlayAnimation("AimPitch", "SteadyAimPitch");

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.PlayAnim();
            Util.PlaySound("sfx_driver_aim_foley", this.gameObject);

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
            base.PlayAnimation("Gesture, Override", "SteadyAim", "Action.playbackRate", 0.25f);
        }

        protected virtual void PlayExitAnim()
        {
            base.PlayAnimation("Gesture, Override", "SteadyAimEnd", "Action.playbackRate", 0.2f);
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

            //this.UpdateLightEffect();

            if (!this.inputBank.skill2.down && base.isAuthority)
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

            //this.FindModelChild("PistolSight").gameObject.SetActive(false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}