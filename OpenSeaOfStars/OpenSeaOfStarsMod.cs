#define HAS_UNITY_EXPLORER
using MelonLoader;
using Il2Cpp;
using Il2CppInterop.Runtime;
using UnityEngine;
using OpenSeaOfStars.Helpers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine.SceneManagement;
using Il2CppSabotage.Blackboard;
using HarmonyLib;
using Il2CppSabotage.Imposter;
using Il2CppSabotage.Graph.Core;
using Il2CppSabotage.Localization;

namespace OpenSeaOfStars
{
    public class OpenSeaOfStarsMod : MelonMod
    {
        public static OpenSeaOfStarsMod OpenInstance { get; private set; }
        
        private ActivityHelper ActivityHelper;
        private SaveHelper SaveHelper;
        private BlackboardHelper BlackboardHelper;
        private CutsceneHelper CutsceneHelper;
        public InventoryHelper InventoryHelper { get; }
        private DialogueHelper DialogueHelper;
        public LevelHelper LevelHelper { get; }
        
        private bool initLoaded;
        public static List<CharacterDefinitionId> RandomizerParty = new() { CharacterDefinitionId.Zale };
        private string loadDialogue = "";
        
        public static Dictionary<string, (string main, string world)> CharacterObjectDict { get; } = new()
        {
            {CharacterDefinitionId.Zale.ToString(), ("PlayableCharacter_SunBoy(Clone)", "PlayableCharacter_WorldMapSunboy(Clone)")},
            {CharacterDefinitionId.Valere.ToString(), ("PlayableCharacter_Moongirl(Clone)", "PlayableCharacter_WorldMapMoongirl(Clone)")},
            {CharacterDefinitionId.Garl.ToString(), ("PlayableCharacter_Garl(Clone)", "PlayableCharacter_WorldMapGarl(Clone)")},
            {CharacterDefinitionId.Serai.ToString(), ("PlayableCharacter_Serai(Clone)", "PlayableCharacter_WorldMapSerai(Clone)")},
            {CharacterDefinitionId.Reshan.ToString(), ("PlayableCharacter_Reshan(Clone)", "PlayableCharacter_WorldMapReshan(Clone)")},
            {CharacterDefinitionId.Bst.ToString(), ("PlayableCharacter_Bst(Clone)", "PlayableCharacter_WorldMapBst(Clone)")}
        };

        public OpenSeaOfStarsMod()
        {
            OpenInstance = this;
            
            ActivityHelper = new ActivityHelper(this);
            BlackboardHelper = new BlackboardHelper(this);
            CutsceneHelper = new CutsceneHelper(this);
            SaveHelper = new SaveHelper(this);
            InventoryHelper = new InventoryHelper(this);
            DialogueHelper = new DialogueHelper();
            LevelHelper = new LevelHelper();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (string.IsNullOrEmpty(sceneName)) return;

            if (sceneName.ToLower().Equals("titlescreen"))
            {
                if (!initLoaded)
                {
                    LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
                    ActivityHelper.initActivityManager();
                    BlackboardHelper.initBlackboardManager();
                    CutsceneHelper.initCutsceneManager();
                    InventoryHelper.GetInventoryItems();
                    SaveHelper.createOpenSaveSlot(RandomizerParty);
                    initLoaded = true;
                }
                else
                {
                    SaveHelper.setLastSave(); // Temp to load the init save with "Continue"
                    CutsceneHelper.ClearAllCutsceneData();
                }
            }

            if (initLoaded)
            {
                BlackboardHelper.FindNewBlackboardVars();
            }

            if (sceneName.ToLower().Equals("eldermisttrials_gameplay"))
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
                
                // spawn live mana. required to battle enemies
                // maybe don't allow entering trial without this?
                BlackboardHelper.AddBlackboardValue("9aee0be400eb37943991e9e95407b84b", 1);
                GameObject tutEnemy = GameObject.Find("ENC_01 (TutoMana)");
                if (tutEnemy != null)
                {
                    tutEnemy.SetActive(false);
                    LoggerInstance.Msg("Turned off blocking tutorial enemy");
                }
            }

            if (sceneName.ToLower().Equals("kilnmountain_cutscene"))
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");

                GameObject blocker = GameObject.Find("DIALOGUES/DIA_BLOCK_EXIT");
                if (blocker != null)
                {
                    GameObject.Destroy(blocker);
                    LoggerInstance.Msg($"Unloaded blocking cutscene");
                }
            }

