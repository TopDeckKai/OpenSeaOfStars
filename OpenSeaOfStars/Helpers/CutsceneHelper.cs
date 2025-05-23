﻿using Il2Cpp;
using UnityEngine;
using HarmonyLib;
using Il2CppSabotage.Graph.Core;
using MelonLoader;
using Il2CppSabotage.Imposter;
using static OpenSeaOfStars.OpenSeaOfStarsMod;

namespace OpenSeaOfStars.Helpers
{
    public class CutsceneHelper
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
        
        public class CutscenePatchData
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
            public Action? onCutsceneAboutToStart;
            public Action? onCutsceneStart;
            public Action? onCutsceneAboutToEnd;
            public Action? onCutsceneEnd;
        }

        #region Cutscene Patch Data
        public static Dictionary<string, CutscenePatchData> storyCutsceneData = new()
        {
            // Evermist Island
            { "BEH_ZenithElevator_GoingUp", new CutscenePatchData
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere},
                    requiredCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere},
                    backupCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Garl, CharacterDefinitionId.Serai},
                    requiredCharacterType = CutscenePatchData.RequiredCharacterType.ANY,
                    isCustom = false
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
            // { "CUT_StartArena", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Bst, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan}} },
            // { "CUT_BronzeIntro", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Bst, CharacterDefinitionId.Valere, CharacterDefinitionId.Reshan}} },
            // wizard lab
            { "CUT_WizardLab_BossFight", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false} },
            { "CUT_BackToHub", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, forceAnimations = true} },
            { "CUT_StormCaller_BeforeBossFight", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai}, isCustom = false, forceAnimations = true} },
            { "CUT_StormCaller_AfterBossFight", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai }, isCustom = false, forceAnimations = true, onCutsceneEnd = () => {
                OpenSeaOfStarsMod.OpenInstance.LevelHelper.loadLevel("StormCallerIslandDefinition");
                return;
            } } },
            // Wraith Island
            // Lucent
            { "CUT_Lucent_IntroInn", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}} },
            { "CUT_QuizIntro", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai}, isCustom = false} },
            { "IntroCutscene", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai}, isCustom = false} },
            { "CUT_LeDukeBossFight", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai}} },
            { "CUT_LeDukeAfterBattle", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale}, forceAnimations = true} },
            { "CUT_NecromancerLair_MeetRomaya", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai}} },
            { "CUT_NecromancerLair_RomayaDefeated", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai}, forceAnimations = true} },
            { "CUT_HauntedMansion_Sandwitch_Quest_Start", new CutscenePatchData 
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl, CharacterDefinitionId.Serai},
                    onCutsceneEnd = () => { storyCutsceneData.Remove("CUT_HauntedMansion_Sandwitch_Quest_Start"); }
                }
            },
            { "CUT_HauntedMansion_Sandwitch_Quest_Kitchen", new CutscenePatchData
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl, CharacterDefinitionId.Serai},
                    onCutsceneEnd = () => { PlayerPartyManager.Instance.SetShelvedParty(new Il2CppSystem.Collections.Generic.List<CharacterDefinitionId>()); }
                }
            },
            // { "BEH_IntroBotanicalHorror", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false} },
            { "CUT_HauntedMansion_BotanicalDefeated", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}, forceAnimations = true} },
            { "CUT_HauntedMansion_BreakingDoorSeal", new CutscenePatchData 
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl},
                    onCutsceneAboutToStart = () => {
                        ShelvedParty.Clear();
                        ShelvedParty.AddRange(RandomizerParty);
                        List<CharacterDefinitionId> reqChars = new() { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere };
                        PlayerPartyManager ppm = PlayerPartyManager.Instance;
                        foreach (CharacterDefinitionId c in RandomizerParty)
                        {
                            ppm.RemovePartyMember(c, true, false, false);
                        }
                        foreach (CharacterDefinitionId c in reqChars)
                        {
                            ppm.AddPartyMember(c, true, true, true);
                        }
                        RandomizerParty.Clear();
                        RandomizerParty.AddRange(reqChars);
                    },
                    onCutsceneStart = () => {
                        GameObject party = GameObject.Find("CapsuleParty(Clone)");
                        party.transform.Find(CharacterObjectDict[CharacterDefinitionId.Garl.ToString()].main).gameObject.SetActive(true);
                    },
                    onCutsceneEnd = () => {
                        GameObject party = GameObject.Find("CapsuleParty(Clone)");
                        party.transform.Find(CharacterObjectDict[CharacterDefinitionId.Zale.ToString()].main).gameObject.SetActive(true);
                        party.transform.Find(CharacterObjectDict[CharacterDefinitionId.Valere.ToString()].main).gameObject.SetActive(true);
                    }
                }
            },
            { "CUT_DwellerDefeated", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere/*, CharacterDefinitionId.MasterMoraine*/}} },
            { "CUT_Lucent_AfterHauntedMansion", new CutscenePatchData 
                {
                    cutsceneCharacters = new List<CharacterDefinitionId> { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl },
                    isCustom = false,
                    onCutsceneAboutToStart = () => {
                        PlayerPartyManager ppm = PlayerPartyManager.Instance;
                        foreach (CharacterDefinitionId c in RandomizerParty)
                        {
                            ppm.RemovePartyMember(c, true, false, false);
                        }
                        foreach (CharacterDefinitionId c in ShelvedParty)
                        {
                            ppm.AddPartyMember(c, false, true, true);
                        }
                        RandomizerParty.Clear();
                        RandomizerParty.AddRange(ShelvedParty);
                        ShelvedParty.Clear();
                    },
                    onCutsceneStart = () => {
                        GameObject party = GameObject.Find("CapsuleParty(Clone)");
                        party.transform.Find(CharacterObjectDict[CharacterDefinitionId.Zale.ToString()].main).gameObject.SetActive(true);
                        party.transform.Find(CharacterObjectDict[CharacterDefinitionId.Valere.ToString()].main).gameObject.SetActive(true);
                        party.transform.Find(CharacterObjectDict[CharacterDefinitionId.Garl.ToString()].main).gameObject.SetActive(true);
                    },
                    onCutsceneEnd = () => {
                        OpenInstance.BlackboardHelper.AddBlackboardValue("20f78bdeb6ac19b4a9a144c9fd6149d7", 0);
                        // turn off Yolande until above bvar moves her
                        GameObject.Find("NPC_Cutscene/PIRATES_CREW/NPC_Yolande")?.SetActive(false);
                        
                        MelonCoroutines.Start(FixParty());
                        return;

                        System.Collections.IEnumerator FixParty()
                        {
                            yield return null;
                            PlayerPartyManager.instance.SetupParty(true);
                        }
                    }
                }
            },
            // Romaya
            { "BEH_Elevator_Up", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere}} },
            { "CUT_FloodedGraveyard_DukeWander", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan, CharacterDefinitionId.Bst}} },
            { "Cut_romayaV2_Intro", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan, CharacterDefinitionId.Bst}} },
            { "Cut_romayaV2_Outro", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Serai, CharacterDefinitionId.Reshan, CharacterDefinitionId.Bst}, forceAnimations = true} },
        };
        private static Dictionary<string, CutscenePatchData> teleCutsceneData = new()
        {
            // mines
            { "TEL_OUT_Room01_02", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true} },
            { "TEL_OUT_Room2", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true} },
            { "TEL_OUT_Room3", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false, swapLeader = true} },
            // haunted mansion
            // { "TEL_IN_BotanicRoom", new CutscenePatchData {cutsceneCharacters = new List<CharacterDefinitionId> {CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl}, isCustom = false, hideSprite = false} },
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
            "BEH_PingPongWind_LeftTunnel",
            // "BEH_IntroBotanicalHorror",
        };
        private static List<string> endingCutscenes = new()
        {
            "BEH_OutWindTunnel_FloorA",
            "BEH_OutTunnel_Right",
            "BEH_OutTunnel_Left",
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
            { "CUT_FloodedGraveyardPath", new CutscenePatchData() },
            { "CUT_SunkenTowerPath", new CutscenePatchData() },
        };
        private static Dictionary<string, CutscenePatchData> callbackCutscenes = new()
        {
            { "CUT_HauntedMansion_GarlPrepareSnack", new CutscenePatchData {onCutsceneEnd = () => {
                PlayerPartyManager.instance.SetShelvedParty(new Il2CppSystem.Collections.Generic.List<CharacterDefinitionId>());
                GameObject.Find("NPC_Garl").SetActive(false);
            }} },
        };
        private static Dictionary<string, CutscenePatchData> cutscenesToCancel = new()
        {
            { "BEH_GetOut", new CutscenePatchData() },
            { "BEH_IntroBotanicalHorror", new CutscenePatchData {onCutsceneEnd = () => {
                if (OpenInstance.BlackboardHelper.GetBlackboardValue("a1b83bdc7debc3548b900b21af499958", out int value) && value == 1)
                {
                    return;
                }
                
                Transform leader = GameObject.Find("CapsuleParty(Clone)").transform.Find(CharacterObjectDict[PlayerPartyManager.instance.LeaderID.ToString()].main);
                Vector3 newPos = leader.position;
                newPos.z = 271;
                leader.position = newPos;
            }} },
        };
        #endregion

        public static readonly List<CharacterDefinitionId> gameplayParty = new()
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
        private static Action? currentCutsceneAboutToEndCallback;
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
            currentCutsceneAboutToEndCallback = null;
            currentCutsceneCallback = null;
            keepActiveFix = false;
            keepActive = null;
        }

        public void PrintCutsceneData()
        {
            mod.LoggerInstance.Msg($@"
                currentCutsceneGraph: {(currentCutsceneGraph != null ? currentCutsceneGraph.name : "null")}
                endingCutsceneGraph: {(endingCutsceneGraph != null ? endingCutsceneGraph.name : "null")}
                didEndingPlay: {didEndingPlay}
                isCustom: {isCustom}
                doSwapLeader: {doSwapLeader}
                doHide: {doHide}
                currentCutsceneType: {currentCutsceneType}
                currentAboutToEndCallback: {currentCutsceneAboutToEndCallback != null}
                currentCallback: {currentCutsceneCallback != null}
                keepActiveFix: {keepActiveFix}
                keepActive: {(keepActive != null ? keepActive.Count : "null")}
            ");
        }

        private static void hidePartyMember(CharacterDefinitionId id, GameObject partyHandler, bool hideSprite)
        {
            GameObject follower = partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).gameObject;
            if (hideSprite)
            {
                follower.transform.Find("CharacterOffset").Find("Character").GetComponent<Animator>().enabled = false;
                follower.transform.Find("CharacterOffset").Find("Character").Find("Sprite").gameObject.SetActive(false);
                follower.transform.Find("CharacterOffset").Find("Character").Find("Sprite").GetComponent<CharacterVisual>().enabled = false;
                // if (OpenSeaOfStarsMod.debug) { OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"HIDE SPRITE IN HIDE CODE: {characterObjectDict[id.ToString()].main}"); }
            }
            else
            {
                follower.transform.Find("CharacterOffset").Find("Character").GetComponent<Animator>().enabled = true;
                follower.transform.Find("CharacterOffset").Find("Character").Find("Sprite").gameObject.SetActive(true);
                follower.transform.Find("CharacterOffset").Find("Character").Find("Sprite").GetComponent<CharacterVisual>().enabled = true;
            }
        }

        // removed forceload variable as you always want to load a character with this method.
        private static void loadCharacterForCutscene(CharacterDefinitionId id, bool isCustom = true, bool hideSprite = false)
        {
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");

            if (!isCustom) //If it is not custom, take the character and hide the sprite
            {
                if (ppm.CurrentParty.Count < 3)
                {
                    ppm.AddPartyMember(id, true, true, false);
                    partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).position = ppm.leader.gameObject.transform.position;
                    #if DEBUG
                    OpenInstance.LoggerInstance.Msg("DEBUG NON CUSTOM CHARACTER SPAWNED: " + id.ToString());
                    #endif

                    foreach (CharacterDefinitionId charId in gameplayParty)
                    {
                        if (!RandomizerParty.Any(c => c.Equals(charId)))
                        {
                            ppm.CombatParty.Remove(charId);
                        }
                    }

                    hidePartyMember(id, partyHandler, hideSprite);
                }
            }
            else
            {
                if (!ppm.CurrentParty.Contains(id))
                {
                    ppm.CurrentParty.Add(id);
                }

                if (partyHandler != null)
                {
                    GameObject leader = partyHandler.transform.Find(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;
                    GameObject follower = partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).gameObject;

                    #if DEBUG
                    OpenInstance.LoggerInstance.Msg("DEBUG CHARACTER SPAWNED: " + id.ToString());
                    #endif

                    follower.SetActive(true);
                    follower.GetComponent<PartyCharacterFollower>().enabled = true;
                    follower.GetComponent<PlayerController>().enabled = true;
                    follower.transform.Find("CharacterOffset").Find("Character").GetComponent<Animator>().enabled = true;
                    follower.transform.Find("CharacterOffset").Find("Character").Find("Sprite").gameObject.SetActive(true);
                    follower.transform.Find("CharacterOffset").Find("Character").Find("Sprite").GetComponent<CharacterVisual>().enabled = true;
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
                    ClearAllCutsceneData();
                    
                }
            }
            else if (currentCutsceneGraph.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks"))
            {
                if (fromDockCutsceneData.ContainsKey(currentCutsceneGraph.gameObject.scene.name))
                {
                    currentCutsceneGraph.StopTree();
                    currentCutsceneCallback?.Invoke();
                    ClearAllCutsceneData();
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
                    GameObject partychar = partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).gameObject;
                    if (!partychar.activeSelf)
                    {
                        partychar.SetActive(true);
                        partychar.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.SetActive(true);
                        partychar.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = true;
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
                    if (!RandomizerParty.Any(c => c.Equals(charId)) && ppm.CurrentParty.Contains(charId))
                    {
                        ppm.CombatParty.Remove(charId);
                        hidePartyMember(charId, partyHandler, true);
                    }
                }
            }
        }

        private void endStoryCutscene()
        {
            currentCutsceneAboutToEndCallback?.Invoke();
            keepActiveFix = false;
            keepActive = new List<CharacterDefinitionId>();
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
            Vector3 pos = partyHandler.transform.Find(CharacterObjectDict[ppm.CurrentParty[0].ToString()].main).transform.position;

            foreach (CharacterDefinitionId id in gameplayParty)
            {
                if (isCustom)
                {
                    if (ppm.CurrentParty.Contains(id))
                    {
                        if (RandomizerParty.Any(c => c.Equals(id)) && ppm.CombatParty.Contains(id))
                        {
                            GameObject partychar = partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).gameObject;
                            partychar.transform.Find("CharacterOffset").Find("Character").Find("Sprite").gameObject.SetActive(true);
                        }
                        else
                        {
                            bool inCombat = ppm.CombatParty.ToArray().Any(c => c.Equals(id));
                            bool inParty = RandomizerParty.Any(c => c.Equals(id));
                            if (!inParty)
                            {
                                GameObject follower = partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).gameObject;
                                ppm.CurrentParty.Remove(id);
                                ppm.CombatParty.Remove(id);
                                if (follower.activeSelf && partyHandler != null)
                                {
                                    follower.SetActive(false);
                                }
                            }
                            else if (!inCombat)
                            {
                                GameObject follower = partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).gameObject;
                                ppm.CombatParty.Remove(id);
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
                        if (ppm.CurrentParty.Contains(id))
                        {
                            ppm.RemovePartyMember(id, true, false, false);
                        }
                        ppm.AddPartyMember(id, ppm.CurrentParty.Count < 3, ppm.CurrentParty.Count < 3, ppm.CurrentParty.Count < 3);
                        partyHandler.transform.Find(CharacterObjectDict[id.ToString()].main).transform.position = pos;
                    }
                    else
                    {
                        if (ppm.CurrentParty.Contains(id))
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

                GameObject leader = partyHandler.transform.Find(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].main).gameObject;

                if (ppm.CombatParty.Count > 1)
                {
                    GameObject follower = partyHandler.transform.Find(CharacterObjectDict[ppm.CombatParty.ToArray()[1].characterId].main).gameObject;
                    follower.GetComponent<PartyCharacterFollower>().FollowTarget(leader.GetComponent<FollowerLeader>(), true, true);  
                }
                if (ppm.CombatParty.Count > 2)
                {
                    GameObject follower = partyHandler.transform.Find(CharacterObjectDict[ppm.CombatParty.ToArray()[2].characterId].main).gameObject;
                    follower.GetComponent<PartyCharacterFollower>().FollowTarget(leader.GetComponent<FollowerLeader>(), true, true);
                }

                //set rpg camera
                setRPGCameraToLeader(ppm, partyHandler, RandomizerParty[0]);

                #if DEBUG
                OpenInstance.LoggerInstance.Msg("LEADER SWAPPED BACK: " + CharacterObjectDict[RandomizerParty[0].ToString()].main);
                #endif
            }

            currentCutsceneCallback?.Invoke();
            ClearAllCutsceneData();
        }

        private static void setRPGCameraToLeader(PlayerPartyManager ppm, GameObject partyHandler, CharacterDefinitionId leadId)
        {
            GameObject cameraObject = GameObject.Find("RpgCameraRig");
            if (cameraObject != null)
            {
                CharacterViewCameraContext context = cameraObject.GetComponent<CameraBehaviour>().currentContext.TryCast<CharacterViewCameraContext>();
                if (context != null)
                {
                    GameObject leader = partyHandler.transform.Find(CharacterObjectDict[leadId.ToString()].main).gameObject;
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
                        GameObject partyCharObj = partyHandler.transform.Find(CharacterObjectDict[partyChar.ToString()].main).gameObject;
                        if (partyCharObj != null && partyCharObj.activeSelf && ppm.CombatParty.Contains(partyChar))
                        {
                            partyCharObj.transform.Find("CharacterOffset").Find("Character").Find("Sprite").gameObject.SetActive(false);
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
                if (!ppm.CombatParty.Contains(cutsceneChar))
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
                                if (!RandomizerParty.Any(c => c.Equals(charId)) && ppm.CurrentParty.Contains(charId))
                                {
                                    ppm.CombatParty.Remove(charId);
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
            private static bool Prefix(GraphControllerBase __instance)
            {
                if (__instance == null || __instance.gameObject == null)
                {
                    return true;
                }

                #if DEBUG
                OpenInstance.LoggerInstance.Msg($"CUTSCENE: {__instance.gameObject.name}");
                #endif
                PlayerPartyManager ppm = PlayerPartyManager.Instance;
                if (cutscenesToCancel.TryGetValue(__instance.gameObject.name, out CutscenePatchData data))
                {
                    OpenInstance.LoggerInstance.Msg($"Cancelling {__instance.gameObject.name}");
                    data.onCutsceneAboutToStart?.Invoke();
                    data.onCutsceneStart?.Invoke();
                    data.onCutsceneAboutToEnd?.Invoke();
                    data.onCutsceneEnd?.Invoke();
                    return false;
                }
                else if (storyCutsceneData.TryGetValue(__instance.gameObject.name, out data))
                {
                    data.onCutsceneAboutToStart?.Invoke();
                    List<CharacterDefinitionId> chars = data.cutsceneCharacters;
                    if (data.requiredCharacters?.Count > 0)
                    {
                        List<CharacterDefinitionId> intersect = data.requiredCharacters.IntersectBy(ppm.CurrentParty.ToArray().Select(c1 => c1.ToString()), c2 => c2.ToString()).ToList();
                        if ((intersect.Count == 0 || intersect.Count < data.requiredCharacters.Count && data.requiredCharacterType == CutscenePatchData.RequiredCharacterType.ALL) && ppm.CurrentParty.Count < 2)
                        {
                            chars = data.backupCharacters;
                        }
                    }
                    GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                    resetCharactersForCutscenes(ppm, partyHandler, chars, data.forceAnimations, data.isCustom, data.hideSprite);
                    currentCutsceneAboutToEndCallback = data.onCutsceneAboutToEnd;
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.Story;
                    data.onCutsceneStart?.Invoke();
                }
                else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_To_Docks") && toDockCutsceneData.TryGetValue(__instance.gameObject.scene.name, out data))
                {
                    data.onCutsceneAboutToStart?.Invoke();
                    GameObject partyHandler = GameObject.Find("WorldMapParty(Clone)");
                    GameObject leader = partyHandler.transform.Find(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].world).gameObject;
                    leader.transform.position = data.newPosition;
                    currentCutsceneAboutToEndCallback = data.onCutsceneAboutToEnd;
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.World;
                    data.onCutsceneStart?.Invoke();
                }
                else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks") && fromDockCutsceneData.TryGetValue(__instance.gameObject.scene.name, out data))
                {
                    data.onCutsceneAboutToStart?.Invoke();
                    GameObject partyHandler = GameObject.Find("WorldMapParty(Clone)");
                    GameObject leader = partyHandler.transform.Find(CharacterObjectDict[ppm.Leader.CharacterDefinitionId.ToString()].world).gameObject;
                    leader.transform.position = data.newPosition;
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.World;
                    data.onCutsceneStart?.Invoke();
                }
                else if (cutscenesToSkip.TryGetValue(__instance.gameObject.name, out data))
                {
                    OpenInstance.LoggerInstance.Msg($"Skipping {__instance.gameObject.name}");
                    data.onCutsceneAboutToStart?.Invoke();
                    data.onCutsceneStart?.Invoke();
                    __instance.SkipTree();
                    data.onCutsceneAboutToEnd?.Invoke();
                    data.onCutsceneEnd?.Invoke();
                }
                else if (callbackCutscenes.TryGetValue(__instance.gameObject.name, out data))
                {
                    data.onCutsceneAboutToStart?.Invoke();
                    data.onCutsceneStart?.Invoke();
                    currentCutsceneAboutToEndCallback = data.onCutsceneAboutToEnd;
                    currentCutsceneCallback = data.onCutsceneEnd;
                    currentCutsceneGraph = __instance;
                    currentCutsceneType = CutsceneType.Story;
                }

                return true;
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
