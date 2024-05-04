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

            this.skillLocator.primary.UnsetSkillOverride(this.gameObject, this.hunk.weaponDef.primarySkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.primary.UnsetSkillOverride(this.gameObject, Modules.Survivors.Hunk.reloadSkillDef, GenericSkill.SkillOverridePriority.Network);
            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();

            this.skillLocator.primary.SetSkillOverride(this, Modules.Survivors.Hunk.confirmSkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.secondary.SetSkillOverride(this, Modules.Survivors.Hunk.cancelSkillDef, GenericSkill.SkillOverridePriority.Network);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;

            //this.skillLocator.secondary.stock = 0;
            //this.skillLocator.secondary.rechargeStopwatch = 0f;

            if (this.slowing)
            {
                this.currentTimeScale += Time.unscaledDeltaTime * 1.15f;
                if (this.currentTimeScale >= 1f)
                {
                    this.slowing = false;
                    this.currentTimeScale = 1f;
                }
                Time.timeScale = this.currentTimeScale;
            }

            if (base.isAuthority)
            {
                if (!this.radial.cursorInCenter && this.radial.isValidIndex)
                {
                    if (this.inputBank.skill1.justPressed)
                    {
                        Util.PlaySound("sfx_hunk_menu_click", this.gameObject);
                        EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new Swap
                        {
                            index = this.radial.index
                        }, InterruptPriority.Frozen);
                        return;
                    }

                    if (this.inputBank.skill2.justPressed)
                    {
                        Util.PlaySound("sfx_hunk_menu_click", this.gameObject);
                        this.hunk.weaponTracker.DropWeapon(this.radial.index);
                        return;
                    }
                }
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
                    if (base.fixedAge <= 0.35f)
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

            this.skillLocator.primary.UnsetSkillOverride(this, Modules.Survivors.Hunk.confirmSkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.secondary.UnsetSkillOverride(this, Modules.Survivors.Hunk.cancelSkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

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