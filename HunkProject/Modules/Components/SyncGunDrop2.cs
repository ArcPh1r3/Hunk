using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncGunDrop2 : INetMessage
    {
        private NetworkInstanceId netId;
        private int index;

        public SyncGunDrop2()
        {
        }

        public SyncGunDrop2(NetworkInstanceId netId, int _index)
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
                hunk.ClientDropWeapon(this.index);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.index);
        }
    }
}