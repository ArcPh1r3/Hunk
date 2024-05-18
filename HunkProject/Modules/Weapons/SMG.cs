using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class SMG : BaseWeapon<SMG>
    {
        public override string weaponNameToken => "SMG";
        public override string weaponName => "LE 5";
        public override string weaponDesc => "32-shot capacity .380 ACP machine gun. Uses a closed bolt and has high accuracy.";
        public override string iconName => "texSMGIcon";
        public override GameObject crosshairPrefab => Modules.Assets.smgCrosshairPrefab;
        public override int magSize => 32;
        public override float magPickupMultiplier => 1f;
        public override float reloadDuration => 2.4f;
        public override string ammoName => "SMG Ammo";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlSMG");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;
        public override bool storedOnBack => true;
        public override float damageFillValue => 0.5f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.9f;
        public override float reloadFillValue => 0.5f;
        public override float accuracyFillValue => 0.5f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.SMG.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_SMG_NAME",
"ROB_HUNK_BODY_SHOOT_SMG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public static ItemDef laserSight;
        public static ItemDef extendedMag;

        public override void Init()
        {
            base.Init();

            this.modelPrefab.AddComponent<Modules.Components.SMGBehavior>();

            laserSight = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
            laserSight.name = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            laserSight.nameToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            laserSight.descriptionToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            laserSight.pickupToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            laserSight.loreToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            laserSight.canRemove = false;
            laserSight.hidden = false;
            laserSight.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLaserSightIcon");
            laserSight.requiredExpansion = null;
            laserSight.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            laserSight.unlockableDef = null;

            laserSight.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlLaserSight");
            Modules.Assets.ConvertAllRenderersToHopooShader(laserSight.pickupModelPrefab);

            HunkWeaponCatalog.itemDefs.Add(laserSight);

            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME", "Laser Sight (LE 5)");
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC", "A custom part for the LE 5 that allows for lightning-fast aiming.");

            extendedMag = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
            extendedMag.name = "ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_NAME";
            extendedMag.nameToken = "ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_NAME";
            extendedMag.descriptionToken = "ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_DESC";
            extendedMag.pickupToken = "ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_DESC";
            extendedMag.loreToken = "ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_DESC";
            extendedMag.canRemove = false;
            extendedMag.hidden = false;
            extendedMag.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLaserSightIcon");
            extendedMag.requiredExpansion = null;
            extendedMag.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            extendedMag.unlockableDef = null;

            extendedMag.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlLaserSight");
            Modules.Assets.ConvertAllRenderersToHopooShader(extendedMag.pickupModelPrefab);

            HunkWeaponCatalog.itemDefs.Add(extendedMag);

            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_NAME", "Extended Mazine (LE 5)");
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_DESC", "A custom part for the LE 5 that greatly boosts its magazine capacity.");
        }
    }
}