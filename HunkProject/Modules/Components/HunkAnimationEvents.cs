using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class HunkAnimationEvents : MonoBehaviour
    {
        private HunkController hunk;

        private void Awake()
        {
            this.hunk = this.GetComponentInParent<HunkController>();
        }

        public void PlaySound(string soundString)
        {
            Util.PlaySound(soundString, this.gameObject);
        }

        public void PumpShotgun()
        {
            Util.PlaySound("sfx_hunk_shotgun_pump", this.gameObject);
            if (this.hunk) this.hunk.DropShell(-this.transform.right * -Random.Range(4, 12));
        }

        public void PumpSlugger()
        {
            Util.PlaySound("sfx_hunk_shotgun_pump", this.gameObject);
            if (this.hunk) this.hunk.DropSlug(-this.transform.right * -Random.Range(4, 12));
        }

        public void HideMag()
        {
            SMGBehavior smg = this.GetComponentInChildren<SMGBehavior>();
            if (smg) smg.HideMag();
        }

        public void ShowMag()
        {
            SMGBehavior smg = this.GetComponentInChildren<SMGBehavior>();
            if (smg) smg.ShowMag();
        }
    }
}