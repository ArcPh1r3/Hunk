using RoR2;
using UnityEngine;
using static RoR2.CameraTargetParams;
using EntityStates;
using HunkMod.Modules;
using BepInEx.Configuration;

namespace HunkMod.SkillStates.Emote
{
    public class BaseEmote : BaseState
    {
        private CameraParamsOverrideHandle camOverrideHandle;

        private Animator animator;
        private ChildLocator childLocator;
        private float duration;
        private uint activePlayID;
        public LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();
            this.FindLocalUser();

            this.characterBody.hideCrosshair = true;
            this.camOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, HunkCameraParams.EMOTE, 0.5f);
        }

        private void FindLocalUser()
        {
            if (localUser == null)
            {
                if (base.characterBody)
                {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
                    {
                        if (lu.cachedBody == base.characterBody)
                        {
                            this.localUser = lu;
                            break;
                        }
                    }
                }
            }
        }

        protected void PlayEmote(string animString, string soundString = "", float animDuration = 0)
        {
            this.PlayEmote(animString, soundString, GetModelAnimator(), animDuration);
        }

        protected void PlayEmote(string animString, string soundString, Animator animator, float animDuration = 0)
        {
            if (animDuration >= 0 && this.duration != 0)
                animDuration = this.duration;

            if (this.duration > 0)
            {
                PlayAnimationOnAnimator(animator, "FullBody, Override", animString, "Emote.playbackRate", animDuration);
            }
            else
            {
                animator.SetFloat("Emote.playbackRate", 1f);
                PlayAnimationOnAnimator(animator, "FullBody, Override", animString);
            }

            if (!string.IsNullOrEmpty(soundString))
            {
                activePlayID = Util.PlaySound(soundString, gameObject);
            };
        }

        public override void Update()
        {
            base.Update();

            //dance cancels lol
            if (base.isAuthority)
            {
                this.CheckEmote<Rest>(Config.restKey);
                this.CheckEmote<Taunt>(Config.tauntKey);
                this.CheckEmote<Dance>(Config.danceKey);
            }
        }

        private void CheckEmote(KeyCode keybind, EntityState state)
        {
            if (Input.GetKeyDown(keybind))
            {
                if (!localUser.isUIFocused)
                {
                    outer.SetInterruptState(state, InterruptPriority.Any);
                }
            }
        }

        private void CheckEmote<T>(ConfigEntry<KeyboardShortcut> keybind) where T : EntityState, new()
        {
            if (Modules.Config.GetKeyPressed(keybind))
            {
                FindLocalUser();

                if (localUser != null && !localUser.isUIFocused)
                {
                    outer.SetInterruptState(new T(), InterruptPriority.Any);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool endEmote = false;

            if (base.characterMotor)
            {
                if (!base.characterMotor.isGrounded) endEmote = true;
            }

            if (base.inputBank)
            {
                if (base.inputBank.skill1.down) endEmote = true;
                if (base.inputBank.skill2.down) endEmote = true;
                if (base.inputBank.skill3.down) endEmote = true;
                if (base.inputBank.skill4.down) endEmote = true;

                if (base.inputBank.moveVector != Vector3.zero) endEmote = true;
            }

            if (this.duration > 0 && base.fixedAge >= this.duration)
                endEmote = true;

            if (endEmote && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.characterBody.hideCrosshair = false;

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (this.activePlayID != 0) AkSoundEngine.StopPlayingID(this.activePlayID);

            base.cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}