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

        public SyncAmmoPickup()
        {
        }

        public SyncAmmoPickup(NetworkInstanceId netId)
        {
            this.netId = netId;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            HunkController hunk = bodyObject.GetComponent<HunkController>();
            if (hunk)
            {
                bool valid = false;
                int index = 0;

                while (!valid)
                {
                    index = UnityEngine.Random.Range(0, hunk.weaponTracker.weaponData.Length);
                    if (hunk.weaponTracker.weaponData[index].weaponDef.canPickUpAmmo) valid = true;
                }

                hunk.AddAmmoFromIndex(index);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
        }
    }
}