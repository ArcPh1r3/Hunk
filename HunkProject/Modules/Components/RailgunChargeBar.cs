using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class RailgunChargeBar : MonoBehaviour
    {
        public HUD targetHUD;

        private GameObject targetBody;
        private HunkController hunk;
        private Image fill;
        private Image fullBar;
        private GameObject[] barElements;
        private float opacity;

        private void Awake()
        {
            this.fill = this.transform.Find("Fill").gameObject.GetComponent<Image>();
            this.fullBar = this.transform.Find("FullFill").gameObject.GetComponent<Image>();

            this.barElements = new GameObject[]
            {
                this.transform.Find("Background").gameObject,
                this.transform.Find("Border").gameObject,
                this.transform.Find("Border (1)").gameObject,
                this.transform.Find("Fill").gameObject
            };
        }

        private void FixedUpdate()
        {
            if (this.targetHUD)
            {
                if (this.targetBody)
                {
                    if (!this.hunk) this.hunk = this.targetBody.GetComponent<HunkController>();
                    if (!this.hunk) return;

                    if (this.hunk.railgunCharge <= 0f)
                    {
                        this.fill.fillAmount = 1f;

                        foreach (GameObject i in this.barElements)
                        {
                            i.SetActive(false);
                        }

                        this.opacity -= Time.fixedDeltaTime * 2f;
                        Color col = this.fullBar.color;
                        col.a = this.opacity;
                        this.fullBar.color = col;
                        this.fullBar.gameObject.SetActive(true);
                    }
                    else
                    {
                        this.opacity = 1f;
                        foreach (GameObject i in this.barElements)
                        {
                            i.SetActive(true);
                        }

                        float fillAmount = Util.Remap(this.hunk.railgunCharge, 0f, this.hunk.railgunMaxCharge, 0f, 1f);
                        this.fill.fillAmount = fillAmount;
                    }
                }
                else
                {
                    if (this.targetHUD.targetBodyObject) this.targetBody = this.targetHUD.targetBodyObject;
                }
            }
        }
    }
}