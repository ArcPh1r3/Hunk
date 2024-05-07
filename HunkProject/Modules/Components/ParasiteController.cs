using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    public class ParasiteController : MonoBehaviour
    {
        private CharacterBody characterBody;

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            this.Invoke("Huh", 1f);
        }

        private void Huh()
        {
            if (this.characterBody.inventory) this.characterBody.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
        }

        /*private void FixedUpdate()
        {
            if (this.characterBody)
            {
                this.characterBody.teamComponent.teamIndex = TeamIndex.Void;

                if (this.characterBody.master)
                {
                    this.characterBody.master.teamIndex = TeamIndex.Void;
                }
            }
        }*/
        private void OnEnable()
        {
            Modules.Survivors.Hunk.virusObjectiveObjects.Add(this.gameObject);
        }

        private void OnDisable()
        {
            Modules.Survivors.Hunk.virusObjectiveObjects.Remove(this.gameObject);
        }
    }
}