            if (sceneName.ToLower().Equals("forbiddencavern_cutscene"))
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

            if (sceneName.ToLower().Equals("mines_gameplay"))
            {
                LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");

                GameObject blocker = GameObject.Find("GPI_AutosaveTrigger_BoucheTrou (C)");
                if (blocker != null)
                {
                    blocker.SetActive(false);
                    LoggerInstance.Msg($"Unloaded buggy autosave");
                }
                blocker = GameObject.Find("GPI_AutosaveTrigger_BoucheTrou (E)");
                if (blocker != null)
                {
                    blocker.SetActive(false);
                    LoggerInstance.Msg($"Unloaded buggy autosave");
                }
                blocker = GameObject.Find("GPI_AutosaveTrigger_BoucheTrou (G)");
                if (blocker != null)
                {
                    blocker.SetActive(false);
                    LoggerInstance.Msg($"Unloaded buggy autosave");
                }
            }

            if (sceneName.ToLower().Equals("forbiddencavern_gameplay"))
            {
                GameObject bossSlots = GameObject.Find("ENCOUNTERS_STUFF/ENC_04 (Boss)/Encounter/PlayerSlots/FrontRowSlots");
                GameObject encounterObj = GameObject.Find("ENCOUNTERS_STUFF/ENC_04 (Boss)/Encounter");
                if (bossSlots != null && encounterObj != null)
                {
                    GameObject bossSlot3 = new GameObject("EncounterPlayerSlot2");

                    bossSlot3.AddComponent<GameObjectSpriteDrawer>();
                    bossSlot3.transform.parent = bossSlots.transform;
                    bossSlot3.transform.position = new Vector3(-39.5f, 2f, 228.91f);
                    EncounterCharacterSlot thirdSlot = bossSlot3.AddComponent<EncounterCharacterSlot>();

                    Encounter encounter = encounterObj.GetComponent<Encounter>();
                    Il2CppSystem.Collections.Generic.List<EncounterCharacterSlot> slots = encounter.playerSlots;
                    slots.Add(thirdSlot);
                    encounter.playerSlots = slots;
                }
                else
                {
                    LoggerInstance.Msg($"BOSS SLOTS NOT FOUND");
                }
            }
            
            if (sceneName.ToLower().Equals("outpost_gameplay"))
            {
                // open outpost
                BlackboardHelper.AddBlackboardValue("8dc814be1b11bee4fa2bbe5cd94479fd", 1);
            }
            
            if (sceneName.ToLower().Equals("mines_gameplay"))
            {
                // if acolyte cutscene is not watched, make Malkomud fightable
                if (BlackboardHelper.GetBlackboardValue("67c2e14989179794caa05fcba09c99f3", out int value) && value == 0)
                {
                    BlackboardHelper.AddBlackboardValue("8dc814be1b11bee4fa2bbe5cd94479fd", 0);
                    Il2CppArrayBase<Transform>? encounters = GameObject.Find("ENCOUNTER_STUFF").GetComponentsInChildren<Transform>(true);
                    if (encounters == null || encounters.Count <= 0)
                    {
                        return;
                    }
                    Transform enc = encounters.First(enc => enc.gameObject.name == "ENC_Mines_Boss_Malkumud");
                    if (enc)
                    {
                        enc.gameObject.SetActive(true);
                    }
                }
            }

            if (sceneName.ToLower().Equals("wizardlab_gameplay"))
            {
                GameObject blueTrigger = GameObject.Find("ROOMS_STUFF/BLUE_ROOM_STUFF/BluePortal_Stuff/TRIG_DisableBluePortal").gameObject;
                GameObject cyanTrigger = GameObject.Find("ROOMS_STUFF/CYAN_ROOM_STUFF/TRIG_Disable_CyanPortal").gameObject;
                if (blueTrigger)
                {
                    GameObject.Destroy(blueTrigger);
                }

                if (cyanTrigger)
                {
                    GameObject.Destroy(cyanTrigger);
                }
                
                // disabling the dummy WizCroube because it hardlocks the game if you Graplou it
                GameObject croube = GameObject.Find("ROOMS_STUFF/BLUE_ROOM_STUFF/MovingFloor_Stuff/Floors_Stuff/WizCroube04_Dummy");
                if (croube)
                {
                    croube.SetActive(false);
                }
            }

