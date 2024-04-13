using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncStoredWeapon : INetMessage
    {
        private NetworkInstanceId netId;
        private ushort weapon;
        private long ammo;

        public SyncStoredWeapon()
        {
        }

        public SyncStoredWeapon(NetworkInstanceId netId, ushort augh, float ammo)
        {
            this.netId = netId;
            this.weapon = augh;
            this.ammo = Mathf.CeilToInt(ammo * 100f);
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.weapon = reader.ReadUInt16();
            this.ammo = reader.ReadInt64();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            HunkController iDrive = bodyObject.GetComponent<HunkController>();
            if (iDrive)
            {
                iDrive.PickUpWeapon(HunkWeaponCatalog.GetWeaponFromIndex(this.weapon));
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.weapon);
            writer.Write(this.ammo);
        }
    }
}