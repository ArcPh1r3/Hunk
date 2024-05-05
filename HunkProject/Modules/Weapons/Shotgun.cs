using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public class Shotgun : BaseWeapon<Shotgun>
    {
        public override string weaponNameToken => "SHOTGUN";
        public override string weaponName => "W-870";
        public override string weaponDesc => "8-round capacity 12-gauge pump-action shotgun. Its sturdy steel action makes this popular model reliable and easy to control.";
        public override string iconName => "texShotgunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.shotgunCrosshairPrefab;
        public override int magSize => 8;
        public override float magPickupMultiplier => 1f;
        public override float reloadDuration => 0.6f;
        public override string ammoName => "Shotgun Shells";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlShotgun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;
        public override bool storedOnBack => true;
        public override float damageFillValue => 0.7f;
        public override float rangefillValue => 0.4f;
        public override float fireRateFillValue => 0.3f;
        public override float reloadFillValue => 0.2f;
        public override float accuracyFillValue => 0.6f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Shotgun.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_SHOTGUN_NAME",
"ROB_HUNK_BODY_SHOOT_SHOTGUN_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            base.Init();
            this.weaponDef.roundReload = true;
        }
    }
}