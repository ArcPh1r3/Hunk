using UnityEngine;
using TMPro;
using RoR2.UI;

namespace HunkMod.Modules.Components
{
    public class HunkNotificationHandler : MonoBehaviour
    {
        public HUD targetHUD;
        public HunkController hunk;

        private Color baseColor;
        private TextMeshProUGUI text;
        private float fadeSpeed = 8f;
        private float opacity;
        private SubState subState = SubState.Idle;
        private float stopwatch;

        private enum SubState
        {
            Starting,
            Idle,
            Ending
        }

        private void Awake()
        {
            this.text = this.GetComponentInChildren<TextMeshProUGUI>();

            this.opacity = 0f;

            Color c = this.baseColor;
            c.a = this.opacity;
            this.text.color = c;
        }

        private void Start()
        {
            if (!MainPlugin.riskUIInstalled) this.text.font = Modules.Assets.hgFont;
        }

        private void FixedUpdate()
        {
            if (!this.hunk)
            {
                if (this.targetHUD)
                {
                    if (this.targetHUD.targetBodyObject)
                    {
                        this.hunk = this.targetHUD.targetBodyObject.GetComponent<HunkController>();
                    }
                }

                return;
            }

            this.hunk.notificationHandler = this;

            switch (this.subState)
            {
                case SubState.Starting:
                    this.opacity += Time.fixedDeltaTime * this.fadeSpeed;
                    this.opacity = Mathf.Clamp01(this.opacity);
                    if (this.opacity >= 1f) this.subState = SubState.Idle;
                    break;
                case SubState.Idle:
                    this.stopwatch -= Time.fixedDeltaTime;
                    if (this.stopwatch <= 0f) this.subState = SubState.Ending;
                    break;
                case SubState.Ending:
                    this.opacity -= Time.fixedDeltaTime * this.fadeSpeed;
                    this.opacity = Mathf.Clamp01(this.opacity);
                    break;
            }

            Color c = this.baseColor;
            c.a = this.opacity;
            this.text.color = c;
        }

        public void Init(string newText, Color color, float duration = 3f)
        {
            this.text.text = newText;
            this.opacity = 0f;
            this.baseColor = color;
            this.stopwatch = duration;
            this.subState = SubState.Starting;
        }

        public void SoftInit(string newText, Color color, float duration = 3f)
        {
            if (this.opacity > 0f) return;

            this.text.text = newText;
            this.opacity = 0f;
            this.baseColor = color;
            this.stopwatch = duration;
            this.subState = SubState.Starting;
        }
    }
}