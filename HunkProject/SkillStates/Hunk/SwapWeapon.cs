using EntityStates;
using UnityEngine;
using RoR2;

namespace HunkMod.SkillStates.Hunk
{
    public class SwapWeapon : BaseHunkSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.ammoKillTimer = -1f;

            EntityStateMachine.FindByCustomName(this.gameObject, "Aim").SetNextStateToMain();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hunk.reloadTimer = 2f;

            this.skillLocator.secondary.stock = 0;
            this.skillLocator.secondary.rechargeStopwatch = 0f;

            if (base.isAuthority && !this.inputBank.skill4.down)
            {
                EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new Swap(), InterruptPriority.Frozen);
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}