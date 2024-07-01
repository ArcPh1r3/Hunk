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
        //private Animator animator;
        private bool isToggleMode;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.hideCrosshair = false;
            this.hunk.isAiming = true;
            this.isToggleMode = Modules.Config.toggleAim.Value;
            //this.animator = this.GetModelAnimator();
            if (!this.camParamsOverrideHandle.isValid) this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, HunkCameraParams.AIM, 0.5f);

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            // bullet time
            if (this.hunk.lockOnTimer > 0f)
            {
                this.hunk.lockOnTimer = 0.666f;
                if (NetworkServer.active) this.characterBody.AddTimedBuff(Modules.Survivors.Hunk.bulletTimeBuff, 0.666f);
            }
            //this.hunk.lockOnTimer = 0f;

            if (!this.hunk.isReloading)
            {
                this.PlayAnim();
                Util.PlaySound("sfx_hunk_smg_aim", this.gameObject);
            }
            else
            {
                if (this.hunk.ammo > 0)
                {
                    this.PlayAnim();
                    Util.PlaySound("sfx_hunk_smg_aim", this.gameObject);
                }
            }

            if (this.hunk.weaponTracker.variantHandler) base.PlayCrossfade("AimPitch", "AimPitchJacket", 0.1f);
            else base.PlayCrossfade("AimPitch", "AimPitchAiming", 0.1f);

            if (this.hunk.weaponDef.exposeWeakPoints)
            {
                this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
                {
                    prefab = Modules.Assets.headshotOverlay,
                    childLocatorEntry = "ScopeContainer"
                });
            }
        }

        protected virtual void PlayAnim()
        {
            base.PlayCrossfade("Gesture, Override", "AimIn", "Action.playbackRate", 0.25f, 0.05f);
        }

        protected virtual void PlayExitAnim()
        {
            //if (!this.hunk.isReloading) base.PlayCrossfade("Gesture, Override", "AimOut", "Action.playbackRate", 0.6f, 0.05f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;
            this.hunk.ammoKillTimer = 3f;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;

            this.characterBody.hideCrosshair = this.hunk.isReloading;

            if (!this.hunk.isRolling) base.characterBody.SetAimTimer(0.2f);

            if (this.hunk.ammo <= 0)
            {
                this.skillLocator.primary.UnsetSkillOverride(this.gameObject, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
                this.skillLocator.primary.SetSkillOverride(this.gameObject, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);
            }
            else
            {
                this.skillLocator.primary.SetSkillOverride(this.gameObject, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
                this.skillLocator.primary.UnsetSkillOverride(this.gameObject, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);
            }

            if (base.fixedAge > 0.05f) this.skillLocator.primary.stock = 1;
            else this.skillLocator.primary.stock = 0;

            if (base.fixedAge >= 0.1f)
            {
                if (this.isToggleMode)
                {
                    if (this.inputBank.skill2.justPressed)
                    {
                        this.skillLocator.secondary.stock = 0;
                        this.skillLocator.secondary.rechargeStopwatch = 0f;
                        if (base.isAuthority) this.outer.SetNextStateToMain();
                    }
                }
                else
                {
                    if (!this.inputBank.skill2.down && base.isAuthority)
                    {
                        this.outer.SetNextStateToMain();
                    }
                }

                if (this.inputBank.skill4.down && base.isAuthority)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            this.characterBody.hideCrosshair = true;
            this.hunk.isAiming = false;
            this.hunk.lockOnTimer = -1f;

            if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            this.PlayExitAnim();
            base.PlayCrossfade("AimPitch", "AimPitch", 0.1f);
            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }

            this.skillLocator.primary.UnsetSkillOverride(this.gameObject, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.primary.UnsetSkillOverride(this.gameObject, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}