using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

namespace HunkMod.Modules.Components
{
    public class AmmoDisplay2 : MonoBehaviour
    {
        public HUD targetHUD;
        public TextMeshProUGUI currentText;
        public TextMeshProUGUI totalText;

        private Image divider;
        private Image mainPanel;
        private HunkController hunk;
        private float alpha;

        private int totalAmmo
        {
            get
            {
                return this.hunk.weaponTracker.weaponData[this.hunk.weaponTracker.equippedIndex].totalAmmo;
            }
        }

        private void Start()
        {
            this.mainPanel = this.GetComponent<Image>();
            this.divider = this.transform.Find("Divider").gameObject.GetComponent<Image>();
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

            if (this.currentText && this.totalText)
            {
                if (this.hunk)
                {
                    if (this.hunk.maxAmmo <= 0f)
                    {
                        this.totalText.text = "";
                        this.currentText.text = "";
                        this.mainPanel.enabled = false;
                        this.divider.enabled = false;
                        return;
                    }

                    if (this.hunk.maxAmmo >= 999f)
                    {
                        this.totalText.text = "";
                        this.currentText.text = "";
                        this.mainPanel.enabled = false;
                        this.divider.enabled = false;
                        return;
                    }

                    this.mainPanel.enabled = true;
                    this.divider.enabled = true;

                    if (this.hunk.ammo <= 0f)
                    {
                        this.currentText.text = "<color=#C80000>0" + Helpers.colorSuffix;
                    }
                    else
                    {
                        if (this.hunk.ammo > this.hunk.weaponTracker.weaponData[hunk.weaponTracker.equippedIndex].currentAmmo)
                        {
                            this.currentText.text = "<color=#00FF66>" + Mathf.CeilToInt(this.hunk.ammo).ToString() + Helpers.colorSuffix;
                        }
                        else
                        {
                            this.currentText.text = Mathf.CeilToInt(this.hunk.ammo).ToString();
                        }
                    }

                    if (this.totalAmmo > 0)
                    {
                        this.totalText.text = this.totalAmmo.ToString();
                    }
                    else
                    {
                        this.totalText.text = "<color=#C80000>0" + Helpers.colorSuffix;
                    }

                    Color col = this.currentText.color;
                    Color col2 = this.totalText.color;
                    Color col3 = this.mainPanel.color;
                    Color col4 = this.divider.color;

                    if (this.hunk.ammoKillTimer <= 0f) this.alpha -= 4f * Time.fixedDeltaTime;
                    else this.alpha += 4f * Time.fixedDeltaTime;

                    this.alpha = Mathf.Clamp01(this.alpha);

                    col.a = this.alpha;
                    col2.a = this.alpha;
                    col3.a = Util.Remap(this.alpha, 0f, 1f, 0f, 0.8f);
                    col4.a = Util.Remap(this.alpha, 0f, 1f, 0f, 0.5f);

                    this.currentText.color = col;
                    this.totalText.color = col2;
                    this.mainPanel.color = col3;
                    this.divider.color = col4;
                }
            }
        }
    }
}