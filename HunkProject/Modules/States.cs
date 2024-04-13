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
        }
    }
}