using RoR2;
using UnityEngine;

namespace HunkMod.Modules
{
    internal static class Helpers
    {
        internal const string agilePrefix = "<style=cIsUtility>Agile.</style> ";

        internal const string whiteItemHex = "00FF66";
        internal const string greenItemHex = "00FF66";
        internal const string redItemHex = "FF0033";
        internal const string yellowItemHex = "FFFF00";
        internal const string lunarItemHex = "0066FF";
        internal const string voidItemHex = "C678B4";
        internal const string colorSuffix = "</color>";

        internal static Color whiteItemColor = new Color(1f, 1f, 1f);
        internal static Color greenItemColor = new Color(0f, 1f, 102f / 255f);
        internal static Color redItemColor = new Color(1f, 0f, 51f / 255f);
        internal static Color yellowItemColor = new Color(1f, 1f, 0f);
        internal static Color lunarItemColor = new Color(0f, 102f / 255f, 1f);
        internal static Color voidItemColor = new Color(198f / 255f, 120f / 255f, 180f / 255f);
        internal static Color badColor = new Color(127f / 255f, 0f, 0f);

        internal static string ScepterDescription(string desc)
        {
            return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
        }

        public static bool isHunkInPlay
        {
            get
            {
                bool isHunkOnPlayerTeam = false;
                foreach (CharacterBody i in CharacterBody.readOnlyInstancesList)
                {
                    if (i && i.teamComponent && i.teamComponent.teamIndex == TeamIndex.Player && i.baseNameToken == Modules.Survivors.Hunk.bodyNameToken)
                    {
                        isHunkOnPlayerTeam = true;
                        break;
                    }
                }
                return isHunkOnPlayerTeam;
            }
        }
    }
}