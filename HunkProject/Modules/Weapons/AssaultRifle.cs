using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class AssaultRifle : BaseWeapon<AssaultRifle>
    {
        public override string weaponNameToken => "AR";
        public override string weaponName => "CQBR Assault Rifle";
        public override string weaponDesc => "A 5.56x45mm assault rifle optimized by U.B.C.S. for this operation. Its short length affords great mobility, even in urban settings.";
        public override string iconName => "texAssaultRifleIcon";
        public override GameObject crosshairPrefab => Modules.Assets.smgCrosshairPrefab;
        public override int magSize => 64;
        public override float magPickupMultiplier => 1.5f;
        public override int startingMags => 2;
        public override float reloadDuration => 2.4f;
        public override string ammoName => "5.56mm Ammo";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlAssaultRifle");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;
        public override bool storedOnBack => true;
        public override float damageFillValue => 0.6f;
        public override float rangefillValue => 0.9f;
        public override float fireRateFillValue => 0.75f;
        public override float reloadFillValue => 0.5f;
        public override float accuracyFillValue => 0.8f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.AssaultRifle.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_AR_NAME",
"ROB_HUNK_BODY_SHOOT_AR_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        //public static ItemDef laserSight;
        //public static ItemDef extendedMag;

        public override void Init()
        {
            base.Init();

            this.modelPrefab.AddComponent<Modules.Components.ARBehavior>();

            /*laserSight = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
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
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC", "A custom part for the LE 5 that boosts its effectiveness at long range and when striking weak points.");

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

            extendedMag.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlExtendedMag");
            Modules.Assets.ConvertAllRenderersToHopooShader(extendedMag.pickupModelPrefab);

            HunkWeaponCatalog.itemDefs.Add(extendedMag);

            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_NAME", "Extended Magazine (LE 5)");
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON2_" + weaponNameToken + "_DESC", "A custom part for the LE 5 that greatly boosts its magazine capacity.");*/
        }
    }
}