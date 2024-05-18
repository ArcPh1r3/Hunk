using RoR2;
using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkGunPickup : MonoBehaviour
    {
        public ItemDef itemDef;

        private void Start()
        {
            if (!this.itemDef) this.itemDef = Modules.Weapons.M19.instance.itemDef;

            this.gameObject.name = "QuestVolatileBatteryWorldPickup(Clone)";
            this.GetComponent<GenericPickupController>().SetPickupIndexFromString("ItemIndex." + this.itemDef.name);
            this.GetComponent<GenericPickupController>().enabled = false;

            this.Invoke("KillYourself", 1f);
        }

        private void OnEnable()
        {
            this.Init();
        }

        public void Init()
        {
            if (!this.itemDef) this.itemDef = Modules.Weapons.M19.instance.itemDef;

            this.GetComponent<GenericPickupController>().SetPickupIndexFromString("ItemIndex." + this.itemDef.name);
            this.GetComponent<GenericPickupController>().pickupIndex = PickupCatalog.FindPickupIndex("ItemIndex." + this.itemDef.name);
            this.GetComponent<GenericPickupController>().NetworkpickupIndex = PickupCatalog.FindPickupIndex("ItemIndex." + this.itemDef.name);
            this.GetComponentInChildren<PickupDisplay>().SetPickupIndex(PickupCatalog.FindPickupIndex("ItemIndex." + this.itemDef.name));
        }

        private void KillYourself()
        {
            if (this.itemDef != Modules.Weapons.M19.instance.itemDef) this.transform.Find("PickupDisplay").localScale = Vector3.one * 0.5f;
            else this.transform.Find("PickupDisplay").localScale = Vector3.one;
        }
    }
}