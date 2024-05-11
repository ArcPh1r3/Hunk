using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncAmmoPickup : INetMessage
    {
        private NetworkInstanceId netId;
        private long multiplier;
        private int index;

        public SyncAmmoPickup()
        {
        }

        public SyncAmmoPickup(NetworkInstanceId netId, float mult, int index)
        {
            this.netId = netId;
            this.multiplier = Mathf.RoundToInt(mult * 100f);
            this.index = index;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.multiplier = reader.ReadInt64();
            this.index = reader.ReadInt32();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            HunkController hunk = bodyObject.GetComponent<HunkController>();
            if (hunk)
            {
                hunk.AddAmmoFromIndex(this.index, this.multiplier * 0.01f);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.multiplier);
            writer.Write(this.index);
        }
    }
}