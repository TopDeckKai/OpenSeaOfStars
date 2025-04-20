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

        private static Dictionary<string, string> newDialogue = new Dictionary<string, string>() {
            { LocalizationIdConstants[0], "Sea of Nightmare" }
        };

        private class DialoguePatchData
        {
            public string localizationId = "";
            public string gameObject = "";
            public string category = "";
            public int nodePos = 0;
            public int slot = 0;
        }

        private static Dictionary<string, DialoguePatchData[]> dialoguePatch = new Dictionary<string, DialoguePatchData[]>()
        {
            { 
                "vespertine_cutscene_worldmap", new DialoguePatchData[] { 
                    new DialoguePatchData { localizationId = LocalizationIdConstants[0], gameObject = "DIALOGUES/DIA_Hortence_TakeHelm", category = "Camping", nodePos = 1, slot = 1 } 
                } 
            }
        };

        public bool changeDialoguePerScene(string scene)
        {
            DialoguePatchData[] dialoguePatches = dialoguePatch[scene];

            if (dialoguePatches != null)
            {
                int success = 0;

                foreach (DialoguePatchData dpd in dialoguePatches)
                {
                    GameObject gameDialogue = GameObject.Find(dpd.gameObject);
                    if (gameDialogue != null)
                    {
                        CutsceneTreeController ctc = gameDialogue.GetComponent<CutsceneTreeController>();
                        if (ctc != null)
                        {
                            CutsceneTree ct = ctc.currentGraph;
                            if (ct != null && ct.nodes.Count > 0)
                            {
                                // Surrounding with try catch because comparing the type with "is" isn't working, chalking that up to modding library jank?
                                try
                                {
                                    GraphNode node = ct.nodes[dpd.nodePos];
                                    PlayDialogNode dialogueNode = node.Cast<PlayDialogNode>();
                                    DialogBoxData dbd = dialogueNode.dialogBoxData.value;
                                    DialogBoxData.DialogChoice dc = new DialogBoxData.DialogChoice();
                                    dc.characterDefinitionId = CharacterDefinitionId.LeaderCharacter;
                                    LocalizationId sonLocId = new LocalizationId();
                                    sonLocId.categoryName = dpd.category;
                                    sonLocId.locId = DialogueHelper.LocalizationIdConstants[0];
                                    dc.localizationId = sonLocId;
                                    dbd.dialogChoices.Insert(dpd.slot, dc);
                                    dialogueNode.dialogBoxData.value = dbd;

                                    Msg("Loaded " + DialogueHelper.LocalizationIdConstants[0]);
                                    success = success + 1;
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                }

                if (success == dialoguePatches.Length)
                {
                    return true;
                }
            }

            return false;
        }

        [HarmonyPatch(typeof(LocalizationId), "GetText")]
        private static class AdditionalDialoguePatch
        {
            [HarmonyPrefix]
            private static bool Prefix(LocalizationId __instance, ref string __result)
            {
                if (LocalizationIdConstants.Any(s => s.Equals(__instance.locId)))
                {
                    __result = newDialogue[__instance.locId];
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
                if (LocalizationIdConstants.Any(s => s.Equals(__instance.locId)))
                {
                    __result = newDialogue[__instance.locId];
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
                if (LocalizationIdConstants.Any(s => s.Equals(localizationId.locId)))
                {
                    __result = newDialogue[localizationId.locId];
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
                if (LocalizationIdConstants.Any(s => s.Equals(locId)))
                {
                    __result = newDialogue[locId];
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
                if (LocalizationIdConstants.Any(s => s.Equals(locId)))
                {
                    __result = newDialogue[locId];
                    return false;
                }

                return true;
            }
        }
    }
}
