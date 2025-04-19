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
            World = 2,
            StoryExt = 3
        }

        private OpenSeaOfStarsMod mod;
        private CutsceneManager cutManager;
        
        private class CutscenePatchData
        {
            public List<CharacterDefinitionId> requiredCharacters;
            public bool forceAnimations = false;
            public Vector3 newPosition;
            public bool isCustom = true;
            public bool hideSprite = false;
            public bool swapLeader = false;
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
            // { "BEH_FloorA_PingPongWindTunnel", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = false, extendedCutscene = true } },
            // { "BEH_OutWindTunnel_FloorA", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = false} },
            { "CUT_SalamanderAppears", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl }, isCustom = false } },
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
        private static Dictionary<string, CutscenePatchData> teleCutsceneData = new()
        {
            // mines
            { "TEL_OUT_Room01_02", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true } },
            { "TEL_OUT_Room2", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true  } },
            { "TEL_OUT_Room3", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true  } },
        };
        private static Dictionary<string, CutscenePatchData> toDockCutsceneData = new()
        {
            { "EvermistIsland_WorldMap_Gameplay", new CutscenePatchData {newPosition = new Vector3(116.5f, 1.01f, 74.5f)} },
        };
        private static Dictionary<string, CutscenePatchData> fromDockCutsceneData = new()
        {
            { "EvermistIsland_WorldMap_Gameplay", new CutscenePatchData {newPosition = new Vector3(114.5f, 3.01f, 74.5f)} }
        };
        private static List<string> startingCutscenes = new()
        {
            "BEH_FloorA_PingPongWindTunnel",
            "BEH_RightTunnel_PingPong",
            "BEH_PingPongWind_LeftTunnel"
        };
        private static List<string> endingCutscenes = new()
        {
            "BEH_OutWindTunnel_FloorA",
            "BEH_OutTunnel_Right",
            "BEH_OutTunnel_Left"
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
        public static GraphControllerBase currentCutsceneGraph = null;
        public static GraphControllerBase endingCutsceneGraph = null;
        public static bool didEndingPlay = false;
        public static bool isCustom = true;
        public static bool doSwapLeader = false;
        public static bool doHide = false;

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

        private static void hidePartyMember(CharacterDefinitionId id, GameObject partyHandler, bool hideSprite)
        {
            GameObject follower = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;
            if (hideSprite)
            {
                follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = false;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = false;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = false;
                // if (OpenSeaOfStarsMod.debug) { OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"HIDE SPRITE IN HIDE CODE: {characterObjectDict[id.ToString()].main}"); }
            }
            else
            {
                follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = true;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = true;
            }
        }

        // removed forceload variable as you always want to load a character with this method.
        private static void loadCharacterForCutscene(CharacterDefinitionId id, bool isCustom = true, bool hideSprite = false)
        {
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");

            if (!isCustom) //If it is not custom, take the character and hide the sprite
            {
                if (ppm.currentParty.Count < 3)
                {
                    ppm.AddPartyMember(id, true, true, false);
                    if (OpenSeaOfStarsMod.debug)
                    {
                        Msg("DEBUG NON CUSTOM CHARACTER SPAWNED: " + id.ToString);
                    }

                    foreach (CharacterDefinitionId charId in gameplayParty)
                    {
                        if (!OpenSeaOfStarsMod.randomizerParty.Any(c => c.Equals(charId)))
                        {
                            ppm.combatParty.Remove(charId);
                        }
                    }

                    hidePartyMember(id, partyHandler, hideSprite);
                }
            }
            else
            {
                if (!ppm.currentParty.Contains(id))
                {
                    ppm.currentParty.Add(id);
                }

                if (partyHandler != null)
                {
                    GameObject leader = partyHandler.transform.FindChild(characterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;
                    GameObject follower = partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).gameObject;

                    if (OpenSeaOfStarsMod.debug)
                    {
                        Msg("DEBUG CHARACTER SPAWNED: " + id.ToString);
                    }

                    follower.SetActive(true);
                    follower.GetComponent<PartyCharacterFollower>().enabled = true;
                    follower.GetComponent<PlayerController>().enabled = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = true;
                    follower.transform.localPosition = leader.transform.localPosition;
                }
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
            if (currentCutsceneType == CutsceneType.Story && currentCutsceneGraph != null && !currentCutsceneGraph.IsPlaying)
            {
                endStoryCutscene();
            }
            else if (currentCutsceneType == CutsceneType.StoryExt
                && currentCutsceneGraph != null && !currentCutsceneGraph.IsPlaying
                && endingCutsceneGraph != null && !endingCutsceneGraph.IsPlaying
                && didEndingPlay
            ) {
                endStoryCutscene();
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
            //TODO: Find harmony patch for this call instead of calling it on update thread
            else if (doSwapLeader && doHide)
            {
                PlayerPartyManager ppm = PlayerPartyManager.instance;
                GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                foreach (CharacterDefinitionId charId in gameplayParty)
                {
                    if (!OpenSeaOfStarsMod.randomizerParty.Any(c => c.Equals(charId)) && ppm.currentParty.Contains(charId))
                    {
                        ppm.combatParty.Remove(charId);
                        hidePartyMember(charId, partyHandler, true);
                    }
                }
            }
        }

        private void endStoryCutscene()
        {
            keepActiveFix = false;
            keepActive = new List<CharacterDefinitionId>();
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
            Vector3 pos = partyHandler.transform.FindChild(characterObjectDict[ppm.currentParty[0].ToString()].main).transform.position;

            foreach (CharacterDefinitionId id in gameplayParty)
            {
                if (isCustom)
                {
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
                else
                {
                    if (OpenSeaOfStarsMod.randomizerParty.Any(c => c.Equals(id)))
                    {
                        if (ppm.currentParty.Contains(id))
                        {
                            ppm.RemovePartyMember(id, true, false, false);
                        }
                        ppm.AddPartyMember(id, ppm.combatParty.Count < 3, ppm.combatParty.Count < 3, ppm.combatParty.Count < 3);
                        partyHandler.transform.FindChild(characterObjectDict[id.ToString()].main).transform.position = pos;
                    }
                    else
                    {
                        if (ppm.currentParty.Contains(id))
                        {
                            ppm.RemovePartyMember(id, true, false, false);
                        }
                    }
                }
            }
            if (doSwapLeader)
            {
                ppm.SetLeader(OpenSeaOfStarsMod.randomizerParty[0]);
                ppm.SetupParty(true);
                doSwapLeader = false;

                GameObject leader = partyHandler.transform.FindChild(characterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;

                if (ppm.combatParty.Count > 1)
                {
                    GameObject follower = partyHandler.transform.FindChild(characterObjectDict[ppm.combatParty[1].characterId].main).gameObject;
                    follower.GetComponent<PartyCharacterFollower>().FollowTarget(leader.GetComponent<FollowerLeader>(), true, true);  
                }
                if (ppm.combatParty.Count > 2)
                {
                    GameObject follower = partyHandler.transform.FindChild(characterObjectDict[ppm.combatParty[2].characterId].main).gameObject;
                    follower.GetComponent<PartyCharacterFollower>().FollowTarget(leader.GetComponent<FollowerLeader>(), true, true);
                }

                //set rpg camera
                setRPGCameraToLeader(ppm, partyHandler, OpenSeaOfStarsMod.randomizerParty[0]);

                if (OpenSeaOfStarsMod.debug)
                {
                    Msg("LEADER SWAPPED BACK: " + characterObjectDict[OpenSeaOfStarsMod.randomizerParty[0].ToString()].main);
                }
            }

            currentCutsceneGraph = null;
            currentCutsceneType = CutsceneType.None;
            endingCutsceneGraph = null;
            didEndingPlay = false;
            doHide = false;
            isCustom = true;
        }

        private static void setRPGCameraToLeader(PlayerPartyManager ppm, GameObject partyHandler, CharacterDefinitionId leadId)
        {
            GameObject cameraObject = GameObject.Find("RpgCameraRig");
            if (cameraObject != null)
            {
                CharacterViewCameraContext context = cameraObject.GetComponent<CameraBehaviour>().currentContext.TryCast<CharacterViewCameraContext>();
                if (context != null)
                {
                    GameObject leader = partyHandler.transform.FindChild(characterObjectDict[leadId.ToString()].main).gameObject;
                    context.player = leader.transform;
                    context.cameraLookAtPosition = leader.GetComponent<PlayerCameraLookAtPosition>();
                }
            }
        }

        // This method is the starting point for handling the game's objects in such a way that only the intended characters show for a cutscene 
        private static void resetCharactersForCutscenes(PlayerPartyManager ppm, GameObject partyHandler, List<CharacterDefinitionId> enabledList, bool forceAnimations, bool isCustomCode = true, bool hideSprite = false)
        {

            if (isCustomCode)
            {
                foreach (CharacterDefinitionId partyChar in OpenSeaOfStarsMod.randomizerParty)
                {
                    if (!enabledList.Any(c => c.Equals(partyChar)))
                    {
                        GameObject partyCharObj = partyHandler.transform.FindChild(characterObjectDict[partyChar.ToString()].main).gameObject;
                        if (partyCharObj != null && partyCharObj.active && ppm.combatParty.Contains(partyChar))
                        {
                            partyCharObj.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = false;
                            if (OpenSeaOfStarsMod.debug) { OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"HIDE SPRITE IN CUSTOM CODE: {characterObjectDict[partyChar.ToString()].main}"); }
                        }
                    }
                }
            }
            else
            {
                foreach (CharacterDefinitionId partyChar in OpenSeaOfStarsMod.randomizerParty)
                {
                    if (!enabledList.Any(c => c.Equals(partyChar)))
                    {
                        ppm.RemovePartyMember(partyChar, true, false, false);
                    }
                }
            }

            foreach (CharacterDefinitionId cutsceneChar in enabledList)
            {
                if (!ppm.combatParty.Contains(cutsceneChar))
                {
                    loadCharacterForCutscene(cutsceneChar, isCustomCode, hideSprite);
                }
            }

            if (forceAnimations)
            {
                keepActiveFix = true;
                keepActive = enabledList;
            }

            isCustom = isCustomCode;
        }

        [HarmonyPatch(typeof(Teleporter), "OnPlayerEntersZone")]
        private static class teleCollisionPatch
        {
            private static void Prefix(Teleporter __instance)
            {
                if (__instance == null || __instance.gameObject == null)
                {
                    return;
                }

                if (__instance != null && OpenSeaOfStarsMod.debug)
                {
                    if (__instance.transform.parent != null && __instance.transform.parent.parent != null && __instance.transform.parent.parent.gameObject != null)
                    {
                        OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"TELE PLAYER: {__instance.transform.parent.parent.gameObject.name}/{__instance.transform.parent.gameObject.name}/{__instance.gameObject.name}");
                    }
                    else if (__instance.transform.parent != null && __instance.transform.parent.gameObject != null)
                    {
                        OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"TELE PLAYER: {__instance.transform.parent.gameObject.name}/{__instance.gameObject.name}");
                    }
                    else
                    {
                        OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"TELE PLAYER: {__instance.gameObject.name}");
                    }
                }

                if (currentCutsceneType == CutsceneType.None)
                {
                    PlayerPartyManager ppm = PlayerPartyManager.Instance;
                    if (teleCutsceneData.TryGetValue(__instance.gameObject.name, out CutscenePatchData tele))
                    {
                        GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                        resetCharactersForCutscenes(ppm, partyHandler, tele.requiredCharacters, tele.forceAnimations, tele.isCustom, tele.hideSprite);
                        currentCutsceneType = CutsceneType.StoryExt;

                        if (tele.swapLeader)
                        {
                            ppm.SetLeader(tele.requiredCharacters[0]);
                            ppm.SetupParty(true);
                            doSwapLeader = true;
                            doHide = tele.hideSprite;

                            foreach (CharacterDefinitionId charId in gameplayParty)
                            {
                                if (!OpenSeaOfStarsMod.randomizerParty.Any(c => c.Equals(charId)) && ppm.currentParty.Contains(charId))
                                {
                                    ppm.combatParty.Remove(charId);
                                    hidePartyMember(charId, partyHandler, tele.hideSprite);
                                }
                            }

                            if (OpenSeaOfStarsMod.debug)
                            {
                                Msg("LEADER SWAPPED: " + characterObjectDict[tele.requiredCharacters[0].ToString()].main);
                            }
                        }
                    }
                }
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
                    resetCharactersForCutscenes(ppm, partyHandler, story.requiredCharacters, story.forceAnimations, story.isCustom, story.hideSprite);
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

            [HarmonyPostfix]
            private static void Postfix(GraphControllerBase __instance)
            {
                if (currentCutsceneType == CutsceneType.StoryExt && startingCutscenes.Contains(__instance.gameObject.name))
                {
                    currentCutsceneGraph = __instance;
                }
                if (currentCutsceneType == CutsceneType.StoryExt && endingCutscenes.Contains(__instance.gameObject.name))
                {
                    endingCutsceneGraph = __instance;
                    didEndingPlay = true;
                }
            }
        }
    }
}
