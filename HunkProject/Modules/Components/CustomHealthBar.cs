using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class CustomHealthBar : MonoBehaviour
    {
        public HUD targetHUD;

        private GameObject targetBody;
        private HealthComponent healthComponent;
        private HunkController hunk;
        private Image fill;
        private Image shieldFill;
        private Image barrierFill;
        private Image barrier2Fill;
        private Image gunIcon;

        private float minFill = 0.072f;
        private float maxFill = 0.55f;

        private void Awake()
        {
            this.fill = this.transform.Find("Center/HealthFill").gameObject.GetComponent<Image>();
            this.barrierFill = this.transform.Find("Center/BarrierFill").gameObject.GetComponent<Image>();
            this.barrier2Fill = this.transform.Find("Center/BarrierFill2").gameObject.GetComponent<Image>();
            this.shieldFill = this.transform.Find("Center/ShieldFill").gameObject.GetComponent<Image>();
            this.gunIcon = this.transform.Find("Center/GunIcon").gameObject.GetComponent<Image>();
        }

        private void FixedUpdate()
        {
            if (this.targetHUD)
            {
                if (this.targetBody)
                {
                    if (!this.healthComponent) this.healthComponent = this.targetBody.GetComponent<HealthComponent>();
                    if (!this.hunk) this.hunk = this.targetBody.GetComponent<HunkController>();

                    if (!this.healthComponent) return;
                    if (!this.hunk) return;

                    float combinedHealth = this.healthComponent.fullHealth + this.healthComponent.fullShield;
                    float healthFraction = this.healthComponent.fullHealth / combinedHealth;

                    float _healthFill = Util.Remap(this.healthComponent.health, 0f, combinedHealth, this.minFill, this.maxFill);
                    float _shieldFill = Util.Remap(this.healthComponent.shield, 0f, combinedHealth, this.minFill, this.maxFill);
                    this.fill.fillAmount = _healthFill * healthFraction;
                    this.shieldFill.fillAmount = (_shieldFill * (1f - healthFraction)) + _healthFill;

                    float _barrierFill = Util.Remap(this.healthComponent.barrier, 0f, this.healthComponent.fullBarrier, this.minFill, this.maxFill);
                    this.barrierFill.fillAmount = _barrierFill;
                    this.barrier2Fill.fillAmount = _barrierFill;

                    if (this.hunk)
                    {
                        if (this.hunk.weaponDef)
                        {
                            this.gunIcon.sprite = this.hunk.weaponDef.icon;
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