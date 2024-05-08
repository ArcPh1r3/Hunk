using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class DelayedGravity : MonoBehaviour
    {
        private void Awake()
        {
            this.Invoke("Wahoo", 1f);
        }

        private void Wahoo()
        {
            this.GetComponent<Rigidbody>().useGravity = true;
            Destroy(this);
        }
    }
}