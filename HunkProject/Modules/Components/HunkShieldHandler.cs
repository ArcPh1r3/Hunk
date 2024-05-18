using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkShieldHandler : MonoBehaviour
    {
        private float opacity;
        private MeshRenderer meshRenderer;
        private void Awake()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            this.opacity = 0f;
        }

        private void Update()
        {
            if (this.opacity <= 1f)
            {
                this.opacity += Time.fixedDeltaTime;
                this.opacity = Mathf.Clamp01(this.opacity);

                Color c = this.meshRenderer.material.GetColor("_TintColor");
                c.a = this.opacity;
                this.meshRenderer.material.SetColor("_TintColor", c);
            }
        }
    }
}