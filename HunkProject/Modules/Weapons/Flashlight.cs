using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class Flashlight : BaseWeapon<Flashlight>
    {
        public override string weaponNameToken => "FLASHLIGHT";
        public override string weaponName => "Flashlight";
        public override string weaponDesc => "Helps you see in the dark.";
        public override string iconName => "texMUPIcon";
        public override GameObject crosshairPrefab => Modules.Assets.pistolCrosshairPrefab;
        public override int magSize => 0;
        public override float magPickupMultiplier => 0f;
        public override float reloadDuration => 0f;
        public override string ammoName => "";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlFlashlight");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0f;
        public override float rangefillValue => 0.4f;
        public override float fireRateFillValue => 0f;
        public override float reloadFillValue => 0f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.MUP.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_MUP_NAME",
"ROB_HUNK_BODY_SHOOT_MUP_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}