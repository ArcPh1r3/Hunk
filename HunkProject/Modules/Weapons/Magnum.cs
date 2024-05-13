using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class Magnum : BaseWeapon<Magnum>
    {
        public override string weaponNameToken => "MAGNUM";
        public override string weaponName => "Lightning Hawk";
        public override string weaponDesc => "7-round capacity .50 AE MAG. Gas-operated action, which is unusual for a semi-auto handgun, gives it both power and accuracy.";
        public override string iconName => "texMagnumIcon";
        public override GameObject crosshairPrefab => Modules.Assets.magnumCrosshairPrefab;
        public override int magSize => 7;
        public override float magPickupMultiplier => 1f;
        public override float reloadDuration => 1.8f;
        public override string ammoName => "MAG Rounds";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlMagnum");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0.8f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.6f;
        public override float reloadFillValue => 0.9f;
        public override float accuracyFillValue => 0.9f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Magnum.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_MAGNUM_NAME",
"ROB_HUNK_BODY_SHOOT_MAGNUM_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public static ItemDef longBarrel;

        public override void Init()
        {
            base.Init();

            this.modelPrefab.AddComponent<Modules.Components.MagnumBehavior>();

            longBarrel = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
            longBarrel.name = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            longBarrel.nameToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            longBarrel.descriptionToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            longBarrel.pickupToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            longBarrel.loreToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            longBarrel.canRemove = false;
            longBarrel.hidden = false;
            longBarrel.pickupIconSprite = weaponDef.icon;
            longBarrel.requiredExpansion = null;
            longBarrel.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            longBarrel.unlockableDef = null;

            HunkWeaponCatalog.itemDefs.Add(longBarrel);

            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME", "Long Barrel (Lightning Hawk)");
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC", "A custom part for the Lightning Hawk, this bull barrel reduces recoil and imparts extra speed to bullets, increasing damage.");
        }
    }
}