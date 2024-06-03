using UnityEngine;

namespace HunkMod.Modules.Components.UI
{
    public class HunkItemDisplay : MonoBehaviour
    {
        public static HunkItemDisplay instance;

        public float activeTimer;
        public float smoothSpeed = 5f;
        public Vector3 activePosition = new Vector3(0f, -72f, 0f);
        public Vector3 inactivePosition = new Vector3(0f, 350f, 0f);

        private Vector3 desiredPosition;
        private RectTransform rectTransform;

        private void Awake()
        {
            instance = this;

            this.rectTransform = this.GetComponent<RectTransform>();
            this.activeTimer = 8f;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab)) this.activeTimer = 3f;
        }

        private void FixedUpdate()
        {
            this.activeTimer -= Time.fixedDeltaTime;

            if (this.activeTimer > 0f)
            {
                this.desiredPosition = this.activePosition;
            }
            else
            {
                this.desiredPosition = this.inactivePosition;
            }

            if (this.rectTransform)
            {
                this.rectTransform.localPosition = Vector3.Slerp(this.rectTransform.localPosition, this.desiredPosition, this.smoothSpeed * Time.fixedDeltaTime);
            }
        }
    }
}