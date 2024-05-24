using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class MUP : BaseWeapon<MUP>
    {
        public override string weaponNameToken => "MUP";
        public override string weaponName => "MUP";
        public override string weaponDesc => "16-shot capacity 9mm handgun. Used by professionals in many state organizations. Reliable with high accuracy.";
        public override string iconName => "texMUPIcon";
        public override GameObject crosshairPrefab => Modules.Assets.pistolCrosshairPrefab;
        public override int magSize => 16;
        public override float magPickupMultiplier => 2f;
        public override int startingMags => 1;
        public override float reloadDuration => 1.2f;
        public override string ammoName => "9mm Ammo";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlMUP");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0.3f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.75f;
        public override float reloadFillValue => 1f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.MUP.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_MUP_NAME",
"ROB_HUNK_BODY_SHOOT_MUP_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public static ItemDef gunStock;

        public override void Init()
        {
            base.Init();

            this.modelPrefab.AddComponent<Modules.Components.MUPBehavior>();

            gunStock = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
            gunStock.name = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            gunStock.nameToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            gunStock.descriptionToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            gunStock.pickupToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            gunStock.loreToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            gunStock.canRemove = false;
            gunStock.hidden = false;
            gunStock.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGunStockIcon");
            gunStock.requiredExpansion = null;
            gunStock.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            gunStock.unlockableDef = null;

            gunStock.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlGunStock");
            Modules.Assets.ConvertAllRenderersToHopooShader(gunStock.pickupModelPrefab);

            HunkWeaponCatalog.itemDefs.Add(gunStock);

            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME", "Gun Stock (MUP)");
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC", "A custom part for the MUP that allows the gun to fire 3 rounds per pull of the trigger.");
        }
    }
}