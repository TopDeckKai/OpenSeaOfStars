using Il2Cpp;
using UnityEngine;
using HarmonyLib;
using Il2CppSabotage.Graph.Core;
using MelonLoader;
using Il2CppSabotage.Imposter;

namespace OpenSeaOfStars.Helpers
{
    public class CutsceneHelper : MelonLogger
    {
        private OpenSeaOfStarsMod mod;
        private CutsceneManager cutManager;
        private static List<CharacterDefinitionId> gameplayParty = new List<CharacterDefinitionId>();
        private static List<string> characterObjectNames = new List<string>();
        private static GraphControllerBase currentCutsceneGraph = null;

        // TODO: Refactor to use game object.find potentially instead of this, using the dictonary for the gameobject's name

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

        private static void loadCharacterForCutscene(CharacterDefinitionId id, bool forceLoadCharacter)
        {
            PlayerPartyManager ppm = PlayerPartyManager.Instance;
            ppm.currentParty.Add(id);
            if (ppm.currentParty.Count == 2)
            {
                // CharacterDefinitionId.FirstFollower = id;
                // ppm.AddToCombatParty(id);
            }
            else if (ppm.currentParty.Count == 3)
            {
                // CharacterDefinitionId.LastFollower = id;
                // ppm.AddToCombatParty(id);
            }

            GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");
            if (partyHandler != null)
            {
                GameObject leader = partyHandler.transform.FindChild(getCharacterObjectString(ppm.Leader.CharacterDefinitionId)).gameObject;
                GameObject follower = partyHandler.transform.FindChild(getCharacterObjectString(id)).gameObject;
                if (ppm.currentParty.Count <= 3 || forceLoadCharacter)
                {
                    follower.SetActive(true);
                    follower.GetComponent<PartyCharacterFollower>().enabled = true;
                    follower.GetComponent<PlayerController>().enabled = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").GetComponent<Animator>().enabled = true;
                    follower.transform.FindChild("CharacterOffset").FindChild("Character").FindChild("Sprite").GetComponent<CharacterVisual>().enabled = true;
                    follower.transform.localPosition = leader.transform.localPosition;
                }
            }
        }

        public void checkCutsceneFinished()
        {
            if (currentCutsceneGraph != null && !currentCutsceneGraph.IsPlaying)
            {
                PlayerPartyManager ppm = PlayerPartyManager.Instance;

                foreach (CharacterDefinitionId id in gameplayParty)
                {
                    GameObject partyHandler = GameObject.Find("CapsuleParty(Clone)");

                    if (ppm.currentParty.Contains(id) && !OpenSeaOfStarsMod.randomizerParty.Contains(id))
                    {
                        //I really don't understand why comparisons don't properly work. Contains returned false when it should've returned true. TODO refactor
                        bool inParty = false;
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
                            if (follower.active)
                            {
                                Msg("REMOVAL for " + id.ToString());
                                ppm.currentParty.Remove(id);
                                ppm.combatParty.Remove(id);
                                if (partyHandler != null)
                                {
                                    follower.active = false;
                                }
                            }
                        }
                    }
                }

                currentCutsceneGraph = null;
            }
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

        [HarmonyPatch(typeof(GraphControllerBase), "StartTree")]
        private static class StartCutscenePatch
        {
            [HarmonyPrefix]
            private static void Prefix(GraphControllerBase __instance)
            {
                if (__instance != null && __instance.gameObject != null)
                {
                    if (__instance.gameObject.name.Equals("CUT_IntroBossSlug")
                        || __instance.gameObject.name.Equals("CUT_GiantSlugDefeated")
                    )
                    {
                        PlayerPartyManager ppm = PlayerPartyManager.Instance;
                        if (ppm.currentParty.Count == 1)
                        {
                            if (ppm.currentParty.Contains(CharacterDefinitionId.Valere))
                            {
                                loadCharacterForCutscene(CharacterDefinitionId.Zale, false);
                            }
                            else
                            {
                                loadCharacterForCutscene(CharacterDefinitionId.Valere, false);
                            }
                        }

                        currentCutsceneGraph = __instance;
                    }
                }
            }
        }
    }
}
