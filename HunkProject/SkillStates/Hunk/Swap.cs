using RoR2;
using EntityStates;

namespace HunkMod.SkillStates.Hunk
{
    public class Swap : BaseHunkSkillState
    {
        private float duration = 0.5f;
        private bool swapped;

        public override void OnEnter()
        {
            base.OnEnter();
            this.hunk.reloadTimer = 1f;

            base.PlayCrossfade("Gesture, Override", "StoreGun", "Swap.playbackRate", this.duration * 0.5f, 0.1f);

            Util.PlaySound("sfx_hunk_store_gun", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= (0.5f * this.duration) && !this.swapped)
            {
                this.swapped = true;
                this.hunk.SwapToLastWeapon();
                base.PlayAnimation("Gesture, Override", "EquipGun", "Swap.playbackRate", this.duration);
                Util.PlaySound("sfx_hunk_equip_smg", this.gameObject);
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.swapped) return InterruptPriority.Any;
            return InterruptPriority.Death;
        }
    }
}