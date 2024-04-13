using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class HunkCSS : MonoBehaviour
    {
        private ChildLocator childLocator;

        private void Awake()
        {
            this.childLocator = this.GetComponent<ChildLocator>();
        }

        public void ThrowGun()
        {
            Util.PlaySound("sfx_driver_gun_throw", this.gameObject);
        }

        public void CatchGun()
        {
            Util.PlaySound("sfx_driver_gun_catch", this.gameObject);
        }

        public void FailCatchGun()
        {

        }

        public void GunDrop()
        {
            Util.PlaySound("sfx_driver_gun_drop", this.gameObject);
        }
    }
}