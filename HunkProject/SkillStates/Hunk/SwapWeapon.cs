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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && !this.inputBank.skill4.down)
            {
                this.hunk.SwapToLastWeapon();
                base.PlayAnimation("Reload", "BufferEmpty");
                this.outer.SetNextStateToMain();
                return;
            }
        }
    }
}