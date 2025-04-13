﻿// #define HAS_UNITY_EXPLORER
using MelonLoader;
using Il2Cpp;
using Unity;
using UnityEngine;
using UnityEditor;
using OpenSeaOfStars.Helpers;
using Il2CppSabotage.Blackboard;
using HarmonyLib;
using Il2CppSabotage.Imposter;

namespace OpenSeaOfStars
{
    public class OpenSeaOfStarsMod : MelonMod
    {
        ActivityHelper ActivityHelper;
        SaveHelper SaveHelper;
        BlackboardHelper BlackboardHelper;
        CutsceneHelper CutsceneHelper;
        
        bool initLoaded = false;
        public static bool debug = true;
        public static List<CharacterDefinitionId> randomizerParty = new List<CharacterDefinitionId> { CharacterDefinitionId.Zale };
        
        
        private readonly Dictionary<string, int> charMap = new()
        {
            { CharacterDefinitionId.Zale.ToString(), 0 },
            { CharacterDefinitionId.Valere.ToString(), 1 },
            { CharacterDefinitionId.Garl.ToString(), 2 },
            { CharacterDefinitionId.Serai.ToString(), 3 },
            { CharacterDefinitionId.Reshan.ToString(), 4 },
            { CharacterDefinitionId.Bst.ToString(), 5 }
        };

        public OpenSeaOfStarsMod()
        {
            ActivityHelper = new ActivityHelper(this);
            BlackboardHelper = new BlackboardHelper(this);
            CutsceneHelper = new CutsceneHelper(this);
            SaveHelper = new SaveHelper(this);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName != null && sceneName.ToLower().Equals("titlescreen"))
            {
                if (!initLoaded)
                {
                    LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
                    ActivityHelper.initActivityManager(debug);
                    BlackboardHelper.initBlackboardManager(debug);
                    CutsceneHelper.initCutsceneManager(debug);
                    SaveHelper.createOpenSaveSlot(randomizerParty, debug);
                    initLoaded = true;
                }
                else
                {
                    SaveHelper.setLastSave(); // Temp to load the init save with "Continue"
                }
            }

            if (!string.IsNullOrEmpty(sceneName) && sceneName.ToLower().Equals("eldermisttrials_gameplay"))
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
                
                // spawn live mana. required to battle enemies
                // maybe don't allow entering trial without this?
                BlackboardHelper.AddBlackboardValue("9aee0be400eb37943991e9e95407b84b", 1);
                GameObject tutEnemy = GameObject.Find("ENC_01 (TutoMana)");
                if (tutEnemy != null)
                {
                    GameObject.Destroy(tutEnemy);
                    LoggerInstance.Msg("Unloaded blocking tutorial enemy");
                }
            }

            if (sceneName != null && sceneName.ToLower().Equals("kilnmountain_cutscene"))
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");

                GameObject blocker = GameObject.Find("DIALOGUES/DIA_BLOCK_EXIT");
                if (blocker != null)
                {
                    GameObject.Destroy(blocker);
                    LoggerInstance.Msg($"Unloaded blocking cutscene");
                }
            }

            if (sceneName != null && sceneName.ToLower().Equals("forbiddencavern_cutscene"))
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");

                GameObject bridgeBlocker = GameObject.Find("Cutscene_CollapsedCorridor/TRIG_CollapsedCorridor");
                if (bridgeBlocker != null)
                {
                    GameObject.Destroy(bridgeBlocker);
                    LoggerInstance.Msg($"Unloaded blocking cutscene");
                }

                GameObject bossBlocker = GameObject.Find("Cutscene_LuslugBoss/TRIG_LetsCamp");
                if (bossBlocker != null)
                {
                    GameObject.Destroy(bossBlocker);
                    LoggerInstance.Msg($"Unloaded blocking cutscene");
                }
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (CutsceneHelper.currentCutsceneType == CutsceneHelper.CutsceneType.Story)
            {
                CutsceneHelper.checkCutsceneFinished();
            }
            else if (CutsceneHelper.currentCutsceneType == CutsceneHelper.CutsceneType.World)
            {
                CutsceneHelper.skipWorldCutscene();
            }

            if (!debug)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                SaveHelper.save();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                BlackboardHelper.tryReadBlackboardManager();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                #if HAS_UNITY_EXPLORER
                HideUnityExplorer();
                #endif
                
                GameObject.FindObjectOfType<PauseMenu>(true).ReturnToTitle();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                var list = GameObject.FindObjectsOfType<Transform>(true);
                try
                {
                    Transform t = list.First(obj => obj.name == "ENCOUNTER_STUFF");
                    if (t != null)
                    {
                        GameObject enc = t.gameObject;
                        enc.SetActive(!enc.active);
                    }
                }
                catch (Exception e)
                {
                    return;
                }
            }
            
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
            {
                AddPartyMember(CharacterDefinitionId.Zale);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.V))
            {
                AddPartyMember(CharacterDefinitionId.Valere);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                AddPartyMember(CharacterDefinitionId.Garl);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                AddPartyMember(CharacterDefinitionId.Serai);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                AddPartyMember(CharacterDefinitionId.Reshan);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                AddPartyMember(CharacterDefinitionId.Bst);
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                PlayerPartyManager ppm = PlayerPartyManager.Instance;
                if (!ppm.cargoCharacters.Contains(CharacterDefinitionId.Teaks))
                {
                    LoggerInstance.Msg($"Adding {CharacterDefinitionId.Teaks.ToString()} to cargo for debug");
                    ppm.AddCargoCharacter(CharacterDefinitionId.Teaks);
                }
            }
        }
        
        private void AddPartyMember(CharacterDefinitionId character)
        {
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            if (ppm.currentParty.Contains(character))
            {
                return;
            }
            LoggerInstance.Msg($"Adding {character.ToString()} for debug");
            ppm.AddPartyMember(character, ppm.currentParty.Count < 3, true, true);
            ppm.SetupParty(true);
            
            randomizerParty.Add(character);
        }

        #if HAS_UNITY_EXPLORER
        /// <summary>
        /// Same as pressing F7 (or whatever keybind you've changed UE to)<br/>
        /// This exists because there's a bug with UE that prevents all inputs when opening game menus
        /// </summary>
        public static void HideUnityExplorer()
        {
            MelonBase ue = FindMelon("UnityExplorer", "Sinai");
            if (ue != null)
            {
                UnityExplorer.UI.UIManager.ShowMenu = false;
            }
        }
        #endif
    }
}
