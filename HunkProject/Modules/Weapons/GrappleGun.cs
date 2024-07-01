using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class GrappleGun : BaseWeapon<GrappleGun>
    {
        public override string weaponNameToken => "GRAPPLEGUN";
        public override string weaponName => "Hookshot";
        public override string weaponDesc => "Shoots a grapple hook attached to a zip-line that allows you to propel yourself to any desired location.";
        public override string iconName => "texGrappleGunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.LoadCrosshair("Loader");
        public override int magSize => 999;
        public override float magPickupMultiplier => 0f;
        public override int startingMags => 0;
        public override float reloadDuration => 0f;
        public override string ammoName => "";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlGrappleGun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0f;
        public override float rangefillValue => 0.6f;
        public override float fireRateFillValue => 0f;
        public override float reloadFillValue => 0f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.GrappleGun.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_MUP_NAME",
"ROB_HUNK_BODY_SHOOT_MUP_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            base.Init();
            this.weaponDef.exposeWeakPoints = false;
            this.weaponDef.canPickUpAmmo = false;
        }
    }
}