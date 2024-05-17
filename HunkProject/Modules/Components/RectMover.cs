using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class RectMover : MonoBehaviour
    {
        public Vector3 pos;

        private RectTransform rt;

        private void Awake()
        {
            this.rt = this.GetComponent<RectTransform>();
        }

        private void Update()
        {
            this.rt.transform.localPosition = this.pos;
        }
    }
}