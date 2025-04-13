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
        private static List<CharacterDefinitionId> gameplayParty = new List<CharacterDefinitionId>();
        private static List<string> characterObjectNames = new List<string>();
        private static List<string> worldMapObjectNames = new List<string>();
        private static List<string> characterIdleString = new List<string>();
        public static GraphControllerBase currentCutsceneGraph = null;
        public static CutsceneType currentCutsceneType = CutsceneType.None;

        private static bool keepActiveFix = false;
        private static List<CharacterDefinitionId> keepActive = new List<CharacterDefinitionId>();

        public CutsceneHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
            gameplayParty.Add(CharacterDefinitionId.Zale);
            gameplayParty.Add(CharacterDefinitionId.Valere);
            gameplayParty.Add(CharacterDefinitionId.Garl);
            gameplayParty.Add(CharacterDefinitionId.Serai);
            gameplayParty.Add(CharacterDefinitionId.Reshan);
            gameplayParty.Add(CharacterDefinitionId.Bst);

            characterObjectNames.Add("PlayableCharacter_SunBoy(Clone)");
            characterObjectNames.Add("PlayableCharacter_Moongirl(Clone)");
            characterObjectNames.Add("PlayableCharacter_Garl(Clone)");
            characterObjectNames.Add("PlayableCharacter_Serai(Clone)");
            characterObjectNames.Add("PlayableCharacter_Reshan(Clone)");
            characterObjectNames.Add("PlayableCharacter_Bst(Clone)");

            worldMapObjectNames.Add("PlayableCharacter_WorldMapSunboy(Clone)");
            worldMapObjectNames.Add("PlayableCharacter_WorldMapMoongirl(Clone)");
            worldMapObjectNames.Add("PlayableCharacter_WorldMapGarl(Clone)");
            worldMapObjectNames.Add("PlayableCharacter_WorldMapSerai(Clone)");
            worldMapObjectNames.Add("PlayableCharacter_WorldMapReshan(Clone)");
            worldMapObjectNames.Add("PlayableCharacter_WorldMapBst(Clone)");
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
                GameObject leader = partyHandler.transform.FindChild(getCharacterObjectString(ppm.Leader.CharacterDefinitionId)).gameObject;
                GameObject follower = partyHandler.transform.FindChild(getCharacterObjectString(id)).gameObject;

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
            if (currentCutsceneGraph != null && currentCutsceneGraph.gameObject != null)
            {
                if (currentCutsceneGraph.gameObject.name.Equals("BEH_Dock_Fast_Travel_To_Docks"))
                {
                    if (currentCutsceneGraph.gameObject.scene.name.Equals("EvermistIsland_WorldMap_Gameplay"))
                    {
                        currentCutsceneGraph.StopTree();
                        currentCutsceneGraph = null;
                        currentCutsceneType = CutsceneType.None;
                    }
                }
                else if (currentCutsceneGraph.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks"))
                {
                    if (currentCutsceneGraph.gameObject.scene.name.Equals("EvermistIsland_WorldMap_Gameplay"))
                    {
                        currentCutsceneGraph.StopTree();
                        currentCutsceneGraph = null;
                        currentCutsceneType = CutsceneType.None;
                    }
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
                        if (iteratePartyCheck(OpenSeaOfStarsMod.randomizerParty, id) && ppm.combatParty.Contains(id))
                        {
                            GameObject partychar = partyHandler.transform.FindChild(getCharacterObjectString(id)).gameObject;
                            partychar.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").gameObject.active = true;
                        }
                        else
                        {
                            //I really don't understand why comparisons don't properly work. Contains returned false when it should've returned true. TODO refactor
                            bool inParty = false;
                            bool inCombat = false;
                            foreach (CharacterDefinitionId thechar in ppm.combatParty)
                            {
                                if (getCharacterObjectString(thechar).Equals(getCharacterObjectString(id)))
                                {
                                    inCombat = true; break;
                                }
                            }
                            foreach (CharacterDefinitionId thechar in OpenSeaOfStarsMod.randomizerParty)
                            {
                                if (getCharacterObjectString(thechar).Equals(getCharacterObjectString(id)))
                                {
                                    inParty = true; break;
                                }
                            }
                            if (!inParty)
                            {
                                GameObject follower = partyHandler.transform.FindChild(getCharacterObjectString(id)).gameObject;
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
                            else
                            if (!inCombat)
                            {
                                GameObject follower = partyHandler.transform.FindChild(getCharacterObjectString(id)).gameObject;
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
                    GameObject partychar = partyHandler.transform.FindChild(getCharacterObjectString(id)).gameObject;
                    if (!partychar.active) { 
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
                if (!iteratePartyCheck(enabledList, partyChar))
                {
                    GameObject partyCharObj = partyHandler.transform.FindChild(getCharacterObjectString(partyChar)).gameObject;
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

        private static bool iteratePartyCheck(List<CharacterDefinitionId> list, CharacterDefinitionId id)
        {
            foreach (CharacterDefinitionId listId in list)
            {
                if (listId == id) { return true; }
            }
            return false;
        }

        // I am getting errors when using a dictionary for the CharacterDefinitionId??? back to basics i guess
        private static string getCharacterObjectString(CharacterDefinitionId id)
        {
            string ret = "";
            if (id == CharacterDefinitionId.Zale)
            {
                ret = characterObjectNames[0];
            }
            else if (id == CharacterDefinitionId.Valere)
            {
                ret = characterObjectNames[1];
            }
            else if (id == CharacterDefinitionId.Garl)
            {
                ret = characterObjectNames[2];
            }
            else if (id == CharacterDefinitionId.Serai)
            {
                ret = characterObjectNames[3];
            }
            else if (id == CharacterDefinitionId.Reshan)
            {
                ret = characterObjectNames[4];
            }
            else if (id == CharacterDefinitionId.Bst)
            {
                ret = characterObjectNames[5];
            }

            return ret;
        }

        private static string getWorldMapObjectString(CharacterDefinitionId id)
        {
            string ret = "";
            if (id == CharacterDefinitionId.Zale)
            {
                ret = worldMapObjectNames[0];
            }
            else if (id == CharacterDefinitionId.Valere)
            {
                ret = worldMapObjectNames[1];
            }
            else if (id == CharacterDefinitionId.Garl)
            {
                ret = worldMapObjectNames[2];
            }
            else if (id == CharacterDefinitionId.Serai)
            {
                ret = worldMapObjectNames[3];
            }
            else if (id == CharacterDefinitionId.Reshan)
            {
                ret = worldMapObjectNames[4];
            }
            else if (id == CharacterDefinitionId.Bst)
            {
                ret = worldMapObjectNames[5];
            }

            return ret;
        }


        [HarmonyPatch(typeof(GraphControllerBase), "StartTree")]
        private static class StartCutscenePatch
        {
            [HarmonyPrefix]
            private static void Prefix(GraphControllerBase __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    Msg($"CUTSCENE: {__instance.gameObject.name}");
                    PlayerPartyManager ppm = PlayerPartyManager.Instance;
                    GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
                    if (__instance.gameObject.name.Equals("CUT_IntroBossSlug"))
                    {
                        resetCharactersForCutscenes(ppm, partyHandler, new List<CharacterDefinitionId>() { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere }, false);

                        currentCutsceneGraph = __instance;
                        currentCutsceneType = CutsceneType.Story;
                    }
                    else if (__instance.gameObject.name.Equals("CUT_GiantSlugDefeated"))
                    {
                        resetCharactersForCutscenes(ppm, partyHandler, new List<CharacterDefinitionId>() { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere }, true);

                        currentCutsceneGraph = __instance;
                        currentCutsceneType = CutsceneType.Story;
                    }
                    else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_To_Docks"))
                    {
                        Msg("Dock event: " + __instance.gameObject.name);
                        if (__instance.gameObject.scene.name.Equals("EvermistIsland_WorldMap_Gameplay"))
                        {
                            partyHandler = GameObject.Find("WorldMapParty(Clone)");
                            GameObject leader = partyHandler.transform.FindChild(getWorldMapObjectString(ppm.Leader.CharacterDefinitionId)).gameObject;
                            leader.transform.position = new Vector3(116.5f, 1.01f, 74.5f);
                            currentCutsceneGraph = __instance;
                            currentCutsceneType = CutsceneType.World;
                        }
                    }
                    else if (__instance.gameObject.name.Equals("BEH_Dock_Fast_Travel_From_Docks"))
                    {
                        Msg("Dock event: " + __instance.gameObject.name);
                        if (__instance.gameObject.scene.name.Equals("EvermistIsland_WorldMap_Gameplay"))
                        {
                            partyHandler = GameObject.Find("WorldMapParty(Clone)");
                            GameObject leader = partyHandler.transform.FindChild(getWorldMapObjectString(ppm.Leader.CharacterDefinitionId)).gameObject;
                            leader.transform.position = new Vector3(114.5f, 3.01f, 74.5f);
                            currentCutsceneGraph = __instance;
                            currentCutsceneType = CutsceneType.World;
                        }
                    }
                    else if (__instance.gameObject.name.Equals("CUT_ElderMistBoss")) 
                    {
                        resetCharactersForCutscenes(ppm, partyHandler, new List<CharacterDefinitionId>() { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl }, false);

                        currentCutsceneGraph = __instance;
                        currentCutsceneType = CutsceneType.Story;
                    }
                    else if (__instance.gameObject.name.Equals("CUT_ElderMistDefeated"))
                    {
                        resetCharactersForCutscenes(ppm, partyHandler, new List<CharacterDefinitionId>() { CharacterDefinitionId.Zale, CharacterDefinitionId.Valere, CharacterDefinitionId.Garl }, true);

                        currentCutsceneGraph = __instance;
                        currentCutsceneType = CutsceneType.Story;
                    }
                }
            }
        }
    }
}
