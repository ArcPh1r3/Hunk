using UnityEngine;
using RoR2;
using UnityEngine.Networking;

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
            if (NetworkServer.active && this.characterBody && this.characterBody.inventory) this.characterBody.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
        }

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