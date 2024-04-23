using EntityStates;
using UnityEngine;
using RoR2;
using RoR2.UI;
using HunkMod.Modules.Components;

namespace HunkMod.SkillStates.Hunk
{
    public class SwapWeapon : BaseHunkSkillState
    {
        private WeaponRadial radial;
        private bool slowing;
        private float currentTimeScale = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.ammoKillTimer = -1f;

            if (base.isAuthority)
            {
                this.radial = GameObject.Instantiate(Modules.Assets.weaponRadial).GetComponent<WeaponRadial>();
                this.radial.hunk = this.hunk.weaponTracker;

                foreach (HUD i in HUD.readOnlyInstanceList)
                {
                    if (i.targetBodyObject && i.targetBodyObject == this.gameObject)
                    {
                        this.radial.transform.parent = i.mainContainer.transform;
                        this.radial.transform.localPosition = Vector3.zero;
                        this.radial.transform.localScale = Vector3.zero;
                    }
                }
            }

            if (RoR2Application.isInSinglePlayer)
            {
                this.slowing = true;
                this.currentTimeScale = 0.1f;
            }

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;

            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            if (this.slowing)
            {
                this.currentTimeScale += Time.unscaledDeltaTime * 1.25f;
                if (this.currentTimeScale >= 1f)
                {
                    this.slowing = false;
                    this.currentTimeScale = 1f;
                }
                Time.timeScale = this.currentTimeScale;
            }

            if (base.isAuthority && !this.inputBank.skill4.down)
            {
                if (!this.radial.cursorInCenter)
                {
                    if (this.radial.isValidIndex)
                    {
                        EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new Swap
                        {
                            index = this.radial.index
                        }, InterruptPriority.Frozen);
                    }
                }
                else
                {
                    if (base.fixedAge <= 0.5f)
                    {
                        EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new Swap
                        {
                            index = -1
                        }, InterruptPriority.Frozen);
                    }
                }

                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.radial) Destroy(this.radial.gameObject);

            if (RoR2Application.isInSinglePlayer)
            {
                Time.timeScale = 1f;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}