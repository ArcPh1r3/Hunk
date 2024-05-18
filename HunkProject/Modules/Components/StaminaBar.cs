using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class StaminaBar : MonoBehaviour
    {
        public HUD targetHUD;

        private GameObject targetBody;
        private SkillLocator skillLocator;
        private Image fill;
        private Image fullBar;
        private GameObject[] barElements;
        private float opacity;
        private float stopwatch;

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
                    if (!this.skillLocator) this.skillLocator = this.targetBody.GetComponent<SkillLocator>();

                    if (this.skillLocator.utility.stock > 0)
                    {
                        this.stopwatch = 0f;
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
                        this.stopwatch += Time.fixedDeltaTime;

                        if (this.stopwatch >= 0.25f)
                        {
                            this.opacity = 1f;
                            foreach (GameObject i in this.barElements)
                            {
                                i.SetActive(true);
                            }

                            float fillAmount = Util.Remap(this.skillLocator.utility.rechargeStopwatch, 0f, this.skillLocator.utility.finalRechargeInterval, 0f, 1f);
                            this.fill.fillAmount = fillAmount;
                        }
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