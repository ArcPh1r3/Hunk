using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncCombatStopwatch : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncCombatStopwatch()
        {
        }

        public SyncCombatStopwatch(NetworkInstanceId netId, GameObject target)
        {
            this.netId = netId;
            this.target = target;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.target = reader.ReadGameObject();
        }

        public void OnReceived()
        {
            if (!this.target) return;

            this.target.GetComponent<CharacterBody>().outOfCombatStopwatch = 0f;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}