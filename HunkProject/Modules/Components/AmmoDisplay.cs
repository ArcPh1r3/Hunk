using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class AmmoDisplay : MonoBehaviour
    {
        public HUD targetHUD;
        public LanguageTextMeshController targetText;

        private HunkController hunk;

        private void FixedUpdate()
        {
            if (this.targetHUD)
            {
                if (this.targetHUD.targetBodyObject)
                {
                    if (!this.hunk) this.hunk = this.targetHUD.targetBodyObject.GetComponent<HunkController>();
                }
            }

            if (this.targetText)
            {
                if (this.hunk)
                {
                    if (this.hunk.maxAmmo <= 0f)
                    {
                        this.targetText.token = "";
                        return;
                    }

                    if (this.hunk.ammo <= 0f)
                    {
                        this.targetText.token = "<color=#C80000>0 / " + Mathf.CeilToInt(this.hunk.maxAmmo).ToString() + Helpers.colorSuffix;
                    }
                    else
                    {
                        this.targetText.token = Mathf.CeilToInt(this.hunk.ammo).ToString() + " / " + Mathf.CeilToInt(this.hunk.maxAmmo).ToString();
                    }
                }
                else
                {
                    this.targetText.token = "";
                }
            }
        }
    }
}