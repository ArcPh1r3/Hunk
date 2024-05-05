using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public class MUP : BaseWeapon<MUP>
    {
        public override string weaponNameToken => "MUP";
        public override string weaponName => "MUP";
        public override string weaponDesc => "16-shot capacity 9mm handgun. Used by professionals in many state organizations. Reliable with high accuracy.";
        public override string iconName => "texMUPIcon";
        public override GameObject crosshairPrefab => Modules.Assets.pistolCrosshairPrefab;
        public override int magSize => 16;
        public override float magPickupMultiplier => 2f;
        public override float reloadDuration => 1.2f;
        public override string ammoName => "9mm Ammo";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlMUP");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0.3f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.75f;
        public override float reloadFillValue => 1f;
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