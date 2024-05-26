using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public class Railgun : BaseWeapon<Railgun>
    {
        public override string weaponNameToken => "RAILGUN";
        public override string weaponName => "Ferromagnetic Infantry-use Next Generation Railgun";
        public override string weaponDesc => "A portable rail cannon developed by Cornell Garner and the U.S. military in order to eliminate bioweapons in case of an outbreak.";
        public override string iconName => "texRailgunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.grenadeLauncherCrosshairPrefab2;
        public override int magSize => 1000;
        public override float magPickupMultiplier => 0.01f;
        public override int startingMags => 0;
        public override float reloadDuration => 3f;
        public override string ammoName => "????";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlRailgun");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Railgun;
        public override bool storedOnBack => true;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 1f;
        public override float fireRateFillValue => 0.05f;
        public override float reloadFillValue => 0.05f;
        public override float accuracyFillValue => 1f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.Railgun.Charge)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_RAILGUN_NAME",
"ROB_HUNK_BODY_SHOOT_RAILGUN_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            base.Init();
            //this.modelPrefab.AddComponent<Modules.Components.RocketLauncherVisuals>();
            this.weaponDef.allowAutoReload = false;
            this.weaponDef.exposeWeakPoints = false;
            this.weaponDef.canPickUpAmmo = false;
        }
    }
}