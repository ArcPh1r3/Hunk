using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;

namespace HunkMod.Modules.Components.UI
{
    public class CustomHealthBar : MonoBehaviour
    {
        public HUD targetHUD;

        public Gradient healthBarGradient = new Gradient
        {
            alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey
                {
                    alpha = 1f,
                    time = 0f
                },
                new GradientAlphaKey
                {
                    alpha = 1f,
                    time = 1f
                }
            },
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey
                {
                    color = Color.red,
                    time = 0f
                },
                new GradientColorKey
                {
                    color = Color.yellow,
                    time = 0.5f
                },
                new GradientColorKey
                {
                    color = new Color(93f / 255f, 171f / 255f, 115f / 255f),
                    time = 0.6f
                },
                new GradientColorKey
                {
                    color = new Color(93f / 255f, 171f / 255f, 115f / 255f),
                    time = 1f
                }
            }
        };

        public float fillSpeed = 8f;
        private float activeTimer;

        private GameObject targetBody;
        private HealthComponent healthComponent;
        private HunkController hunk;
        private Image fill;
        private Image shieldFill;
        private Image barrierFill;
        private Image barrier2Fill;
        private Image gunIcon;

        public float smoothSpeed = 5f;
        public Vector3 activePosition = new Vector3(-100f, 30f, 0f);
        public Vector3 inactivePosition = new Vector3(-100f, -300f, 0f);

        private Vector3 desiredPosition;
        private RectTransform rectTransform;

        public float minFill = 0.072f;
        public float maxFill = 0.525f;

        private void Awake()
        {
            this.fill = this.transform.Find("Center/HealthFill").gameObject.GetComponent<Image>();
            this.barrierFill = this.transform.Find("Center/BarrierFill").gameObject.GetComponent<Image>();
            this.barrier2Fill = this.transform.Find("Center/BarrierFill2").gameObject.GetComponent<Image>();
            this.shieldFill = this.transform.Find("Center/ShieldFill").gameObject.GetComponent<Image>();
            this.gunIcon = this.transform.Find("Center/GunIcon").gameObject.GetComponent<Image>();

            this.rectTransform = this.GetComponent<RectTransform>();
            this.activeTimer = 8f;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab)) this.activeTimer = 2f;
        }

        private void FixedUpdate()
        {
            if (this.targetHUD)
            {
                this.activeTimer -= Time.fixedDeltaTime;

                if (this.activeTimer > 0f) this.desiredPosition = this.activePosition;
                else this.desiredPosition = this.inactivePosition;

                if (this.rectTransform) this.rectTransform.localPosition = Vector3.Lerp(this.rectTransform.localPosition, this.desiredPosition, this.smoothSpeed * Time.fixedDeltaTime);

                if (this.targetBody)
                {
                    if (!this.healthComponent) this.healthComponent = this.targetBody.GetComponent<HealthComponent>();
                    if (!this.hunk) this.hunk = this.targetBody.GetComponent<HunkController>();

                    if (!this.healthComponent) return;
                    if (!this.hunk) return;

                    if (!this.healthComponent.alive)
                    {
                        this.fill.fillAmount = 0f;
                        this.shieldFill.fillAmount = 0f;
                        this.barrierFill.fillAmount = 0f;
                        this.barrier2Fill.fillAmount = 0f;
                        this.targetBody = null;
                        return;
                    }

                    float curse = this.healthComponent.body.cursePenalty;

                    float combinedHealth = this.healthComponent.fullCombinedHealth;
                    float healthFraction = this.healthComponent.health / this.healthComponent.fullHealth;
                    float shieldFraction = 0f;
                    if (this.healthComponent.fullShield > 0f) shieldFraction = this.healthComponent.shield / this.healthComponent.fullShield;

                    float healthSplit = this.healthComponent.fullHealth / combinedHealth;
                    float shieldSplit = 1f - healthSplit;

                    Color healthColor = this.healthBarGradient.Evaluate(healthFraction);
                    this.fill.color = healthColor;
                    this.fill.fillAmount = Mathf.Lerp(this.fill.fillAmount, Util.Remap((healthSplit * healthFraction) / curse, 0f, 1f, this.minFill, this.maxFill), this.fillSpeed * Time.fixedDeltaTime);

                    this.shieldFill.fillAmount = Mathf.Lerp(this.shieldFill.fillAmount, Util.Remap(((healthSplit * healthFraction) + (shieldSplit * shieldFraction)) / curse, 0f, 1f, this.minFill, this.maxFill), this.fillSpeed * Time.fixedDeltaTime) / curse;

                    float _barrierFill = Util.Remap(this.healthComponent.barrier, 0f, this.healthComponent.fullBarrier, this.minFill, this.maxFill);

                    this.barrierFill.fillAmount = _barrierFill;
                    this.barrier2Fill.fillAmount = _barrierFill;

                    if (this.healthComponent.combinedHealth == combinedHealth)
                    {
                    }
                    else this.activeTimer = 2f;

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
                    else this.activeTimer = 0f;
                }
            }
        }
    }
}