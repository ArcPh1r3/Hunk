using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncWeapon : INetMessage
    {
        private NetworkInstanceId netId;
        private ushort weapon;
        private bool cutAmmo;

        public SyncWeapon()
        {
        }

        public SyncWeapon(NetworkInstanceId netId, ushort augh, bool ough, bool isAmmoBox)
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
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            HunkController iDrive = bodyObject.GetComponent<HunkController>();
            HunkWeaponDef weaponDef = HunkWeaponCatalog.GetWeaponFromIndex(this.weapon);

            float ammo = -1f;
            if (this.cutAmmo) ammo = weaponDef.magSize * 0.5f;

            if (iDrive) iDrive.PickUpWeapon(weaponDef);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.weapon);
            writer.Write(this.cutAmmo);
        }
    }
}