            if (sceneName.ToLower().Equals("vespertine_cutscene_worldmap"))
            {
                // This does not work on first frame of load. TODO refactor.
                loadDialogue = sceneName.ToLower();
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (CutsceneHelper.currentCutsceneType == CutsceneHelper.CutsceneType.Story || CutsceneHelper.currentCutsceneType == CutsceneHelper.CutsceneType.StoryExt)
            {
                CutsceneHelper.checkCutsceneFinished();
            }
            else if (CutsceneHelper.currentCutsceneType == CutsceneHelper.CutsceneType.World)
            {
                CutsceneHelper.skipWorldCutscene();
            }
            
            if (loadDialogue.Length > 0)
            {
                if (DialogueHelper.changeDialoguePerScene(loadDialogue))
                {
                    loadDialogue = "";
                }
            }

            #if DEBUG
            if (Input.GetKeyDown(KeyCode.K))
            {
                SaveHelper.save();
            }

            else if (Input.GetKeyDown(KeyCode.P))
            {
                #if HAS_UNITY_EXPLORER
                HideUnityExplorer();
                #endif
                
                GameObject.FindObjectOfType<PauseMenu>(true).ReturnToTitle();
            }

            else if (Input.GetKeyDown(KeyCode.E))
            {
                var list = GameObject.FindObjectsOfType<Transform>(true);
                try
                {
                    List<string> encounterNames = new() {"ENCOUNTER_STUFF", "ENCOUNTERS_STUFF", "ENC_YeetGolem_01"};
                    Transform t = list.First(obj => encounterNames.Contains(obj.name));
                    if (t != null)
                    {
                        GameObject enc = t.gameObject;
                        enc.SetActive(!enc.activeSelf);
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
            
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                BlackboardHelper.AddBlackboardValue("e531806b7c2ae3840b743077f1167609", 0);
            }
            
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                AddPartyMember(CharacterDefinitionId.Zale);
            }
            else if (Input.GetKeyDown(KeyCode.V))
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
            else if (Input.GetKeyDown(KeyCode.I))
            {
                InventoryHelper.PrintInventoryItems();
            }
            #endif
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
            if (character == CharacterDefinitionId.Zale && ppm.leader.CharacterDefinitionId != CharacterDefinitionId.Valere || character == CharacterDefinitionId.Valere && ppm.leader.CharacterDefinitionId != CharacterDefinitionId.Zale)
            {
                PlayerPartyCharacter old = ppm.leader;
                ppm.SetMainCharacter(character);
                ppm.RemovePartyMember(old.CharacterDefinitionId, true, true, false);
                ppm.SetLeader(character);
                ppm.SetLeaderFirstInParty();
                ppm.leader.transform.position = old.transform.position;
                CameraBehaviour cam = Camera.main.GetComponentInParent<CameraBehaviour>();
                if (cam.currentContext.GetIl2CppType() == Il2CppType.Of<CharacterViewCameraContext>())
                {
                    string charObjName = SceneManager.GetActiveScene().name.ToLower().Contains("worldmap") ? CharacterObjectDict[character.ToString()].world : CharacterObjectDict[character.ToString()].main;
                    cam.currentContext.Cast<CharacterViewCameraContext>().cameraLookAtPosition = GameObject.FindObjectsOfType<PlayerCameraLookAtPosition>(true).First(c => c.name.Equals(charObjName));
                }
                BlackboardHelper.AddBlackboardValue("eade193956f385243bbd0ab47aee2ee9", 1); // can fly with a Solstice Warrior

                System.Collections.IEnumerator reAddCharAfterFrame()
                {
                    yield return null;
                    ppm.AddPartyMember(old.CharacterDefinitionId, ppm.currentParty.Count < 3, true, true);
                    ppm.SetupParty(!BoatManager.Instance.IsInBoatMode);
                }
                MelonCoroutines.Start(reAddCharAfterFrame());
            }
            else if (ppm.currentParty.Count <= 3)
            {
                ppm.followers.ToArray().First(c => c.characterDefinitionId.ToString() == character.ToString()).transform.position = ppm.leader.transform.position;
            }
            ppm.SetupParty(!BoatManager.Instance.IsInBoatMode);
            
            RandomizerParty.Add(character);
        }

        
        #if HAS_UNITY_EXPLORER && DEBUG
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
