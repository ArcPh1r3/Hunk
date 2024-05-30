using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncTVirusOverlay : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncTVirusOverlay()
        {
        }

        public SyncTVirusOverlay(NetworkInstanceId netId, GameObject target)
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
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject)
            {
                Chat.AddMessage("Fuck");
                return;
            }

            TVirusHandler tVirus = bodyObject.GetComponentInChildren<TVirusHandler>();
            if (tVirus) tVirus.TriggerRevive();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}