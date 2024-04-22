using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncDecapitation : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncDecapitation()
        {
        }

        public SyncDecapitation(NetworkInstanceId netId, GameObject target)
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
            if (!bodyObject) return;

            ModelLocator modelLocator = bodyObject.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform) modelLocator.modelTransform.gameObject.AddComponent<Decapitation>();
            else bodyObject.AddComponent<Decapitation>();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}