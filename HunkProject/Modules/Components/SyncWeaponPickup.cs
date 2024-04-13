using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncWeaponPickup : INetMessage
    {
        private NetworkInstanceId netId;
        private ushort weapon;
        private bool cutAmmo;

        public SyncWeaponPickup()
        {
        }

        public SyncWeaponPickup(NetworkInstanceId netId, ushort augh, bool ough)
        {
            this.netId = netId;
            this.weapon = augh;
            this.cutAmmo = ough;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.weapon = reader.ReadUInt16();
            this.cutAmmo = reader.ReadBoolean();
        }

        public void OnReceived()
        {
            GameObject pickupObject = Util.FindNetworkObject(this.netId);
            if (!pickupObject) return;

            WeaponPickup pickupComponent = pickupObject.GetComponentInChildren<WeaponPickup>();
            if (pickupComponent) pickupComponent.SetWeapon(HunkWeaponCatalog.GetWeaponFromIndex(this.weapon), this.cutAmmo);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.weapon);
            writer.Write(this.cutAmmo);
        }
    }
}