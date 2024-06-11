using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncCVirus : INetMessage
    {
        private NetworkInstanceId netId;
        private GameObject target;

        public SyncCVirus()
        {
        }

        public SyncCVirus(NetworkInstanceId netId, GameObject target)
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

            this.target.AddComponent<CVirusHandler>();

            GameObject positionIndicator = GameObject.Instantiate(Modules.Assets.cVirusPositionIndicator);
            positionIndicator.transform.parent = this.target.transform;
            positionIndicator.transform.localPosition = Vector3.zero;

            CharacterBody body = this.target.GetComponent<CharacterBody>();
            if (body)
            {
                if (body.mainHurtBox) positionIndicator.GetComponent<PositionIndicator>().targetTransform = body.mainHurtBox.transform;
            }
            else positionIndicator.GetComponent<PositionIndicator>().targetTransform = this.target.transform;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.target);
        }
    }
}