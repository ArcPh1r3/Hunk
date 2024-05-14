using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace HunkMod.Modules
{
    internal static class Config
    {
        public static ConfigFile myConfig;

        public static ConfigEntry<float> baseDropRate;
        public static ConfigEntry<bool> dynamicCrosshair;
        public static ConfigEntry<bool> enableRecoil;
        public static ConfigEntry<bool> capInfection;
        public static ConfigEntry<bool> globalInfectionSound;
        public static ConfigEntry<bool> customEscapeSequence;
        public static ConfigEntry<bool> permanentInfectionEvent;
        public static ConfigEntry<bool> showWeaponIcon;
        public static ConfigEntry<bool> fancyAmmoDisplay;
        public static ConfigEntry<float> baseAmmoPanelOpacity;
        public static ConfigEntry<bool> fancyShield;
        public static ConfigEntry<bool> fancyShieldGlobal;
        public static ConfigEntry<bool> shieldBubble;
        public static ConfigEntry<bool> blacklistHunkItems;
        public static ConfigEntry<bool> overTheShoulderCamera;
        public static ConfigEntry<bool> overTheShoulderCamera2;
        public static ConfigEntry<bool> cursed;

        public static ConfigEntry<float> baseHealth;
        public static ConfigEntry<float> healthGrowth;
        public static ConfigEntry<float> baseDamage;
        public static ConfigEntry<float> damageGrowth;
        public static ConfigEntry<float> baseArmor;
        public static ConfigEntry<float> armorGrowth;
        public static ConfigEntry<float> baseMovementSpeed;
        public static ConfigEntry<float> baseCrit;
        public static ConfigEntry<float> baseRegen;

        public static ConfigEntry<KeyboardShortcut> restKey;
        public static ConfigEntry<KeyboardShortcut> tauntKey;
        public static ConfigEntry<KeyboardShortcut> danceKey;

        internal static void ReadConfig()
        {
            #region General
            baseDropRate
= Config.BindAndOptionsSlider("01 - General",
"Base Drop Rate",
4f,
"Base chance for ammo to drop on kill", 0f, 100f);

            dynamicCrosshair
= Config.BindAndOptions("01 - General",
"Dynamic Crosshair",
true,
"If set to false, will no longer highlight the crosshair when hovering over entities. (Client-side)", true);

            capInfection
= Config.BindAndOptions("01 - General",
"Cap Infection",
true,
"Caps G-Virus infection at 5 stacks of mutation. Set to false to restore the original infinite scaling.");

            globalInfectionSound
= Config.BindAndOptions("01 - General",
"Infection Sound Cue",
true,
"Set to false to disable the global sound cue when an Infected enemy spawns. (Client-side)");

            customEscapeSequence
= Config.BindAndOptions("01 - General",
"Custom Escape Sequence",
true,
"Set to false to disable the custom Moon escape sequence. (Client-side)");

            permanentInfectionEvent
= Config.BindAndOptions("01 - General",
"Permanent Infection Event",
false,
"Set to true to cause an Infected enemy to spawn on every stage, regardless of how many Keycards you have.");

            showWeaponIcon
= Config.BindAndOptions("01 - General",
"Show Gun Icon",
true,
"Set to false to disable the gun icon next to your skills. (Client-side)");

            fancyAmmoDisplay
= Config.BindAndOptions("01 - General",
"Fancy Ammo Display",
true,
"Set to false to disable the fancy ammo display and use the old, simple one. (Client-side)");

            baseAmmoPanelOpacity
    = Config.BindAndOptionsSlider("01 - General",
             "Base Ammo Panel Opacity",
             80f,
             "Opacity of the black panel the ammo count is shown on", 0f, 100f);

            fancyShield
= Config.BindAndOptions("01 - General",
"Fancy Shield",
true,
"Set to false to disable the custom shield overlay and use the ugly vanilla overlay. (Client-side)", true);

            fancyShieldGlobal
= Config.BindAndOptions("01 - General",
"Fancy Shield (Global)",
false,
"Set to true to give this shield overlay to every survivor. (Client-side)", true);

            shieldBubble
= Config.BindAndOptions("01 - General",
"Shield Bubble",
false,
"Set to true to enable a custom shield bubble. Only works with Fancy Shield enabled! (Client-side)", true);

            blacklistHunkItems
= Config.BindAndOptions("01 - General",
"Multiplayer Protection",
true,
"Prevents Non-HUNK players from grabbing HUNK's key items (G-Virus Sample and Keycards). Set to false if you have faith in your allies.");

            enableRecoil
= Config.BindAndOptions("01 - General",
"Enable Recoil",
true,
"Set to false to disable recoil from shooting guns.");

            overTheShoulderCamera
= Config.BindAndOptions("01 - General",
"Enable Over The Shoulder Camera",
true,
"Set to false to use more standard camera positioning.", true);

            overTheShoulderCamera2
= Config.BindAndOptions("01 - General",
"Enable Resident Evil Style Camera",
false,
"Set to true to use a more tight camera position based on Resident Evil 2.", true);

            cursed
= Config.BindAndOptions("01 - General",
"Cursed",
false,
"Enables unfinished, stupid and old content.", true);
            #endregion

            #region Emotes
            restKey
                = Config.BindAndOptions("02 - Keybinds",
                         "Rest Emote",
                         new KeyboardShortcut(KeyCode.Alpha1),
                         "Key used to Rest");
            tauntKey
                = Config.BindAndOptions("02 - Keybinds",
                                     "Salute Emote",
                                     new KeyboardShortcut(KeyCode.Alpha2),
                                     "Key used to Taunt");

            danceKey
                = Config.BindAndOptions("02 - Keybinds",
                                     "Dance Emote",
                                     new KeyboardShortcut(KeyCode.Alpha3),
                                     "Key used to Dance");
            #endregion

            #region Stats
            baseHealth
                = Config.BindAndOptionsSlider("03 - Character Stats",
                         "Base Health",
                         110f,
                         "", 1f, 500f, true);
            healthGrowth
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Health Growth",
                                     33f,
                                     "", 0f, 100f, true);
            baseRegen
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Base Health Regen",
                                     1.5f,
                                     "", 0f, 5f, true);
            baseArmor
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Base Armor",
                                     20f,
                                     "", 0f, 20f, true);
            armorGrowth
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Armor Growth",
                                     0f,
                                     "", 0f, 2f, true);
            baseDamage
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Base Damage",
                                     12f,
                                     "", 1f, 24f, true);
            damageGrowth
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Damage Growth",
                                     2.4f,
                                     "", 0f, 5f, true);
            baseMovementSpeed
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Base Movement Speed",
                                     7f,
                                     "", 0f, 14f, true);
            baseCrit
                = Config.BindAndOptionsSlider("03 - Character Stats",
                                     "Base Crit",
                                     1f,
                                     "", 0f, 100f, true);
            #endregion
        }

        public static void InitROO(Sprite modSprite, string modDescription)
        {
            if (MainPlugin.rooInstalled) _InitROO(modSprite, modDescription);
        }

        public static void _InitROO(Sprite modSprite, string modDescription)
        {
            ModSettingsManager.SetModIcon(modSprite);
            ModSettingsManager.SetModDescription(modDescription);
        }

        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<T> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (MainPlugin.rooInstalled)
            {
                TryRegisterOption(configEntry, restartRequired);
            }

            return configEntry;
        }

        public static ConfigEntry<float> BindAndOptionsSlider(string section, string name, float defaultValue, string description = "", float min = 0, float max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            description += " (Default: " + defaultValue + ")";

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<float> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (MainPlugin.rooInstalled)
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        public static ConfigEntry<int> BindAndOptionsSlider(string section, string name, int defaultValue, string description = "", int min = 0, int max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            description += " (Default: " + defaultValue + ")";

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<int> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (MainPlugin.rooInstalled)
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOption<T>(ConfigEntry<T> entry, bool restartRequired)
        {
            if (entry is ConfigEntry<float>)
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = 0, max = 20, formatString = "{0:0.00}", restartRequired = restartRequired }));
            }
            if (entry is ConfigEntry<int>)
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, restartRequired));
            }
            if (entry is ConfigEntry<bool>)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(entry as ConfigEntry<bool>, restartRequired));
            }
            if (entry is ConfigEntry<KeyboardShortcut>)
            {
                ModSettingsManager.AddOption(new KeyBindOption(entry as ConfigEntry<KeyboardShortcut>, restartRequired));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<int> entry, int min, int max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, new IntSliderConfig() { min = min, max = max, formatString = "{0:0.00}", restartRequired = restartRequired }));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<float> entry, float min, float max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = min, max = max, formatString = "{0:0.00}", restartRequired = restartRequired }));
        }

        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return Config.BindAndOptions("01 - General",
                         "Enabled",
                         true,
                         "Set to false to disable this character", true);
        }

        internal static ConfigEntry<bool> ForceUnlockConfig(string characterName)
        {
            return Config.BindAndOptions("01 - General",
                         "Force Unlock",
                         false,
                         "Makes this character unlocked by default", true);
        }

        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }


    public class StageSpawnInfo 
    {
        private string stageName;
        private int minStages;

        public StageSpawnInfo(string stageName, int minStages) {
            this.stageName = stageName;
            this.minStages = minStages;
        }

        public string GetStageName() { return stageName; }
        public int GetMinStages() { return minStages; }
    }
}