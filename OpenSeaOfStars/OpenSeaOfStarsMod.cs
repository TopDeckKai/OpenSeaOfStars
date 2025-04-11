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
            if (debug)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.K))
                {
                    SaveHelper.save();
                }
                if (UnityEngine.Input.GetKeyDown(KeyCode.L))
                {
                    BlackboardHelper.tryReadBlackboardManager();
                }
                if (UnityEngine.Input.GetKeyDown(KeyCode.O))
                {
                    CharacterDefinitionId id = CharacterDefinitionId.Valere;
                    PlayerPartyManager ppm = PlayerPartyManager.Instance;

                    if (ppm.combatParty.Count < 2)
                    {
                        LoggerInstance.Msg("Adding character for debug " + id.ToString());

                        ppm.currentParty.Add(id);
                        ppm.AddToCombatParty(id);
                        CharacterDefinitionId.FirstFollower = id;
                        GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                        if (partyHandler != null)
                        {
                            LoggerInstance.Msg("PartyFound! Game Objects: " + partyHandler.transform.childCount);
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
    }
}
