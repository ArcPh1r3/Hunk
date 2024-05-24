using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class Revolver : BaseWeapon<Revolver>
    {
        public override string weaponNameToken => "REVOLVER";
        public override string weaponName => "Quickdraw Army";
        public override string weaponDesc => "6-shot capacity .45 ACP, single action handgun. Various customizations of this gun exist, each with their own name.";
        public override string iconName => "texRevolverIcon";
        public override GameObject crosshairPrefab => Modules.Assets.magnumCrosshairPrefab;
        public override int magSize => 6;
        public override float magPickupMultiplier => 1f;
        public override int startingMags => 1;
        public override float reloadDuration => 0.9f;
        public override string ammoName => ".45 ACP Rounds";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlRevolver");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 0.8f;
        public override float fireRateFillValue => 0.85f;
        public override float reloadFillValue => 0.1f;
        public override float accuracyFillValue => 0.8f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Revolver.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_REVOLVER_NAME",
"ROB_HUNK_BODY_SHOOT_REVOLVER_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public static ItemDef speedloader;

        public override void Init()
        {
            base.Init();
            this.weaponDef.roundReload = true;

            this.modelPrefab.AddComponent<Modules.Components.RevolverBehavior>();

            speedloader = ItemDef.Instantiate(Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/CloverVoid/CloverVoid.asset").WaitForCompletion());
            speedloader.name = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            speedloader.nameToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME";
            speedloader.descriptionToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            speedloader.pickupToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            speedloader.loreToken = "ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC";
            speedloader.canRemove = false;
            speedloader.hidden = false;
            speedloader.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpeedloaderIcon");
            speedloader.requiredExpansion = null;
            speedloader.tags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.CannotCopy,
                ItemTag.CannotDuplicate,
                ItemTag.CannotSteal,
                ItemTag.WorldUnique
            };
            speedloader.unlockableDef = null;

            speedloader.pickupModelPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlSpeedloader");
            //Modules.Assets.ConvertAllRenderersToHopooShader(speedloader.pickupModelPrefab);
            speedloader.pickupModelPrefab.GetComponentInChildren<MeshRenderer>().material = Modules.Assets.CreateMaterial("matSpeedloader", 0f, Color.black, 1f);

            HunkWeaponCatalog.itemDefs.Add(speedloader);

            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_NAME", "Speedloader (Quickdraw Army)");
            LanguageAPI.Add("ROB_HUNK_WEAPON_ADDON_" + weaponNameToken + "_DESC", "A custom part for the Quickdraw Army, allowing you to fill your chamber faster when empty.");
        }
    }
}