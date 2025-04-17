using Il2Cpp;
using UnityEngine;
using HarmonyLib;
using Il2CppSabotage.Graph.Core;
using MelonLoader;
using Il2CppSabotage.Imposter;
using static MelonLoader.MelonLogger;

namespace OpenSeaOfStars.Helpers
{
    public class CutsceneHelper : MelonLogger
    {
        public enum CutsceneType
        {
            None = 0,
            Story = 1,
            World = 2
        }

        private OpenSeaOfStarsMod mod;
        private CutsceneManager cutManager;
        
        private class CutscenePatchData
        {
            public List<CharacterDefinitionId> requiredCharacters;
            public bool forceAnimations;
            public Vector3 newPosition;
        }

        private static Dictionary<string, CutscenePatchData> storyCutsceneData = new()
        {
            // forbidden cavern
            { "CUT_IntroBossSlug", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}} },
            { "CUT_GiantSlugDefeated", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}, forceAnimations = true} },
            // mountain trail
            { "CUT_ElderMistBoss", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_ElderMistDefeated", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            // outpost/mines
            // { "BEH_FloorA_PingPongWindTunnel", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            // { "BEH_OutWindTunnel_FloorA", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            { "CUT_SalamanderAppears", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_FightSalamanderDone", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            { "CUT_Mines_MeetingMalkomud", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_Mines_MalkomudAfterBossFight", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_Outpost_AfterSavingVillage", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_Elevator_TheSleepingSerpent", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            // brisk
            { "CUT_StartArena", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Bst, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan}} },
            // wizard lab
            { "CUT_WizardLab_BossFight", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_BackToHub", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
        };
        private static Dictionary<string, CutscenePatchData> toDockCutsceneData = new()
        {
            { "EvermistIsland_WorldMap_Gameplay", new CutscenePatchData {newPosition = new Vector3(116.5f, 1.01f, 74.5f)} },
        };
        private static Dictionary<string, CutscenePatchData> fromDockCutsceneData = new()
        {
            { "EvermistIsland_WorldMap_Gameplay", new CutscenePatchData {newPosition = new Vector3(114.5f, 3.01f, 74.5f)} }
        };

        private static List<string> cutscenesToSkip = new()
        {
            // "BEH_ZenithElevator_GoingUp",
            // "BEH_OutWindTunnel_FloorA",
            // "BEH_OutTunnel_Right",
            // "BEH_OutTunnel_Left",
            // "CUT_ActTwoInterlude",
            // "CUT_Outpost_AfterSavingVillage"
        };
        
        private static List<CharacterDefinitionId> gameplayParty = new()
        {
            CharacterDefinitionId.Zale,
            CharacterDefinitionId.Valere,
            CharacterDefinitionId.Garl,
            CharacterDefinitionId.Serai,
            CharacterDefinitionId.Reshan,
            CharacterDefinitionId.Bst
        };
        private static Dictionary<string, (string main, string world)> characterObjectDict = new()
        {
            {CharacterDefinitionId.Zale.ToString(), ("PlayableCharacter_SunBoy(Clone)", "PlayableCharacter_WorldMapSunboy(Clone)")},
            {CharacterDefinitionId.Valere.ToString(), ("PlayableCharacter_Moongirl(Clone)", "PlayableCharacter_WorldMapMoongirl(Clone)")},
            {CharacterDefinitionId.Garl.ToString(), ("PlayableCharacter_Garl(Clone)", "PlayableCharacter_WorldMapGarl(Clone)")},
            {CharacterDefinitionId.Serai.ToString(), ("PlayableCharacter_Serai(Clone)", "PlayableCharacter_WorldMapSerai(Clone)")},
            {CharacterDefinitionId.Reshan.ToString(), ("PlayableCharacter_Reshan(Clone)", "PlayableCharacter_WorldMapReshan(Clone)")},
            {CharacterDefinitionId.Bst.ToString(), ("PlayableCharacter_Bst(Clone)", "PlayableCharacter_WorldMapBst(Clone)")}
        };
        private static List<string> characterIdleString = new List<string>();
        public static GraphControllerBase currentCutsceneGraph = null;
        public static CutsceneType currentCutsceneType = CutsceneType.None;

        private static bool keepActiveFix = false;
        private static List<CharacterDefinitionId> keepActive = new List<CharacterDefinitionId>();

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

        // removed forceload variable as you always want to load a character with this method.
        private static void loadCharacterForCutscene(CharacterDefinitionId id, bool forceStandingAnimation = false)
        {
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            if (!ppm.currentParty.Contains(id))
            {
                ppm.currentParty.Add(id);
            }

            GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
            if (partyHandler != null)
            {
                GameObject leader = partyHandler.transform.FindChild(characterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;
                GameObject follower = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;

                follower.SetActive(true);
                follower.GetComponent<PartyCharacterFollower>().enabled = true;
                follower.GetComponent<PlayerController>().enabled = true;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = true;
                follower.transform.localPosition = leader.transform.localPosition;
            }
        }

        public void skipWorldCutscene()
        {
            if (currentCutsceneGraph == null || currentCutsceneGraph.gameObject == null)
            {
                return;
            }
            
            if (currentCutsceneGraph.gameObject.name.Equals("BEH_Dock_Fast_Travel_To_Docks"))
            {
                if (toDockCutsceneData.ContainsKey(currentCutsceneGraph.gameObject.scene.name))
                {
                    currentCutsceneGraph.StopTree();
                    currentCutsceneGraph = null;
                    currentCutsceneType = CutsceneType.None;
                    
                }
            }
            else if (currentCutsceneGraph.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks"))
            {
                if (fromDockCutsceneData.ContainsKey(currentCutsceneGraph.gameObject.scene.name))
                {
                    currentCutsceneGraph.StopTree();
                    currentCutsceneGraph = null;
                    currentCutsceneType = CutsceneType.None;
                }
            }
        }

        public void checkCutsceneFinished()
        {
            if (currentCutsceneGraph != null && !currentCutsceneGraph.IsPlaying)
            {
                keepActiveFix = false;
                keepActive = new List<CharacterDefinitionId>();
                PlayerPartyManager ppm = PlayerPartyManager.Instance;

                foreach (CharacterDefinitionId id in gameplayParty)
                {
                    GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                    if (ppm.currentParty.Contains(id))
                    {
                        if (OpenSeaOfStarsMod.randomizerParty.Any(c => c.Equals(id)) && ppm.combatParty.Contains(id))
                        {
                            GameObject partychar = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;
                            partychar.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = true;
                        }
                        else
                        {
                            bool inCombat = ppm.combatParty.ToArray().Any(c => c.Equals(id));
                            bool inParty = OpenSeaOfStarsMod.randomizerParty.Any(c => c.Equals(id));
                            if (!inParty)
                            {
                                GameObject follower = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;
                                ppm.currentParty.Remove(id);
                                ppm.combatParty.Remove(id);
                                if (follower.active)
                                {
                                    if (partyHandler != null)
                                    {
                                        follower.active = false;
                                    }
                                }
                            }
                            else if (!inCombat)
                            {
                                GameObject follower = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;
                                ppm.combatParty.Remove(id);
                                if (follower.active)
                                {
                                    if (partyHandler != null)
                                    {
                                        follower.active = false;
                                    }
                                }
                            }
                        }
                    }
                }

                currentCutsceneGraph = null;
                currentCutsceneType = CutsceneType.None;
            }
            else if (keepActiveFix)
            {
                GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                bool didFix = false;
                foreach (CharacterDefinitionId id in keepActive)
                { 
                    GameObject partychar = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;
                    if (!partychar.active) 
                    { 
                        partychar.active = true; 
                        didFix = true;
                    }
                }

                if (didFix)
                {
                    keepActiveFix = false;
                }
            }
        }

        // This method is the starting point for handling the game's objects in such a way that only the intended characters show for a cutscene 
        private static void resetCharactersForCutscenes(PlayerPartyManager ppm, GameObject partyHandler, List<CharacterDefinitionId> enabledList, bool forceAnimations)
        {
            foreach (CharacterDefinitionId cutsceneChar in enabledList)
            {
                if (!ppm.combatParty.Contains(cutsceneChar))
                {
                    loadCharacterForCutscene(cutsceneChar);
                }
            }

            foreach (CharacterDefinitionId partyChar in OpenSeaOfStarsMod.randomizerParty)
            {
                if (!enabledList.Any(c => c.Equals(partyChar)))
                {
                    GameObject partyCharObj = partyHandler.transform.FindChild(characterObjectDict[partyChar.ToString()].main).gameObject;
                    if (partyCharObj != null && partyCharObj.active && ppm.combatParty.Contains(partyChar))
                    {
                        partyCharObj.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = false;
                    }
                }
            }

            if (forceAnimations)
            {
                keepActiveFix = true;
                keepActive = enabledList;
            }
        }

        [HarmonyPatch(typeof(GraphControllerBase), "StartTree")]
        private static class StartCutscenePatch
        {
            [HarmonyPrefix]
            private static void Prefix(GraphControllerBase __instance)
            {
                if (__instance == null || __instance.gameObject == null)
                {
                    return;
                }
                
                OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"CUTSCENE: {__instance.gameObject.name}");
                PlayerPartyManager ppm = PlayerPartyManager.Instance;
                if (storyCutsceneData.TryGetValue(__instance.gameObject.name, out CutscenePatchData story))
                {
                    GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                    resetCharactersForCutscenes(ppm, partyHandler, story.requiredCharacters, story.forceAnimations);
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.Story;
                }
                else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_To_Docks") && toDockCutsceneData.TryGetValue(__instance.gameObject.scene.name, out CutscenePatchData toDock))
                {
                    GameObject partyHandler = GameObject.Find("WorldMapParty(Clone)");
                    GameObject leader = partyHandler.transform.FindChild(characterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].world).gameObject;
                    leader.transform.position = toDock.newPosition;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.World;
                }
                else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks") && fromDockCutsceneData.TryGetValue(__instance.gameObject.scene.name, out CutscenePatchData fromDock))
                {
                    GameObject partyHandler = GameObject.Find("WorldMapParty(Clone)");
                    GameObject leader = partyHandler.transform.FindChild(characterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].world).gameObject;
                    leader.transform.position = fromDock.newPosition;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.World;
                }
                else if (cutscenesToSkip.Contains(__instance.gameObject.name))
                {
                    OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"Skipping {__instance.gameObject.name}");
                    __instance.SkipTree();
                }
            }
        }
    }
}
