using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkHeadshotTracker : MonoBehaviour
    {
        private void Awake()
        {
            Destroy(this, 0.1f);
        }
    }
}