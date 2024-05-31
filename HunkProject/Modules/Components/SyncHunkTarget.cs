using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncHunkTarget : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncHunkTarget()
        {
        }

        public SyncHunkTarget(NetworkInstanceId netId, GameObject target)
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

            HunkController hunk = bodyObject.GetComponent<HunkController>();
            if (hunk)
            {
                hunk.speedLineTimer = 1f;
                hunk.TriggerDodge();
                hunk.TargetObject(this.target.transform);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}