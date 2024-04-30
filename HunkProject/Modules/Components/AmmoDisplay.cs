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
        private float alpha;

        private int totalAmmo
        {
            get
            {
                return this.hunk.weaponTracker.weaponData[this.hunk.weaponTracker.equippedIndex].totalAmmo;
            }
        }

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

                    if (this.hunk.maxAmmo >= 999f)
                    {
                        this.targetText.token = "";
                        return;
                    }

                    if (this.hunk.ammo <= 0f)
                    {
                        if (this.totalAmmo > 0)
                        {
                            this.targetText.token = "<color=#C80000>0 / " + this.totalAmmo + Helpers.colorSuffix;
                        }
                        else
                        {
                            this.targetText.token = "<color=#C80000>0 / 0" + Helpers.colorSuffix;
                        }
                    }
                    else
                    {
                        if (this.hunk.ammo > this.hunk.weaponTracker.weaponData[hunk.weaponTracker.equippedIndex].currentAmmo)
                        {
                            this.targetText.token = "<color=#00FF66>" + Mathf.CeilToInt(this.hunk.ammo).ToString() + Helpers.colorSuffix + " / " + Mathf.CeilToInt(this.totalAmmo).ToString();
                        }
                        else
                        {
                            this.targetText.token = Mathf.CeilToInt(this.hunk.ammo).ToString() + " / " + Mathf.CeilToInt(this.totalAmmo).ToString();
                        }
                    }

                    Color col = this.targetText.textMeshPro.color;

                    if (this.hunk.ammoKillTimer <= 0f) this.alpha -= 4f * Time.fixedDeltaTime;
                    else this.alpha += 4f * Time.fixedDeltaTime;

                    this.alpha = Mathf.Clamp01(this.alpha);
                    
                    col.a = this.alpha;

                    this.targetText.textMeshPro.color = col;
                }
                else
                {
                    this.targetText.token = "";
                }
            }
        }
    }
}