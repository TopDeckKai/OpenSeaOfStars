using Il2Cpp;
using UnityEngine;
using Il2CppSabotage.Blackboard;
using Il2CppSabotage.Localization;
using Il2CppSabotage.Graph.BehaviorTree.Nodes.Leaf.Blackboard;
using HarmonyLib;
using Il2CppSabotage.Graph.Core;
using MelonLoader;
using static UnityEngine.UIElements.StyleVariableResolver;
using Il2CppSabotage.Imposter;

namespace OpenSeaOfStars.Helpers
{
    public class CutsceneHelper : MelonLogger
    {
        private OpenSeaOfStarsMod mod;
        private CutsceneManager cutManager;
        internal static CharacterDefinitionId[] gameplayParty = new CharacterDefinitionId[] { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan, CharacterDefinitionId.Bst };
        
        // TODO: Refactor to use game object.find potentially instead of this, using the dictonary for the gameobject's name
        private Dictionary<CharacterDefinitionId, int> capsulePartyIndexes = new Dictionary<CharacterDefinitionId, int>()
        {
            { CharacterDefinitionId.Zale, 0 }, { CharacterDefinitionId.Valere, 1 },{ CharacterDefinitionId.Garl, 2 }, { CharacterDefinitionId.Serai, 3 }, { CharacterDefinitionId.Reshan, 4 }, { CharacterDefinitionId.Bst, 5 }
        };

        public CutsceneHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
        }

        internal void initCutsceneManager(bool debug)
        {
            try
            {
                cutManager = CutsceneManager.Instance;
                mod.LoggerInstance.Msg($"CutsceneManager Found!!");
            }
            catch
            {
                mod.LoggerInstance.Msg($"Unable to find CutsceneManager");
            }
        }

        [HarmonyPatch(typeof(GraphControllerBase), "StartTree")]
        private static class StartCutscenePatch
        {
            [HarmonyPostfix]
            private static void Prefix(GraphControllerBase __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    if (__instance.gameObject.name.Equals("CUT_IntroBossSlug")
                        || __instance.gameObject.name.Equals("CUT_GiantSlugDefeated")
                    )
                    {
                        CharacterDefinitionId id = CharacterDefinitionId.Valere;
                        PlayerPartyManager ppm = PlayerPartyManager.Instance;

                        ppm.currentParty.Add(id);
                        ppm.AddToCombatParty(id);
                        CharacterDefinitionId.FirstFollower = id;
                        GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");

                        // TODO refactor into static method that works with all characters

                        if (partyHandler != null)
                        {
                            Msg("PartyFound! Game Objects: " + partyHandler.transform.childCount);
                            GameObject sunboy = partyHandler.transform.GetChild(0).gameObject;
                            GameObject moongirl = partyHandler.transform.GetChild(1).gameObject;
                            moongirl.SetActive(true);
                            moongirl.GetComponent<PartyCharacterFollower>().enabled = true;
                            moongirl.GetComponent<PlayerController>().enabled = true;
                            moongirl.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                            moongirl.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = true;
                            moongirl.transform.localPosition = sunboy.transform.localPosition;
                            //moongirl.GetComponent<PartyCharacterFollower>().FollowTarget(sunboy.GetComponent<FollowerLeader>(), true, true);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GraphControllerBase), "StopTree")]
        private static class StopCutscenePatch
        {
            [HarmonyPostfix]
            private static void Prefix(GraphControllerBase __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    if (__instance.gameObject.name.Equals("CUT_IntroBossSlug")
                        || __instance.gameObject.name.Equals("CUT_GiantSlugDefeated")
                    )
                    {
                        PlayerPartyManager ppm = PlayerPartyManager.Instance;

                        foreach (CharacterDefinitionId id in gameplayParty)
                        {
                            Msg("Checking character removal for " + id.ToString() + ": " + ppm.currentParty.Contains(id) + "  " + !OpenSeaOfStarsMod.randomizerParty.Contains(id));

                            if (ppm.currentParty.Contains(id) && !OpenSeaOfStarsMod.randomizerParty.Contains(id))
                            {
                                ppm.currentParty.Remove(id);
                                ppm.combatParty.Remove(id);
                                GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                                if (partyHandler != null)
                                {
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
