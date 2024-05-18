using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class TemplarExplosionTracker : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(this, 0.2f);
        }
    }
}