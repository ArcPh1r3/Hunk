using System.Collections.Generic;
using System;

namespace HunkMod.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }

        public static void RegisterStates()
        {
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.MainState));

            entityStates.Add(typeof(HunkMod.SkillStates.Emote.BaseEmote));
            entityStates.Add(typeof(HunkMod.SkillStates.Emote.Rest));
            entityStates.Add(typeof(HunkMod.SkillStates.Emote.Taunt));
            entityStates.Add(typeof(HunkMod.SkillStates.Emote.Dance));

            entityStates.Add(typeof(HunkMod.SkillStates.FuckMyAss));

            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.BaseHunkSkillState));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.KnifeCounter));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Reload));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Roll));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.SteadyAim));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Step));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Swap));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.SwapWeapon));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.SwingAltKnife));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.SwingKnife));

            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Weapon.M19.Shoot));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Weapon.Magnum.Shoot));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Weapon.Shotgun.Shoot));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Weapon.Slugger.Shoot));
            entityStates.Add(typeof(HunkMod.SkillStates.Hunk.Weapon.SMG.Shoot));
        }
    }
}