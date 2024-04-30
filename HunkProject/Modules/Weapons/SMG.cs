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
    }
}