using HarmonyLib;
using Il2Cpp;
using Il2CppRewired;
using Il2CppSabotage.Graph.Core;
using Il2CppSabotage.Localization;
using MelonLoader;
using UnityEngine;

namespace OpenSeaOfStars.Helpers;

public class DialogueHelper : MelonLogger
{
    public static List<string> LocalizationIdConstants = new() {
        "OPEN_SEA_OF_STARS_VESPERTINE_SEA_OF_NIGHTMARE"
    };

    private class DialoguePatchData
    {
        public string localizationId = "";
        public string gameObject = "";
        public string category = "";
        public int nodePos;
        public int slot;
    }

    private class DialogueOptionPatchData
    {
        public string dialogueText = "";
        public string loadLevel = "";
    }

    private static Dictionary<string, DialogueOptionPatchData> newDialogueOption = new() {
        { LocalizationIdConstants[0], new DialogueOptionPatchData() {dialogueText = "Sea of Nightmare", loadLevel = "SeaOfNightmare_WorldMap" } }
    };


    private static Dictionary<string, DialoguePatchData[]> dialoguePatch = new()
    {
        { 
            "vespertine_cutscene_worldmap", new[] { 
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
                            try
                            {
                                GraphNode node = ct.nodes.ToArray()[dpd.nodePos];
                                PlayDialogNode dialogueNode = node.Cast<PlayDialogNode>();
                                DialogBoxData dbd = dialogueNode.dialogBoxData.value;
                                DialogBoxData.DialogChoice dc = new() { characterDefinitionId = CharacterDefinitionId.LeaderCharacter };
                                LocalizationId sonLocId = new()
                                {
                                    categoryName = dpd.category,
                                    locId = LocalizationIdConstants[0]
                                };
                                dc.localizationId = sonLocId;
                                dbd.dialogChoices.Insert(dpd.slot, dc);
                                dialogueNode.dialogBoxData.value = dbd;

                                Msg("Loaded " + LocalizationIdConstants[0]);
                                success++;
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

    [HarmonyPatch(typeof(PlayDialogNode), "OnDialogCompleted")]
    private static class DialogueChoicePatch
    {
        [HarmonyPostfix]
        private static void Postfix(PlayDialogNode __instance)
        {
            if (__instance.dialogBoxData.value.ContainsChoice()) {
                LocalizationId choiceLocId = __instance.dialogBoxData.value.dialogChoices.ToArray()[__instance.dialogChoiceIndex].localizationId;

                if (LocalizationIdConstants.Any(s => s.Equals(choiceLocId.locId)))
                {
                    if (!newDialogueOption[choiceLocId.locId].loadLevel.Equals(""))
                    {
                        OpenSeaOfStarsMod.OpenInstance.LevelHelper.loadLevel(newDialogueOption[choiceLocId.locId].loadLevel);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(LocalizationId), "GetText")]
    private static class AdditionalDialoguePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(LocalizationId __instance, ref string __result)
        {
            if (LocalizationIdConstants.Any(s => s.Equals(__instance.locId)))
            {
                __result = newDialogueOption[__instance.locId].dialogueText;
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
                __result = newDialogueOption[__instance.locId].dialogueText;
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
                __result = newDialogueOption[localizationId.locId].dialogueText;
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
                __result = newDialogueOption[locId].dialogueText;
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
                __result = newDialogueOption[locId].dialogueText;
                return false;
            }

            return true;
        }
    }
}