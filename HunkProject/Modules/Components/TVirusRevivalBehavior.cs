using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace HunkMod.Modules.Components
{
    public class TVirusRevivalBehavior : MonoBehaviour
    {
        public CharacterMaster master;

        private void Awake()
        {
            if (!NetworkServer.active) return;

            this.Invoke("Respawn", 1f);
        }

        public void Respawn()
        {
            this.master.RespawnExtraLife();
            this.master.inventory.RemoveItem(Modules.Survivors.Hunk.tVirusRevival);

            Destroy(this);
        }
    }
}