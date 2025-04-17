using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2Cpp;
using Il2CppRewired;
using Il2CppSabotage.Graph.Core;
using Il2CppSabotage.Localization;
using MelonLoader;
using UnityEngine;

namespace OpenSeaOfStars.Helpers
{
    internal class DialogueHelper : MelonLogger
    {
        public static List<string> LocalizationIdConstants = new List<string>() {
            "OPEN_SEA_OF_STARS_VESPERTINE_SEA_OF_NIGHTMARE"
        };

        [HarmonyPatch(typeof(LocalizationId), "GetText")]
        private static class AdditionalDialoguePatch
        {
            [HarmonyPrefix]
            private static bool Prefix(LocalizationId __instance, ref string __result)
            {

                if (__instance.locId.Equals(LocalizationIdConstants[0]))
                {
                    __result = "Sea of Nightmare";
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(LocalizationId), "GetText", new Type[] {typeof(Controller), typeof(Player)})]
        private static class AdditionalDialogueParamsPatch
        {
            [HarmonyPrefix]
            private static bool Prefix(LocalizationId __instance, ref string __result, Controller controller, Player player)
            {

                if (__instance.locId.Equals(LocalizationIdConstants[0]))
                {
                    __result = "Sea of Nightmare";
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(LocalizationManager), "GetText", new Type[] { typeof(LocalizationId), typeof(Controller), typeof(Player) })]
        private static class AdditionalDialogueInManagerPatch
        {
            [HarmonyPrefix]
            private static bool Prefix(ref string __result, LocalizationId localizationId, Controller controller, Player player)
            {

                if (localizationId.locId.Equals(LocalizationIdConstants[0]))
                {
                    __result = "Sea of Nightmare";
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(LocalizationManager), "GetText", new Type[] { typeof(string), typeof(string), typeof(Controller), typeof(Player) })]
        private static class AdditionalDialogueInManagerLongPatch
        {
            [HarmonyPrefix]
            private static bool Prefix(ref string __result, string locId, Controller controller, Player player)
            {

                if (locId.Equals(LocalizationIdConstants[0]))
                {
                    __result = "Sea of Nightmare";
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(LocalizationManager), "GetText", new Type[] { typeof(string) })]
        private static class AdditionalDialogueInManagerLocPatch
        {
            [HarmonyPrefix]
            private static bool Prefix(ref string __result, string locId)
            {

                if (locId.Equals(LocalizationIdConstants[0]))
                {
                    __result = "Sea of Nightmare";
                    return false;
                }

                return true;
            }
        }
    }
}
