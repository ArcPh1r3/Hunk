using EntityStates;
using UnityEngine;
using RoR2;

namespace HunkMod.SkillStates.Hunk
{
    public class RoundReload : BaseHunkSkillState
    {
        public float baseDuration = 2.4f;
        public InterruptPriority interruptPriority = InterruptPriority.Skill;

        private enum SubState
        {
            Startup,
            Loading,
            End
        }

        private float duration;
        private float stopwatch;
        private SubState subState;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.cachedWeaponDef.reloadDuration / this.attackSpeedStat;
            this.hunk.isReloading = true;

            this.subState = SubState.Startup;
            this.stopwatch = this.duration;

            base.PlayCrossfade("Gesture, Override", "RoundReload", "Reload.playbackRate", this.duration, 0.05f);

            // ugh
            if (this.hunk.weaponDef.animationSet == HunkWeaponDef.AnimationSet.Pistol) Util.PlaySound("sfx_hunk_revolver_reload", this.gameObject);
            else Util.PlaySound("sfx_hunk_shotgun_reload", this.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            this.hunk.isReloading = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;
            this.hunk.ammoKillTimer = 3f;
            this.stopwatch -= Time.fixedDeltaTime;

            if (base.isAuthority && this.inputBank.skill2.down && this.interruptPriority == InterruptPriority.Any && this.hunk.ammo > 0) // aiming to cancel a passive reload
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.isAuthority && this.inputBank.skill3.down && this.skillLocator.utility.stock > 0) // roll cancel
            {
                this.outer.SetNextStateToMain();
                return;
            }

            switch (this.subState)
            {
                case SubState.Startup:
                    if (this.stopwatch < 0f)
                    {
                        this.hunk.ReloadRound();
                        this.subState = SubState.Loading;
                        this.stopwatch = 0.4f / this.attackSpeedStat;
                        base.PlayAnimation("Gesture, Override", "RoundReloadLoop", "Reload.playbackRate", 0.4f / this.attackSpeedStat);
                    }
                    break;
                case SubState.Loading:
                    if (this.stopwatch < 0f)
                    {
                        if (this.hunk.ammo >= this.hunk.maxAmmo || this.hunk.weaponTracker.weaponData[this.hunk.weaponTracker.equippedIndex].totalAmmo <= 0)
                        {
                            this.hunk.FinishRoundReload();
                            this.subState = SubState.End;
                            this.stopwatch = this.duration;

                            // ugh
                            if (this.hunk.weaponDef.animationSet == HunkWeaponDef.AnimationSet.Pistol)
                            {
                                base.PlayAnimation("Gesture, Override", "RoundReloadOutPistol", "Reload.playbackRate", this.duration);
                                Util.PlaySound("sfx_hunk_revolver_reload_end", this.gameObject);
                            }
                            else
                            {
                                base.PlayAnimation("Gesture, Override", "RoundReloadOut", "Reload.playbackRate", this.duration);
                                Util.PlaySound("sfx_hunk_shotgun_reload_end", this.gameObject);
                            }
                        }
                        else
                        {
                            this.hunk.ReloadRound();
                            this.stopwatch = 0.4f / this.attackSpeedStat;
                        }
                    }
                    break;
                case SubState.End:
                    if (this.stopwatch < 0f)
                    {
                        if (base.isAuthority)
                        {
                            this.outer.SetNextStateToMain();
                        }
                    }
                    break;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge <= 0.1f) return InterruptPriority.Frozen;
            return this.interruptPriority;
        }
    }
}