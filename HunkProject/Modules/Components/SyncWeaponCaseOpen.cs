using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace HunkMod.Modules.Components
{
    internal class SyncWeaponCaseOpen : INetMessage
    {
        private NetworkInstanceId netId;

        public SyncWeaponCaseOpen()
        {
        }

        public SyncWeaponCaseOpen(NetworkInstanceId netId, GameObject target)
        {
            this.netId = netId;
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            GameObject target = Util.FindNetworkObject(this.netId);
            if (!target) return;

            //target.GetComponent<WeaponChest>().gunPickup.enabled = true;
            //target.GetComponent<WeaponChest>().gunPickup.GetComponent<GenericPickupController>().enabled = true;

            if (target.name.Contains("2")) target.GetComponent<Highlight>().targetRenderer.transform.parent.parent.GetComponent<Animator>().Play("Open");
            else target.GetComponent<Highlight>().targetRenderer.transform.parent.parent.parent.GetComponent<Animator>().Play("Open");

            GameObject.Destroy(target.GetComponent<WeaponChest>().gunPickup.gameObject);

            Util.PlaySound("sfx_hunk_weapon_case_open", target.gameObject);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
        }
    }
}