using UnityEngine;
using RoR2;
using RoR2.Skills;

[CreateAssetMenu(fileName = "wpn", menuName = "ScriptableObjects/WeaponDef", order = 1)]
public class HunkWeaponDef : ScriptableObject
{
    public enum AnimationSet
    {
        Pistol,
        SMG,
        Rocket,
        Unarmed,
        Throwable,
        PistolAlt,
        Railgun
    }

    [Header("General")]
    public string nameToken = "";
    public string descriptionToken = "";
    public Sprite icon = null;
    public GameObject crosshairPrefab = null;
    public int magSize = 8;
    public float magPickupMultiplier = 1f;
    public int startingMags = 1;
    public float reloadDuration = 2.4f;
    public string ammoName = "";
    public bool allowAutoReload = true;
    public bool exposeWeakPoints = true;
    public bool roundReload = false;
    public bool canPickUpAmmo = true;

    [Header("Skills")]
    public SkillDef primarySkillDef;

    [Header("Visuals")]
    public GameObject modelPrefab;
    public AnimationSet animationSet = AnimationSet.SMG;
    public bool storedOnBack = true;

    [Header("UI")]
    public float damageFillValue;
    public float rangefillValue;
    public float fireRateFillValue;
    public float reloadFillValue;
    public float accuracyFillValue;

    [HideInInspector]
    public ushort index; // assigned at runtime
    [HideInInspector]
    public ItemDef itemDef;

    public static HunkWeaponDef CreateWeaponDefFromInfo(HunkWeaponDefInfo weaponDefInfo)
    {
        HunkWeaponDef weaponDef = (HunkWeaponDef)ScriptableObject.CreateInstance(typeof(HunkWeaponDef));
        weaponDef.name = weaponDefInfo.nameToken;

        weaponDef.nameToken = weaponDefInfo.nameToken;
        weaponDef.descriptionToken = weaponDefInfo.descriptionToken;
        weaponDef.icon = weaponDefInfo.icon;
        weaponDef.crosshairPrefab = weaponDefInfo.crosshairPrefab;
        weaponDef.magSize = weaponDefInfo.magSize;
        weaponDef.magPickupMultiplier = weaponDefInfo.magPickupMultiplier;
        weaponDef.startingMags = weaponDefInfo.startingMags;
        weaponDef.reloadDuration = weaponDefInfo.reloadDuration;
        weaponDef.ammoName = weaponDefInfo.ammoName;

        weaponDef.primarySkillDef = weaponDefInfo.primarySkillDef;

        weaponDef.modelPrefab = weaponDefInfo.modelPrefab;
        weaponDef.animationSet = weaponDefInfo.animationSet;
        weaponDef.storedOnBack = weaponDefInfo.storedOnBack;

        weaponDef.damageFillValue = weaponDefInfo.damageFillValue;
        weaponDef.rangefillValue = weaponDefInfo.rangefillValue;
        weaponDef.fireRateFillValue = weaponDefInfo.fireRateFillValue;
        weaponDef.reloadFillValue = weaponDefInfo.reloadFillValue;
        weaponDef.accuracyFillValue = weaponDefInfo.accuracyFillValue;

        return weaponDef;
    }

    public HunkWeaponDef CloneWeapon(bool addToCatalog = true)
    {
        HunkWeaponDef weaponDef = HunkWeaponDef.Instantiate(this);

        HunkMod.HunkWeaponCatalog.AddWeapon(weaponDef, false);
        R2API.ContentAddition.AddItemDef(weaponDef.itemDef);

        return weaponDef;
    }
}

[System.Serializable]
public struct HunkWeaponDefInfo
{
    public string nameToken;
    public string descriptionToken;
    public Sprite icon;
    public GameObject crosshairPrefab;
    public int magSize;
    public float magPickupMultiplier;
    public int startingMags;
    public float reloadDuration;
    public string ammoName;

    public SkillDef primarySkillDef;

    public GameObject modelPrefab;
    public HunkWeaponDef.AnimationSet animationSet;
    public bool storedOnBack;

    public float damageFillValue;
    public float rangefillValue;
    public float fireRateFillValue;
    public float reloadFillValue;
    public float accuracyFillValue;
}