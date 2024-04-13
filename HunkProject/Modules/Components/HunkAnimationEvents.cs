using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class HunkAnimationEvents : MonoBehaviour
    {
        public void PlaySound(string soundString)
        {
            Util.PlaySound(soundString, this.gameObject);
        }
    }
}