using EntityStates;
using UnityEngine;
using RoR2;

namespace HunkMod.SkillStates.Hunk
{
    public class Reload : BaseHunkSkillState
    {
        public float baseDuration = 2.4f;
        public string animString = "Reload";
        public InterruptPriority interruptPriority = InterruptPriority.Skill;

        private float duration;
        private bool success;
        private bool cummed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.cachedWeaponDef.reloadDuration / this.attackSpeedStat;
            this.hunk.isReloading = true;

            if (this.hunk.weaponTracker.weaponData[this.hunk.weaponTracker.equippedIndex].totalAmmo <= 0)
            {
                this.duration = 0.3f;
                this.success = false;
                Util.PlaySound("sfx_hunk_gun_click", this.gameObject);
            }
            else
            {
                if (this.cachedWeaponDef.roundReload)
                {
                    if (base.isAuthority)
                    {
                        this.outer.SetNextState(new RoundReload
                        {
                            baseDuration = this.baseDuration,
                            interruptPriority = this.interruptPriority
                        });
                    }
                    return;
                }

                if (this.hunk.weaponDef.animationSet == HunkWeaponDef.AnimationSet.Pistol || this.hunk.weaponDef.animationSet == HunkWeaponDef.AnimationSet.PistolAlt)
                {
                    this.animString = "ReloadPistol";
                    base.PlayCrossfade("Gesture, Override", this.animString, "Reload.playbackRate", this.duration, 0.1f);
                    this.success = true;
                    Util.PlaySound("sfx_hunk_pistol_reload_01", this.gameObject);
                }
                else
                {
                    if (this.hunk.weaponDef.nameToken.Contains("GRENADE")) this.animString = "ReloadSingle";
                    base.PlayCrossfade("Gesture, Override", this.animString, "Reload.playbackRate", this.duration, 0.1f);
                    this.success = true;
                    Util.PlaySound("sfx_hunk_smg_reload_01", this.gameObject);
                }
            }
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

            if (base.isAuthority && this.inputBank.skill2.down && this.interruptPriority == InterruptPriority.Any && this.hunk.ammo > 0) // aiming to cancel a passive reload
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= (0.84f * this.duration) && !this.cummed)
            {
                this.cummed = true;
                if (this.success) this.hunk.FinishReload();
            }

            if (base.isAuthority && this.inputBank.skill3.down && this.skillLocator.utility.stock > 0) // roll cancel
            {
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge <= 0.1f) return InterruptPriority.Frozen;
            return this.interruptPriority;
        }
    }
}