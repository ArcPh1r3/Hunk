using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class FuckYouLivelyPot : MonoBehaviour
    {
        private CharacterBody characterBody;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
        }

        private void FixedUpdate()
        {
            this.characterBody.outOfCombatStopwatch = 0f;
        }
    }
}