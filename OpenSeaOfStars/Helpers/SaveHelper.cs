﻿using Il2Cpp;
using Il2CppSystem;
using UnityEngine;
using Il2CppSabotage.Blackboard;
using Il2CppInterop.Runtime;

namespace OpenSeaOfStars.Helpers
{
    public class SaveHelper
    {
        private OpenSeaOfStarsMod mod;
        private SaveManager saveManager;
        private int modInitSaveSlot = 8;

        public SaveHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
        }

        internal void setLastSave()
        {
            saveManager.lastSelectedSlotIndex = modInitSaveSlot;
        }

        internal void setLastSave(int index)
        {
            saveManager.lastSelectedSlotIndex = index;
        }

        internal void createOpenSaveSlot(bool debug = false)
        {
            try
            {
                saveManager = SaveManager.Instance;
                mod.LoggerInstance.Msg($"Save Manager Found!!");
            }
            catch
            {
                mod.LoggerInstance.Msg($"Unable to find SaveManager");
            }

            if (saveManager != null)
            {
                SaveGameSlot sgs = null;

                CharacterDefinitionId initialCharacter = CharacterDefinitionId.Zale;

                try
                {
                    SaveGameSlot defaultSave = saveManager.GetSaveSlot(99);
                    sgs = saveManager.Clone(defaultSave);
                    mod.LoggerInstance.Msg($"Loading Temp Slot at index: " + sgs.slotIndex);

                    sgs.slotIndex = modInitSaveSlot;
                    saveManager.ForceEnableSave();
                    sgs.lastUsedEntranceId = "3136c16acd6167f40a192cb93c188fd4";
                    sgs.saveId = "SOS_SaveSlot" + modInitSaveSlot;

                    setLastSave();

                    CheckpointData checkpointData = sgs.checkpointData;
                    LevelReference lev = new LevelReference();
                    lev.levelDefinitionGuid = "4776b2f6ccdb0fe4195c6c0d89206875";
                    checkpointData.level = lev;

                    checkpointData.savepointId = ESavepointId.NONE;
                    checkpointData.position = new Vector3(0f, 0f, 0f);
                    checkpointData.isBoat = true;

                    sgs.checkpointData = checkpointData;

                    InventoryItemReference weaponVal = new InventoryItemReference();
                    weaponVal.itemGuid = "40b7062ac812c5d47bb1ff0df4987e8e";

                    InventoryItemReference weaponZale = new InventoryItemReference();
                    weaponZale.itemGuid = "e3098c0169021924a97713b57a009928";

                    sgs.characterData[CharacterDefinitionId.Valere].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Valere].currentHP = 46;
                    sgs.characterData[CharacterDefinitionId.Valere].currentSP = 10;
                    sgs.characterData[CharacterDefinitionId.Valere].currentVariant = 0;
                    /*sgs.characterData[CharacterDefinitionId.Valere].equippedWeapon = weaponVal; */

                    sgs.characterData[CharacterDefinitionId.Zale].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Zale].currentHP = 45;
                    sgs.characterData[CharacterDefinitionId.Zale].currentSP = 8;
                    sgs.characterData[CharacterDefinitionId.Zale].currentVariant = 0;
                    /*sgs.characterData[CharacterDefinitionId.Zale].equippedWeapon = weaponZale;*/

                    sgs.characterData[CharacterDefinitionId.Serai].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Serai].currentVariant = 0;
                    sgs.characterData[CharacterDefinitionId.Reshan].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Garl].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.MasterMoraine].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Bst].gaveCharacterStartInventory = true;

                    // sgs.InventorySaveData.ownedInventoryItems.Add(weaponVal, 1);
                    // sgs.InventorySaveData.ownedInventoryItems.Add(weaponZale, 1);

                    sgs.blackboardDictionary = createDefaultBlackboardData();
                    ActivitySaveData activitySaveData = new ActivitySaveData();

                    activitySaveData.mainActivityReference = new ActivityReference("ce3bffc8dd7d47a4b9f49d5a85bb218d"); //239dd33e0c77af042b0abd2943e15417

                    sgs.activitySaveData = activitySaveData;

                    sgs.currentParty.Clear();
                    sgs.currentParty.Add(initialCharacter);
                    sgs.combatParty.Clear();
                    sgs.combatParty.Add(initialCharacter);
                    sgs.mainCharacter = initialCharacter;
                    sgs.leader = initialCharacter;

                    if (debug)
                    {
                        sgs.characterData[initialCharacter].currentHP = 999;
                        sgs.characterData[initialCharacter].currentSP = 999;

                        sgs.characterData[initialCharacter].levelUpStatUpgrades = createDebugLevelUpList();
                    }

                    saveManager.SetSaveSlotAtIndex(modInitSaveSlot, sgs);
                    saveManager.SaveSlotFile(sgs);

                    mod.LoggerInstance.Msg($"Testing if save successful");
                    sgs = saveManager.GetSaveSlot(modInitSaveSlot);

                    if (sgs.checkpointData.level.levelDefinitionGuid.Equals("4776b2f6ccdb0fe4195c6c0d89206875"))
                    {
                        mod.LoggerInstance.Msg($"Save Successful!");
                    }
                    else
                    {
                        mod.LoggerInstance.Msg($"Save Unsuccessful :( " + sgs.checkpointData.level.levelDefinitionGuid);
                    }

                }
                catch (System.Exception ex)
                {
                    mod.LoggerInstance.Msg($"Unable to manage save data: " + ex.Message + "\n" + ex.StackTrace.ToString());
                }
            }
        }

        public void save()
        {
            saveManager.SaveLoadedSaveSlot();
        }

        internal BlackboardDictionary createDefaultBlackboardData()
        {
            BlackboardDictionary ret = new BlackboardDictionary();

            //Debug can fly
            if (OpenSeaOfStarsMod.debug)
            {
                ret.Add("eade193956f385243bbd0ab47aee2ee9", 1);
            }
            else
            {
                ret.Add("eade193956f385243bbd0ab47aee2ee9", 0);
            }

            // Gameplay flags
            ret.Add("3d7bce7fa2d8dd047a99e54e6526423a", 1); // UnlockedBoat
            ret.Add("9aee0be400eb37943991e9e95407b84b", 0); // CanSpawnLiveMana
            ret.Add("1b9a8febc97662c4abefc3990a8f3b13", 0); // NecromancerLair_GetGraplou_Done
            ret.Add("dd9e2a3bb5f9bb64b9474557c1bf132c", 0); // Bvar_Mines_GetPushBracelet_Done
            ret.Add("767078e1d423a5040b2c9cb4317da3b5", 1); // Evermist Docs

            // Unsorted, various cutscene info
            ret.Add("84534192eff14c645b7af6248d3cde17", 1);
            ret.Add("066063647a806d849850751cb32810fe", 1);
            ret.Add("3ec5ca3b716b39344b56a3c7ee8b2234", 1);
            ret.Add("c1a6bc90eed602f4d99338fd87f7e76e", 1);
            ret.Add("164de693261d45541b04c3e0ff4ac7fb", 1);
            ret.Add("9c67f207b52cb2a4eab836dc28fd3ac9", 1);
            ret.Add("a8e1eaf5e4970ae4cbe613455559b04d", 1);
            ret.Add("3e4083d8c99ea5c4e897d24f9a8c79b1", 1);
            ret.Add("48d9143bb8860aa40b88e2765b01191d", 1);
            ret.Add("c37f5c84dde583a45abe7b73809cb921", 1);
            ret.Add("c23a84cd2f08f9f4ebe7e262eac5e1b9", 0);
            ret.Add("415047891f21a1c4eb03c90832e51192", 1);
            ret.Add("53473040c15e1e94f902a5a60d64227c", 1);
            ret.Add("6ef6c263c3529dd429ddda8c2f45b74d", 1);
            ret.Add("2e27bf7d3fb0c5946a6c29ed4a74828b", 1);
            ret.Add("bdf72b15ca6426b41b8a789b8a2b2f29", 1);
            ret.Add("72e96f59b12529f4fa379e544ffcdbbc", 1);
            ret.Add("41ac062e1f0986e4281a53a31cdca717", 1);
            ret.Add("368ffa866fc5bcc4b826720cc5d57df2", 1);
            ret.Add("c65b65e48fc393a448d58ff1aac9644d", 1);
            ret.Add("8921232ab8bbbd648bcf0bf43c3eee8d", 1);
            ret.Add("d45db53dd7c1f3a4ea6143b0ebe5408d", 1);

            // Abandoned Wizard Flags
            ret.Add("cd680b2ad619ca14c869c23e8cfcc55b", 1);
            ret.Add("f911c403f7589884b84287dee465fe72", 1);

            // Kiln Mountain flags
            ret.Add("23872d00c0fccc44b90faed2b33d7d4f", 1);
            ret.Add("7bd4347bdbff8af4a9e90918ab45ab07", 1);
            ret.Add("0be099e5b7be9ed4485bd80110104a99", 1);
            ret.Add("09a0c341be0a35d4abb4ab92702ccf11", 1);
            ret.Add("eed8215f5de6b324bad36b1d9b3ecf7a", 1);
            ret.Add("4600a6cd24212f1488fe7dcd92d5cb5b", 1);
            ret.Add("01d073ab77f40c7459cef473f6a64044", 1);

            // Forbidden Cavern
            ret.Add("4cdb3f4e790033f4f93c97ba0b0ac2f1", 1); // Kid Entrance Door
            ret.Add("42dd882ecad4c304f98556b30c1ad1f5", 1); // Entrance Door

            // Prologue & Mooncradle
            ret.Add("7ea13775cb23fba49a758c2a48ec4038", 1);
            ret.Add("8bf2351d237202145a8f7698d9ee3658", 1);
            ret.Add("487b80dac3d19314793684db783ca1aa", 1);
            ret.Add("f46a1e28208110e48bbef2f5fc45fd1f", 0); // testing, may trigger garl getting his eye poked cutscene
            ret.Add("b8c81760117db0943a181633b4916152", 1);
            ret.Add("9c0f3d33b592d2044baf92d95d4c0c00", 1);
            ret.Add("5304cd26dacc0a246914672d7e002d47", 1);
            ret.Add("e7f74919505683e42b2cbeef63929c31", 1);
            ret.Add("f97f879ff22c77340b3480753368d0e1", 1);
            ret.Add("980f0ab283c33474d8ff1192d7b5bb3b", 1);
            ret.Add("9a2a19e6b68ba3744b1c2693213f18e4", 1);
            ret.Add("94aeba6688db8934fa971de45cf16314", 1);
            ret.Add("213250fa0e0f107468afbfe17d4d02f3", 1);
            ret.Add("58eaa8afa08ba5b4ca5d27fbdafb993a", 1); // Post forbidden cavern number guys scene

            return ret;
        }


        private Il2CppSystem.Collections.Generic.List<EPlayableCharacterStat> createDebugLevelUpList()
        {
            Il2CppSystem.Collections.Generic.List<EPlayableCharacterStat> levelUplist = new Il2CppSystem.Collections.Generic.List<EPlayableCharacterStat>();

            for (int i = 0; i < 50; i++)
            {
                levelUplist.Add(EPlayableCharacterStat.HitPoint);
                levelUplist.Add(EPlayableCharacterStat.SkillPoint);
                levelUplist.Add(EPlayableCharacterStat.PhysicalDefense);
                levelUplist.Add(EPlayableCharacterStat.PhysicalAttack);
                levelUplist.Add(EPlayableCharacterStat.MagicalAttack);
                levelUplist.Add(EPlayableCharacterStat.MagicalDefense);
            }

            return levelUplist;
        }

    }
}
