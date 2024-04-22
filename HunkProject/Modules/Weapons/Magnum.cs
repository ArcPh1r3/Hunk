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
        public override float reloadDuration => 1.8f;
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
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}