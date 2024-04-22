using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class Slugger : BaseWeapon<Slugger>
    {
        public override string weaponNameToken => "SLUGGER";
        public override string weaponName => "Slugger";
        public override string weaponDesc => "4-round capacity 12-gauge pump-action shotgun. Its sturdy steel action makes this popular model reliable and easy to control.";
        public override string iconName => "texShotgunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.shotgunCrosshairPrefab;
        public override int magSize => 4;
        public override float reloadDuration => 3f;
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlShotgun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;
        public override bool storedOnBack => true;
        public override float damageFillValue => 0.7f;
        public override float rangefillValue => 0.5f;
        public override float fireRateFillValue => 0.3f;
        public override float reloadFillValue => 0.2f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Slugger.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}