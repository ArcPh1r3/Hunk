using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public class GoldenGun : BaseWeapon<GoldenGun>
    {
        public override string weaponNameToken => "GOLDGUN";
        public override string weaponName => "Golden Gun";
        public override string weaponDesc => "1-round capacity .50 AE MAG. Gas-operated action, which is unusual for a semi-auto handgun, gives it both power and accuracy.";
        public override string iconName => "texGoldenGunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.magnumCrosshairPrefab;
        public override int magSize => 1;
        public override float magPickupMultiplier => 1f;
        public override float reloadDuration => 1.8f;
        public override string ammoName => "Golden Ammo";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlGoldenGun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 1f;
        public override float fireRateFillValue => 0.1f;
        public override float reloadFillValue => 0.45f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.GoldenGun.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}