using UnityEngine;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class CrosshairRotator : MonoBehaviour
    {
        public float speed = 100f;

        private RectTransform rectTransform;

        private void Awake()
        {
            this.rectTransform = this.GetComponent<RectTransform>();
        }

        private void FixedUpdate()
        {
            if (this.rectTransform)
            {
                this.rectTransform.eulerAngles = new Vector3(0f, 0f, this.rectTransform.eulerAngles.z + (Time.fixedDeltaTime * this.speed));
            }
        }
    }
}