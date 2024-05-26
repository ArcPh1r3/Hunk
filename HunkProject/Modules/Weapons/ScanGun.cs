using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class ScanGun : BaseWeapon<ScanGun>
    {
        public override string weaponNameToken => "SCANNER";
        public override string weaponName => "EMF Scanner";
        public override string weaponDesc => "A device capable of detecting EM fields through walls in real time, it can be used to locate certain electronics.";
        public override string iconName => "texEMFIcon";
        public override GameObject crosshairPrefab => Modules.Assets.grenadeLauncherCrosshairPrefab2;
        public override int magSize => 999;
        public override float magPickupMultiplier => 0f;
        public override int startingMags => 0;
        public override float reloadDuration => 0f;
        public override string ammoName => "EMF Battery";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlEMFVisualizer");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Pistol;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0f;
        public override float reloadFillValue => 0f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.ScanGun.Scan)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_SCANNER_NAME",
"ROB_HUNK_BODY_SHOOT_SCANNER_DESCRIPTION",
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