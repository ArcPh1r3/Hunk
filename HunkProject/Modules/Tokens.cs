using R2API;
using System;

namespace HunkMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = MainPlugin.developerPrefix + "_HUNK_BODY_";

            string desc = "The Driver is literally me.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Each weapon has its own unique strengths and weaknesses so be sure to pick the right tool for the job." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Focus greatly increases your damage output, but be careful not to get flanked while aiming." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Combat Slide while shooting to make sure your damage has no downtime." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Flashbang can be used to make a clean getaway in a pinch." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, still the same as he was when he began.";
            string outroFailure = "..and so he vanished, never to become a real human being.";

            string lore = "Back against the wall and odds\n";
            lore += "With the strength of a will and a cause\n";
            lore += "Your pursuits are called outstanding\n";
            lore += "You’re emotionally complex\n\n";
            lore += "Against the grain of dystopic claims\n";
            lore += "Not the thoughts your actions entertain\n";
            lore += "And you have proved to be\n\n\n";
            lore += "A real human being and a real hero";

            LanguageAPI.Add(prefix + "NAME", "Hunk");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Grim Reaper");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Survivalist");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Enemies have a chance to drop a new <style=cIsUtility>weapon</style>. These give you <style=cIsDamage>powerful attacks</style> for a limited time!");

            LanguageAPI.Add(prefix + "PASSIVE2_NAME", "Marksman (Legacy)");
            LanguageAPI.Add(prefix + "PASSIVE2_DESCRIPTION", $"Your trusty <style=cIsHealth>pistol</style> is all you need.");

            LanguageAPI.Add(prefix + "PASSIVE3_NAME", "Leadfoot");
            LanguageAPI.Add(prefix + "PASSIVE3_DESCRIPTION", $"My words are my <style=cIsHealth>bullets</style>.");

            LanguageAPI.Add(prefix + "PASSIVE4_NAME", "Godsling");
            LanguageAPI.Add(prefix + "PASSIVE4_DESCRIPTION", $"I <style=cIsHealth>drive</style>.");

            LanguageAPI.Add(prefix + "CONFIRM_NAME", "Confirm");
            LanguageAPI.Add(prefix + "CONFIRM_DESCRIPTION", "Proceed with the current skill.");

            LanguageAPI.Add(prefix + "CANCEL_NAME", "Cancel");
            LanguageAPI.Add(prefix + "CANCEL_DESCRIPTION", "Cancel the current skill.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_FIRE_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_FIRE_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * 2}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_AIM_NAME", "Steady Aim");
            LanguageAPI.Add(prefix + "SECONDARY_AIM_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * 5}% damage</style>. <style=cIsUtility>Boosts rate of fire and accuracy.</style>");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DODGE_NAME", "Tactical Dodge");
            LanguageAPI.Add(prefix + "UTILITY_DODGE_DESCRIPTION", "<style=cIsUtility>Dash</style> a short distance. You can <style=cIsUtility>hold up to 2 charges.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_GRENADE_NAME", "Flashbang");
            LanguageAPI.Add(prefix + "SPECIAL_GRENADE_DESCRIPTION", $"Throw a grenade that <style=cIsUtility>dazes</style> enemies for <style=cIsDamage>{100f * 1}% damage</style>. <style=cIsUtility>Dazed enemies aim in random directions for 10 seconds.</style>");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "A Real Hero");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "A Real Hero");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Reach stage 3 in less than 15 minutes.");

            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Driver: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Driver: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Driver, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
