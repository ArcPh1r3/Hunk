using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HunkMod.Modules.Weapons
{
    public class RocketLauncher : BaseWeapon<RocketLauncher>
    {
        public override string weaponNameToken => "ROCKETLAUNCHER";
        public override string weaponName => "Anti-tank Rocket";
        public override string weaponDesc => "A portable rocket launcher that can hold four 66mm incendiary rockets. The rockets unleash a powerful conflagration.";
        public override string iconName => "texRocketLauncherIcon";
        public override GameObject crosshairPrefab => Modules.Assets.rocketLauncherCrosshairPrefab;
        public override int magSize => 4;
        public override float reloadDuration => 3f;
        public override string ammoName => "66mm Rockets";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlRocketLauncher");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.Rocket;
        public override bool storedOnBack => true;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 0.8f;
        public override float fireRateFillValue => 0.1f;
        public override float reloadFillValue => 0.1f;
        public override float accuracyFillValue => 0.9f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.RocketLauncher.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_ROCKETLAUNCHER_NAME",
"ROB_HUNK_BODY_SHOOT_ROCKETLAUNCHER_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            base.Init();
            this.modelPrefab.AddComponent<Modules.Components.RocketLauncherVisuals>();
            this.weaponDef.allowAutoReload = false;
            this.weaponDef.exposeWeakPoints = false;
        }
    }
}