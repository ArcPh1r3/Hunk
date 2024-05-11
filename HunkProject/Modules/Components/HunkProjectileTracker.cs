using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkProjectileTracker : MonoBehaviour
    {
        private void OnEnable()
        {
            MainPlugin.projectileList.Add(this);
        }

        private void OnDisable()
        {
            MainPlugin.projectileList.Add(this);
        }
    }
}