using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class ATM : BaseWeapon<ATM>
    {
        public override string weaponNameToken => "ATM";
        public override string weaponName => "ATM-4";
        public override string weaponDesc => "A recoilless rocket launcher that fires 84mm projectiles. The piercing power of the rocket causes more damage than the explosion.";
        public override string iconName => "texShotgunIcon";
        public override GameObject crosshairPrefab => Modules.Assets.rocketLauncherCrosshairPrefab;
        public override int magSize => 1;
        public override float reloadDuration => 3f;
        public override string ammoName => "84mm Rockets";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlATM");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Rocket;
        public override bool storedOnBack => true;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 0.8f;
        public override float fireRateFillValue => 0.1f;
        public override float reloadFillValue => 0.1f;
        public override float accuracyFillValue => 0.9f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.ATM.Shoot)),
"Weapon",
"ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
"ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            base.Init();
            this.weaponDef.exposeWeakPoints = false;
        }
    }
}