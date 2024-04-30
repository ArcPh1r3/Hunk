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
        public override string iconName => "texM19Icon";
        public override GameObject crosshairPrefab => Modules.Assets.magnumCrosshairPrefab;
        public override int magSize => 6;
        public override float reloadDuration => 1.2f;
        public override string ammoName => ".45 ACP Rounds";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlRevolver");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.75f;
        public override float reloadFillValue => 1f;
        public override float accuracyFillValue => 0.8f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.M19.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}