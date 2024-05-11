using RoR2;
using EntityStates;
using UnityEngine.Networking;

namespace HunkMod.SkillStates.Hunk
{
    public class Swap : BaseHunkSkillState
    {
        public int index;

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

                this.index = this.hunk.weaponTracker.nextWeapon;

                if (this.index != -1) this.hunk.SwapToWeapon(this.index);
                else this.hunk.SwapToLastWeapon();

                base.PlayAnimation("Gesture, Override", "EquipGun", "Swap.playbackRate", this.duration);
                Util.PlaySound("sfx_hunk_equip_smg", this.gameObject);
                this.hunk.ammoKillTimer = 1.5f;
                this.hunk.reloadTimer = 0.33f;
            }

            if (!this.swapped)
            {
                this.skillLocator.secondary.stock = 0;
                this.skillLocator.secondary.rechargeStopwatch = 0f;
            }
            else
            {
                this.skillLocator.secondary.stock = 1;
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

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(this.index);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            this.index = reader.ReadInt32();
            this.GetComponent<Modules.Components.HunkController>().weaponTracker.nextWeapon = this.index;
        }
    }
}