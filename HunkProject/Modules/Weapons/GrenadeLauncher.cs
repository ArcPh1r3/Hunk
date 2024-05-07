using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public class GrenadeLauncher : BaseWeapon<GrenadeLauncher>
    {
        public override string weaponNameToken => "GRENADELAUNCHER";
        public override string weaponName => "GM-79";
        public override string weaponDesc => "Single-round break-action grenade launcher capable for firing flame rounds. Great for area damage, but slow to reload.";
        public override string iconName => "texGrenadeLauncherIcon";
        public override GameObject crosshairPrefab => Modules.Assets.grenadeLauncherCrosshairPrefab2;
        public override int magSize => 1;
        public override float magPickupMultiplier => 2f;
        public override float reloadDuration => 2.4f;
        public override string ammoName => "Incendiary Rounds";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlGrenadeLauncher");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.SMG;
        public override bool storedOnBack => true;
        public override float damageFillValue => 1f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.1f;
        public override float reloadFillValue => 0.4f;
        public override float accuracyFillValue => 0.83f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.GrenadeLauncher.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_ROCKETLAUNCHER_NAME",
"ROB_HUNK_BODY_SHOOT_ROCKETLAUNCHER_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);

        public override void Init()
        {
            base.Init();
            this.modelPrefab.AddComponent<Modules.Components.GrenadeLauncherBehavior>();
            this.weaponDef.exposeWeakPoints = false;
        }
    }
}