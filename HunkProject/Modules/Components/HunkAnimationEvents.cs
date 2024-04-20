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
            Util.PlaySound("", this.gameObject);
            if (this.hunk) this.hunk.DropShell(-this.transform.right * -Random.Range(4, 12));
        }
    }
}