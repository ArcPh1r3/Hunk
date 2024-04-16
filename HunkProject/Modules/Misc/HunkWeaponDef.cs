using UnityEngine;
using RoR2;
using RoR2.Skills;

[CreateAssetMenu(fileName = "wpn", menuName = "ScriptableObjects/WeaponDef", order = 1)]
public class HunkWeaponDef : ScriptableObject
{
    public enum AnimationSet
    {
        Default,
        Knife,
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
    public AnimationSet animationSet = AnimationSet.Default;

    [HideInInspector]
    public ushort index; // assigned at runtime
    [HideInInspector]
    public GameObject pickupPrefab; // same thing

    public string equipAnimationString
    {
        get
        {
            if (this.animationSet == AnimationSet.Default) return "BufferEmpty";
            return "BufferEmpty";
        }
    }

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
}