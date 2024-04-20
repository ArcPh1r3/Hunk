using UnityEngine;
using RoR2;
using RoR2.Skills;

[CreateAssetMenu(fileName = "wpn", menuName = "ScriptableObjects/WeaponDef", order = 1)]
public class HunkWeaponDef : ScriptableObject
{
    public enum AnimationSet
    {
        Pistol,
        SMG
    }

    [Header("General")]
    public string nameToken = "";
    public string descriptionToken = "";
    public Texture icon = null;
    public GameObject crosshairPrefab = null;
    public int magSize = 8;

    [Header("Skills")]
    public SkillDef primarySkillDef;

    [Header("Visuals")]
    public GameObject modelPrefab;
    public AnimationSet animationSet = AnimationSet.SMG;
    public bool storedOnBack = true;

    [HideInInspector]
    public ushort index; // assigned at runtime
    [HideInInspector]
    public GameObject pickupPrefab; // same thing

    public static HunkWeaponDef CreateWeaponDefFromInfo(HunkWeaponDefInfo weaponDefInfo)
    {
        HunkWeaponDef weaponDef = (HunkWeaponDef)ScriptableObject.CreateInstance(typeof(HunkWeaponDef));
        weaponDef.name = weaponDefInfo.nameToken;

        weaponDef.nameToken = weaponDefInfo.nameToken;
        weaponDef.descriptionToken = weaponDefInfo.descriptionToken;
        weaponDef.icon = weaponDefInfo.icon;
        weaponDef.crosshairPrefab = weaponDefInfo.crosshairPrefab;
        weaponDef.magSize = weaponDefInfo.magSize;

        weaponDef.primarySkillDef = weaponDefInfo.primarySkillDef;

        weaponDef.modelPrefab = weaponDefInfo.modelPrefab;
        weaponDef.animationSet = weaponDefInfo.animationSet;
        weaponDef.storedOnBack = weaponDefInfo.storedOnBack;

        return weaponDef;
    }
}

[System.Serializable]
public struct HunkWeaponDefInfo
{
    public string nameToken;
    public string descriptionToken;
    public Texture icon;
    public GameObject crosshairPrefab;
    public int magSize;

    public SkillDef primarySkillDef;

    public GameObject modelPrefab;
    public HunkWeaponDef.AnimationSet animationSet;
    public bool storedOnBack;
}