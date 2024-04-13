using UnityEngine;
using UnityEngine.UI;

namespace HunkMod.Modules.Components
{
    public class CrosshairStartRotate : MonoBehaviour
    {
        private RectTransform rectTransform;

        private void Awake()
        {
            this.rectTransform = this.GetComponent<RectTransform>();

            if (this.rectTransform)
            {
                this.rectTransform.eulerAngles = new Vector3(0f, 0f, 45f);
            }
        }
    }
}