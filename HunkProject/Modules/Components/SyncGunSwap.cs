using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncGunSwap : INetMessage
    {
        private NetworkInstanceId netId;
        private int index;

        public SyncGunSwap()
        {
        }

        public SyncGunSwap(NetworkInstanceId netId, int _index)
        {
            this.netId = netId;
            this.index = _index;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.index = reader.ReadInt32();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            HunkController hunk = bodyObject.GetComponent<HunkController>();
            if (hunk)
            {
                hunk.ServerSetWeapon(this.index);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.index);
        }
    }
}