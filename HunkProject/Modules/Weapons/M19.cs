using RoR2.Skills;
using UnityEngine;

namespace HunkMod.Modules.Weapons
{
    public class M19 : BaseWeapon<M19>
    {
        public override string weaponNameToken => "M19";
        public override string weaponName => "M19";
        public override string weaponDesc => "7-shot, large caliber, single-action handgun that fires .45 ACP rounds. Its base design hasn't changed in over 70 years so variants abound.";
        public override string iconName => "texM19Icon";
        public override GameObject crosshairPrefab => Modules.Assets.pistolCrosshairPrefab;
        public override int magSize => 7;
        public override float magPickupMultiplier => 1f;
        public override float reloadDuration => 1.2f;
        public override string ammoName => "M19 Rounds";
        public override GameObject modelPrefab => Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlM19");
        public override HunkWeaponDef.AnimationSet animationSet => HunkWeaponDef.AnimationSet.PistolAlt;
        public override bool storedOnBack => false;
        public override float damageFillValue => 0.5f;
        public override float rangefillValue => 0.7f;
        public override float fireRateFillValue => 0.7f;
        public override float reloadFillValue => 1f;
        public override float accuracyFillValue => 0.9f;

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
new EntityStates.SerializableEntityStateType(typeof(SkillStates.Hunk.Weapon.M19.Shoot)),
"Weapon",
"ROB_HUNK_BODY_SHOOT_M19_NAME",
"ROB_HUNK_BODY_SHOOT_M19_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShootIcon"),
false);
    }
}