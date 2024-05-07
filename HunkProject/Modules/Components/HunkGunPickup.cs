using RoR2;
using UnityEngine;

namespace HunkMod.Modules.Components
{
    public class HunkGunPickup : MonoBehaviour
    {
        public HunkWeaponDef weaponDef;

        private void Start()
        {
            if (!this.weaponDef) this.weaponDef = Modules.Weapons.M19.instance.weaponDef;

            this.gameObject.name = "QuestVolatileBatteryWorldPickup(Clone)";
            this.GetComponent<GenericPickupController>().SetPickupIndexFromString("ItemIndex." + this.weaponDef.itemDef.name);
            this.GetComponent<GenericPickupController>().enabled = false;

            this.Invoke("KillYourself", 1f);
        }

        private void OnEnable()
        {
            this.Init();
        }

        public void Init()
        {
            if (!this.weaponDef) this.weaponDef = Modules.Weapons.M19.instance.weaponDef;

            this.GetComponent<GenericPickupController>().SetPickupIndexFromString("ItemIndex." + this.weaponDef.itemDef.name);
        }

        private void KillYourself()
        {
            if (this.weaponDef != Modules.Weapons.M19.instance.weaponDef) this.transform.Find("PickupDisplay").localScale = Vector3.one * 0.5f;
            else this.transform.Find("PickupDisplay").localScale = Vector3.one;
        }
    }
}