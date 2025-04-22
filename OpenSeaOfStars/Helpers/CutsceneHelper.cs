using Il2Cpp;
using UnityEngine;
using HarmonyLib;
using Il2CppSabotage.Graph.Core;
using MelonLoader;
using Il2CppSabotage.Imposter;
using static MelonLoader.MelonLogger;
using static OpenSeaOfStars.OpenSeaOfStarsMod;

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
            public enum RequiredCharacterType
            {
                ANY,
                ALL
            }
            public List<CharacterDefinitionId>? cutsceneCharacters;
            public List<CharacterDefinitionId>? requiredCharacters;
            public List<CharacterDefinitionId>? backupCharacters;
            public RequiredCharacterType requiredCharacterType;
            public bool forceAnimations;
            public Vector3 newPosition;
            public bool isCustom = true;
            public bool hideSprite;
            public bool swapLeader;
            public Action? onCutsceneStart;
            public Action? onCutsceneEnd;
        }

        private static Dictionary<string, CutscenePatchData> storyCutsceneData = new()
        {
            // Evermist Island
            { "BEH_ZenithElevator_GoingUp", new CutscenePatchData
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere},
                    requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere},
                    backupCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Garl, CharacterDefinitionId.Serai},
                    requiredCharacterType = CutscenePatchData.RequiredCharacterType.ANY,
                    isCustom = false,
                    forceAnimations = true
                }
            },
            { "BEH_ZenithElevator_GoingDown", new CutscenePatchData
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere},
                    requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere},
                    backupCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Garl, CharacterDefinitionId.Serai},
                    requiredCharacterType = CutscenePatchData.RequiredCharacterType.ANY,
                    isCustom = false
                }
            },
            // forbidden cavern
            { "CUT_IntroBossSlug", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}} },
            { "CUT_GiantSlugDefeated", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}, forceAnimations = true} },
            // mountain trail
            { "CUT_ElderMistBoss", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_ElderMistDefeated", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            { "CUT_YeetThrow", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}, isCustom = false} },
            // Sleeper Island
            { "CUT_XtolThrow", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}, isCustom = false} },
            // outpost/mines
            { "CUT_SalamanderAppears", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl }, isCustom = false } },
            { "CUT_FightSalamanderDone", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            { "CUT_Mines_MeetingMalkomud", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_Mines_MalkomudAfterBossFight", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_Outpost_AfterSavingVillage", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_Elevator_TheSleepingSerpent", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            // brisk
            // { "CUT_StartArena", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Bst, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan}} },
            // { "CUT_BronzeIntro", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Bst, CharacterDefinitionId.Valere, CharacterDefinitionId.Reshan}} },
            // wizard lab
            { "CUT_WizardLab_BossFight", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false} },
            { "CUT_BackToHub", new CutscenePatchData {requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
        };
        private static Dictionary<string, CutscenePatchData> teleCutsceneData = new()
        {
            // mines
            { "TEL_OUT_Room01_02", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true} },
            { "TEL_OUT_Room2", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true} },
            { "TEL_OUT_Room3", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true} },
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
        private static Dictionary<string, CutscenePatchData> cutscenesToSkip = new()
        {
            { "CUT_KeenathanRejoin", new CutscenePatchData {onCutsceneEnd = () => {
                MelonCoroutines.Start(removeKeenathan());
                return;

                System.Collections.IEnumerator removeKeenathan()
                {
                    yield return null;
                    PlayerPartyManager.Instance.RemoveCargoCharacter(CharacterDefinitionId.Keenathan);
                }
            }} },
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

        private static GraphControllerBase? currentCutsceneGraph;
        private static GraphControllerBase? endingCutsceneGraph;
        private static bool didEndingPlay;
        private static bool isCustom = true;
        private static bool doSwapLeader;
        private static bool doHide;

        public static CutsceneType currentCutsceneType = CutsceneType.None;
        private static Action? currentCutsceneCallback;

        private static bool keepActiveFix;
        private static List<CharacterDefinitionId> keepActive;

        public CutsceneHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
        }

        internal void initCutsceneManager()
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

        /// <summary>
        /// Helper function to prevent stale cutscene data when returning to menu
        /// </summary>
        public void ClearAllCutsceneData()
        {
            currentCutsceneGraph = null;
            endingCutsceneGraph = null;
            didEndingPlay = false;
            isCustom = true;
            doSwapLeader = false;
            doHide = false;
            currentCutsceneType = CutsceneType.None;
            currentCutsceneCallback = null;
            keepActiveFix = false;
            keepActive = null;
        }

        private static void hidePartyMember(CharacterDefinitionId id, GameObject partyHandler, bool hideSprite)
        {
            GameObject follower = partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).gameObject;
            if (hideSprite)
            {
                follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = false;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.SetActive(false);
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = false;
                // if (OpenSeaOfStarsMod.debug) { OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"HIDE SPRITE IN HIDE CODE: {characterObjectDict[id.ToString()].main}"); }
            }
            else
            {
                follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.SetActive(true);
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
                    partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).position = ppm.leader.gameObject.transform.position;
                    #if DEBUG
                    OpenInstance.LoggerInstance.Msg("DEBUG NON CUSTOM CHARACTER SPAWNED: " + id.ToString());
                    #endif

                    foreach (CharacterDefinitionId charId in gameplayParty)
                    {
                        if (!RandomizerParty.Any(c => c.Equals(charId)))
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
                    GameObject leader = partyHandler.transform.FindChild(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;
                    GameObject follower = partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).gameObject;

                    #if DEBUG
                    OpenInstance.LoggerInstance.Msg("DEBUG CHARACTER SPAWNED: " + id.ToString());
                    #endif

                    follower.SetActive(true);
                    follower.GetComponent<PartyCharacterFollower>().enabled = true;
                    follower.GetComponent<PlayerController>().enabled = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.SetActive(true);
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
                    currentCutsceneCallback?.Invoke();
                    currentCutsceneCallback = null;
                    currentCutsceneGraph = null;
                    currentCutsceneType = CutsceneType.None;
                    
                }
            }
            else if (currentCutsceneGraph.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks"))
            {
                if (fromDockCutsceneData.ContainsKey(currentCutsceneGraph.gameObject.scene.name))
                {
                    currentCutsceneGraph.StopTree();
                    currentCutsceneCallback?.Invoke();
                    currentCutsceneCallback = null;
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
                foreach (CharacterDefinitionId id in keepActive)
                {
                    GameObject partychar = partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).gameObject;
                    if (!partychar.activeSelf)
                    {
                        partychar.SetActive(true);
                    }
                }
            }
            //TODO: Find harmony patch for this call instead of calling it on update thread
            else if (doSwapLeader && doHide)
            {
                PlayerPartyManager ppm = PlayerPartyManager.instance;
                GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                foreach (CharacterDefinitionId charId in gameplayParty)
                {
                    if (!RandomizerParty.Any(c => c.Equals(charId)) && ppm.currentParty.Contains(charId))
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
            Vector3 pos = partyHandler.transform.FindChild(CharacterObjectDict[ppm.currentParty.ToArray()[0].ToString()].main).transform.position;

            foreach (CharacterDefinitionId id in gameplayParty)
            {
                if (isCustom)
                {
                    if (ppm.currentParty.Contains(id))
                    {
                        if (RandomizerParty.Any(c => c.Equals(id)) && ppm.combatParty.Contains(id))
                        {
                            GameObject partychar = partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).gameObject;
                            partychar.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.SetActive(true);
                        }
                        else
                        {
                            bool inCombat = ppm.combatParty.ToArray().Any(c => c.Equals(id));
                            bool inParty = RandomizerParty.Any(c => c.Equals(id));
                            if (!inParty)
                            {
                                GameObject follower = partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).gameObject;
                                ppm.currentParty.Remove(id);
                                ppm.combatParty.Remove(id);
                                if (follower.activeSelf && partyHandler != null)
                                {
                                    follower.SetActive(false);
                                }
                            }
                            else if (!inCombat)
                            {
                                GameObject follower = partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).gameObject;
                                ppm.combatParty.Remove(id);
                                if (follower.activeSelf && partyHandler != null)
                                {
                                    follower.SetActive(false);
                                }
                            }
                        }
                    }
                } 
                else
                {
                    if (RandomizerParty.Any(c => c.Equals(id)))
                    {
                        if (ppm.currentParty.Contains(id))
                        {
                            ppm.RemovePartyMember(id, true, false, false);
                        }
                        ppm.AddPartyMember(id, ppm.currentParty.Count < 3, ppm.currentParty.Count < 3, ppm.currentParty.Count < 3);
                        partyHandler.transform.FindChild(CharacterObjectDict[id.ToString()].main).transform.position = pos;
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
                ppm.SetLeader(RandomizerParty[0]);
                ppm.SetupParty(true);
                doSwapLeader = false;

                GameObject leader = partyHandler.transform.FindChild(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;

                if (ppm.combatParty.Count > 1)
                {
                    GameObject follower = partyHandler.transform.FindChild(CharacterObjectDict[ppm.combatParty.ToArray()[1].characterId].main).gameObject;
                    follower.GetComponent<PartyCharacterFollower>().FollowTarget(leader.GetComponent<FollowerLeader>(), true, true);  
                }
                if (ppm.combatParty.Count > 2)
                {
                    GameObject follower = partyHandler.transform.FindChild(CharacterObjectDict[ppm.combatParty.ToArray()[2].characterId].main).gameObject;
                    follower.GetComponent<PartyCharacterFollower>().FollowTarget(leader.GetComponent<FollowerLeader>(), true, true);
                }

                //set rpg camera
                setRPGCameraToLeader(ppm, partyHandler, RandomizerParty[0]);

                #if DEBUG
                Msg("LEADER SWAPPED BACK: " + CharacterObjectDict[RandomizerParty[0].ToString()].main);
                #endif
            }

            currentCutsceneCallback?.Invoke();
            currentCutsceneCallback = null;
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
                    GameObject leader = partyHandler.transform.FindChild(CharacterObjectDict[leadId.ToString()].main).gameObject;
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
                foreach (CharacterDefinitionId partyChar in RandomizerParty)
                {
                    if (!enabledList.Any(c => c.Equals(partyChar)))
                    {
                        GameObject partyCharObj = partyHandler.transform.FindChild(CharacterObjectDict[partyChar.ToString()].main).gameObject;
                        if (partyCharObj != null && partyCharObj.activeSelf && ppm.combatParty.Contains(partyChar))
                        {
                            partyCharObj.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.SetActive(false);
                            #if DEBUG 
                            OpenInstance.LoggerInstance.Msg($"HIDE SPRITE IN CUSTOM CODE: {CharacterObjectDict[partyChar.ToString()].main}");
                            #endif
                        }
                    }
                }
            }
            else
            {
                foreach (CharacterDefinitionId partyChar in RandomizerParty)
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

                #if DEBUG
                if (__instance != null)
                {
                    if (__instance.transform.parent != null && __instance.transform.parent.parent != null && __instance.transform.parent.parent.gameObject != null)
                    {
                        OpenInstance.LoggerInstance.Msg($"TELE PLAYER: {__instance.transform.parent.parent.gameObject.name}/{__instance.transform.parent.gameObject.name}/{__instance.gameObject.name}");
                    }
                    else if (__instance.transform.parent != null && __instance.transform.parent.gameObject != null)
                    {
                        OpenInstance.LoggerInstance.Msg($"TELE PLAYER: {__instance.transform.parent.gameObject.name}/{__instance.gameObject.name}");
                    }
                    else
                    {
                        OpenInstance.LoggerInstance.Msg($"TELE PLAYER: {__instance.gameObject.name}");
                    }
                }
                #endif

                if (currentCutsceneType == CutsceneType.None)
                {
                    PlayerPartyManager ppm = PlayerPartyManager.Instance;
                    if (teleCutsceneData.TryGetValue(__instance.gameObject.name, out CutscenePatchData tele))
                    {
                        GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                        resetCharactersForCutscenes(ppm, partyHandler, tele.cutsceneCharacters, tele.forceAnimations, tele.isCustom, tele.hideSprite);
                        currentCutsceneType = CutsceneType.StoryExt;

                        if (tele.swapLeader)
                        {
                            ppm.SetLeader(tele.cutsceneCharacters[0]);
                            ppm.SetupParty(true);
                            doSwapLeader = true;
                            doHide = tele.hideSprite;

                            foreach (CharacterDefinitionId charId in gameplayParty)
                            {
                                if (!RandomizerParty.Any(c => c.Equals(charId)) && ppm.currentParty.Contains(charId))
                                {
                                    ppm.combatParty.Remove(charId);
                                    hidePartyMember(charId, partyHandler, tele.hideSprite);
                                }
                            }

                            #if DEBUG
                            OpenInstance.LoggerInstance.Msg("LEADER SWAPPED: " + CharacterObjectDict[tele.cutsceneCharacters[0].ToString()].main);
                            #endif
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
                
                OpenInstance.LoggerInstance.Msg($"CUTSCENE: {__instance.gameObject.name}");
                PlayerPartyManager ppm = PlayerPartyManager.Instance;
                if (storyCutsceneData.TryGetValue(__instance.gameObject.name, out CutscenePatchData data))
                {
                    List<CharacterDefinitionId> chars = data.cutsceneCharacters;
                    if (data.requiredCharacters?.Count > 0)
                    {
                        IEnumerable<CharacterDefinitionId> intersect = data.requiredCharacters.IntersectBy(ppm.currentParty.ToArray().Select(c1 => c1.ToString()), c2 => c2.ToString());
                        if (intersect.ToList().Count == 0 && ppm.currentParty.Count < 2)
                        {
                            chars = data.backupCharacters;
                        }
                    }
                    GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                    resetCharactersForCutscenes(ppm, partyHandler, chars, data.forceAnimations, data.isCustom, data.hideSprite);
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.Story;
                    data.onCutsceneStart?.Invoke();
                }
                else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_To_Docks") && toDockCutsceneData.TryGetValue(__instance.gameObject.scene.name, out data))
                {
                    GameObject partyHandler = GameObject.Find("WorldMapParty(Clone)");
                    GameObject leader = partyHandler.transform.FindChild(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].world).gameObject;
                    leader.transform.position = data.newPosition;
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.World;
                    data.onCutsceneStart?.Invoke();
                }
                else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks") && fromDockCutsceneData.TryGetValue(__instance.gameObject.scene.name, out data))
                {
                    GameObject partyHandler = GameObject.Find("WorldMapParty(Clone)");
                    GameObject leader = partyHandler.transform.FindChild(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].world).gameObject;
                    leader.transform.position = data.newPosition;
                    data.onCutsceneStart?.Invoke();
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.World;
                }
                else if (cutscenesToSkip.TryGetValue(__instance.gameObject.name, out data))
                {
                    OpenInstance.LoggerInstance.Msg($"Skipping {__instance.gameObject.name}");
                    data.onCutsceneStart?.Invoke();
                    __instance.SkipTree();
                    data.onCutsceneEnd?.Invoke();
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
