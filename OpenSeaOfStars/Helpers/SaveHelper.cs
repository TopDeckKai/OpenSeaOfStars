﻿using Il2Cpp;
using UnityEngine;
using Il2CppSabotage.Blackboard;
using Il2CppKVP = Il2CppSystem.Collections.Generic;

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

        internal void createOpenSaveSlot(List<CharacterDefinitionId> randomizerParty)
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

                try
                {
                    SaveGameSlot defaultSave = saveManager.GetSaveSlot(99);
                    if (!saveManager.CopySaveToSlotIndex(defaultSave, modInitSaveSlot).Result)
                    {
                        mod.LoggerInstance.Msg("Failed to copy save file");
                        return;
                    }

                    sgs = saveManager.GetSaveSlot(modInitSaveSlot);
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

                    foreach (KeyValuePair<string, int> kvp in mod.InventoryHelper.startingItems)
                    {
                        if (!sgs.inventorySaveData.ownedInventoryItems.ContainsKey(mod.InventoryHelper.inventoryItems[kvp.Key].Reference))
                        {
                            sgs.inventorySaveData.ownedInventoryItems.Add(new Il2CppKVP.KeyValuePair<InventoryItemReference, int>(mod.InventoryHelper.inventoryItems[kvp.Key].Reference, kvp.Value));
                        }
                    }

                    // Commenting out starting weapons for now
                    // InventoryItemReference weaponVal = new InventoryItemReference();
                    // weaponVal.itemGuid = "40b7062ac812c5d47bb1ff0df4987e8e";

                    // InventoryItemReference weaponZale = new InventoryItemReference();
                    // weaponZale.itemGuid = "e3098c0169021924a97713b57a009928";

                    sgs.characterData[CharacterDefinitionId.Valere].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Valere].currentHP = 46;
                    sgs.characterData[CharacterDefinitionId.Valere].currentSP = 10;
                    sgs.characterData[CharacterDefinitionId.Valere].currentVariant = EPartyCharacterVariant.DEFAULT;
                    /*sgs.characterData[CharacterDefinitionId.Valere].equippedWeapon = weaponVal; */

                    sgs.characterData[CharacterDefinitionId.Zale].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Zale].currentHP = 45;
                    sgs.characterData[CharacterDefinitionId.Zale].currentSP = 8;
                    sgs.characterData[CharacterDefinitionId.Zale].currentVariant = EPartyCharacterVariant.DEFAULT;
                    /*sgs.characterData[CharacterDefinitionId.Zale].equippedWeapon = weaponZale;*/

                    sgs.characterData[CharacterDefinitionId.Serai].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Serai].currentVariant = EPartyCharacterVariant.DEFAULT;
                    sgs.characterData[CharacterDefinitionId.Reshan].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Garl].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.MasterMoraine].gaveCharacterStartInventory = true;
                    sgs.characterData[CharacterDefinitionId.Bst].gaveCharacterStartInventory = true;

                    // sgs.InventorySaveData.ownedInventoryItems.Add(weaponVal, 1);
                    // sgs.InventorySaveData.ownedInventoryItems.Add(weaponZale, 1);
                    
                    sgs.progressionSaveData.unlockedCombatMoves.Add(mod.InventoryHelper.skillUnlocks["LunarShield"].Reference);
                    sgs.progressionSaveData.unlockedCombatMoves.Add(mod.InventoryHelper.skillUnlocks["DashStrike"].Reference);

                    sgs.blackboardDictionary = createDefaultBlackboardData(randomizerParty);
                    ActivitySaveData activitySaveData = new ActivitySaveData();

                    activitySaveData.mainActivityReference = new ActivityReference("ce3bffc8dd7d47a4b9f49d5a85bb218d"); //239dd33e0c77af042b0abd2943e15417

                    sgs.activitySaveData = activitySaveData;

                    sgs.playerPartyManagerSaveData.partyLineups[0].currentParty.Clear();
                    sgs.playerPartyManagerSaveData.partyLineups[0].combatParty.Clear();
                    for (int i = 0; i < randomizerParty.Count; i++)
                    {
                        sgs.playerPartyManagerSaveData.partyLineups[0].currentParty.Add(randomizerParty[i]);
                        if (i == 0)
                        {
                            sgs.playerPartyManagerSaveData.partyLineups[0].mainCharacter = randomizerParty[i];
                            sgs.playerPartyManagerSaveData.partyLineups[0].leader = randomizerParty[i];
                        }
                        if (i < 3)
                        {
                            sgs.playerPartyManagerSaveData.partyLineups[0].combatParty.Add(randomizerParty[i]);
                        }
                    }

                    #if DEBUG
                    sgs.partyProgressData.currentLevel = 30;
                    sgs.partyProgressData.totalXP = 40630;
                    foreach (CharacterDefinitionId character in CutsceneHelper.gameplayParty)
                    {
                        sgs.characterData[character].currentHP = 999;
                        sgs.characterData[character].currentSP = 999;

                        sgs.characterData[character].levelUpStatUpgrades = createDebugLevelUpList();
                    }
                    #endif

                    sgs.cryptSaveManagerSaveData.favoriteMemorials[7] = 912617;

                    saveManager.SetSaveSlotAtIndex(modInitSaveSlot, sgs);
                    Il2CppSystem.Threading.Tasks.Task<bool> res = saveManager.SaveSlotFile(sgs);
                    mod.LoggerInstance.Msg($"Testing if save successful");
                    sgs = saveManager.GetSaveSlot(modInitSaveSlot);

                    if (res.Result && sgs.checkpointData.level.levelDefinitionGuid.Equals("4776b2f6ccdb0fe4195c6c0d89206875"))
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

        internal BlackboardDictionary createDefaultBlackboardData(List<CharacterDefinitionId> party)
        {
            BlackboardDictionary ret = new BlackboardDictionary();

            #if DEBUG
            if (party.Any(character => character == CharacterDefinitionId.Zale || character == CharacterDefinitionId.Valere))
            {
                ret.Add("eade193956f385243bbd0ab47aee2ee9", 1);
            }
            #endif

            // Gameplay flags
            ret.Add("3d7bce7fa2d8dd047a99e54e6526423a", 1); // UnlockedBoat
            ret.Add("9aee0be400eb37943991e9e95407b84b", 1); // CanSpawnLiveMana
            ret.Add("dd9e2a3bb5f9bb64b9474557c1bf132c", 0); // Bvar_Mines_GetPushBracelet_Done
            ret.Add("767078e1d423a5040b2c9cb4317da3b5", 1); // Evermist Docs
            ret.Add("7d2f0bce0c57c4b4cbb6c525e5b6bc2b", 1); // Bvar_Global_TeaksCamping_On
            ret.Add("0143bd6816480ba4982009e7b52f9099", 1); // CookSnacksUnlocked
            
            // Map fog of war
            ret.Add("bdf72b15ca6426b41b8a789b8a2b2f29", 1); // Bvar_FogUnlocked_IslandShrine04
            ret.Add("72e96f59b12529f4fa379e544ffcdbbc", 1); // Bvar_FogUnlocked_MesaIsland
            ret.Add("41ac062e1f0986e4281a53a31cdca717", 1); // Bvar_FogUnlocked_SkyShrineIsland
            ret.Add("368ffa866fc5bcc4b826720cc5d57df2", 1); // Bvar_FogUnlocked_WatcherIsland
            ret.Add("50862854cc21e3745842a3845273ed41", 1); // Bvar_FogUnlocked_SunkenRuinsActivator
            ret.Add("97889ae144bd21b4bbf4316a28f2b1d3", 1); // Bvar_FogUnlocked_FishingIsland
            ret.Add("a8e1eaf5e4970ae4cbe613455559b04d", 1); // Bvar_FogUnlocked_SettlerIsland

            // Unsorted, various cutscene info
            ret.Add("84534192eff14c645b7af6248d3cde17", 1); // Bvar_ArchivistRoom_Intro_Done
            ret.Add("066063647a806d849850751cb32810fe", 1); // Bvar_MooncradlePast_IntroKid_Done
            ret.Add("4cdb3f4e790033f4f93c97ba0b0ac2f1", 1); // Bbvar_ForbiddenCavern_KidEntranceDoor_Done
            ret.Add("3ec5ca3b716b39344b56a3c7ee8b2234", 1); // Bvar_ArchivistRoom_InterludeAct1_Done
            ret.Add("8921232ab8bbbd648bcf0bf43c3eee8d", 1); // Bvar_Lucent_SeraiNameRevealed
            ret.Add("9c67f207b52cb2a4eab836dc28fd3ac9", 1); // Bvar_ArchivistRoom_InterludeAct4_Done
            ret.Add("c65b65e48fc393a448d58ff1aac9644d", 1); // Bvar_JunglePath_IntroCutScene_Done
            ret.Add("d45db53dd7c1f3a4ea6143b0ebe5408d", 0); // Bvar_Global_WheelsUnlocked_Done // 0 until given 2 wheels figures?

            // Kiln Mountain flags
            ret.Add("23872d00c0fccc44b90faed2b33d7d4f", 1);
            ret.Add("7bd4347bdbff8af4a9e90918ab45ab07", 1);
            ret.Add("0be099e5b7be9ed4485bd80110104a99", 1);
            ret.Add("09a0c341be0a35d4abb4ab92702ccf11", 1);
            ret.Add("4600a6cd24212f1488fe7dcd92d5cb5b", 1);
            ret.Add("01d073ab77f40c7459cef473f6a64044", 1);

            // Prologue & Mooncradle
            // past
            ret.Add("54c150f4b5ad60540a82ffeff0b2e202", 1); // Bvar_MooncradlePast_IntroBrugavesErlina_Done
            ret.Add("d5e47da78cf42e84c95bffcd01b66dae", 1); // Bvar_MooncradlePast_ArgumentWithMoraine_Done
            ret.Add("c19e91f4f68f0064e992433ae3b804fc", 1); // Bvar_MountainTrail_IntroPartySplit_Done
            ret.Add("7ea13775cb23fba49a758c2a48ec4038", 1); // Bvar_FogUnlocked_SeaOfStars1
            ret.Add("8bf2351d237202145a8f7698d9ee3658", 1); // Bvar_FogUnlocked_SleeperIsland
            ret.Add("487b80dac3d19314793684db783ca1aa", 1); // Bvar_FogUnlocked_IslandShrine03
            ret.Add("f46a1e28208110e48bbef2f5fc45fd1f", 0); // Bbvar_ForbiddenCavern_ZoneEntry_Done // testing, may trigger garl getting his eye poked cutscene
            ret.Add("b8c81760117db0943a181633b4916152", 1); // Bvar_MooncradlePast_ReturnFromForbidden_Done
            ret.Add("9c0f3d33b592d2044baf92d95d4c0c00", 1); // Bvar_MooncradlePast_FlashbackKid_Done
            ret.Add("5304cd26dacc0a246914672d7e002d47", 1); // Bvar_MooncradlePast_IntroZenithAcademy_Done
            ret.Add("e7f74919505683e42b2cbeef63929c31", 1); // Bvar_MooncradlePast_Morning_Done
            ret.Add("3e435bdd6992ba044903a36741ebacc2", 1); // Bvar_Mooncradle_Moraine_CallWeapon_Done
            ret.Add("f97f879ff22c77340b3480753368d0e1", 1); // Bvar_MooncradlePast_TrainingBrugaves_Done
            ret.Add("980f0ab283c33474d8ff1192d7b5bb3b", 1); // Bvar_MooncradlePast_TrainingErlina_Done
            ret.Add("9a2a19e6b68ba3744b1c2693213f18e4", 1); // Bvar_MooncradlePast_TrainingSewing_Done
            ret.Add("94aeba6688db8934fa971de45cf16314", 1); // Bvar_Mooncradle_SoundInTheNight_Done
            ret.Add("878d616211febff49b25d78db8a02f75", 1); // Bvar_MooncradlePast_CookiesFound_Done
            ret.Add("213250fa0e0f107468afbfe17d4d02f3", 1); // Bvar_Mooncradle_FinishSewing_Done
            // final test
            ret.Add("2720546160f12594799f63a99280c6b8", 1); // Bvar_Mooncradle_BattleTuto_Done
            ret.Add("14c3e4f34da06e646a90fb8605e6fe99", 1); // Bvar_Mooncradle_IntroTuto_Done
            // Forbidden Cavern/Mountain Trail
            ret.Add("42dd882ecad4c304f98556b30c1ad1f5", 1); // Bvar_ForbiddenCavern_EntranceDoor_Done
            ret.Add("5cbf24a710e8156409bffb38edec491d", 1); // Bvar_ForbiddenCavern_LightBridge_Done
            ret.Add("58eaa8afa08ba5b4ca5d27fbdafb993a", 1); // Bvar_ClockworkCastle_InterludeOne_Done
            ret.Add("ebb3bbf4405b7b4459ee45d1bf1eb812", 1); // Bvar_MountainTrail_ReplayIntro_Done // campfire cooking tutorial
            ret.Add("a162c5e3299382b41a2affd9dd7bf2e4", 1); // Bvar_MountainTrail_ElderMistIntro_Done
            ret.Add("03cc26d5b9ba29a4688f814767a8be58", 1); // CharacterLightColorUnlocked ?? happens during Elder Mist introduction, testing
            ret.Add("e531806b7c2ae3840b743077f1167609", 1); // Bvar_MountainTrail_ToDIntro_Done - learn Time of Day 1
            
            // Sleeper Island
            ret.Add("80a0eb7a932f46d4b8f2ab17ca957731", 1); // Bvar_YeetGolem_Bridges_Done - Xtol area bridges
            ret.Add("2f831b26713f4434e9c29474767cd29c", 1); // Serai spying after leaving Xtol area
            // Moorlands
            ret.Add("ae21436453a98f74f9b2839e9c8b9aab", 1); // meet Teaks
            // Stonemasons Outpost
            ret.Add("21609a20eefbf394c9c7fc145b87c385", 1); // Bvar_Outpost_EnterFirstTime_Done
            ret.Add("def4ac542c3fc3a4496877c00921947f", 1); // Bvar_Mines_MeetingElder_Done
            ret.Add("f243ba246f56bb7419932f918853184e", 0); // Bvar_Mines_ElderExitMine_Done
            ret.Add("d69c8e3fa6966ab45812523c26235776", 1); // Bvar_Mines_Salamander_Done
            ret.Add("daa715cbd0e1e5b4bae8c163230b6bf3", 1); // Bvar_Mines_ExitStair_Done
            ret.Add("8dc814be1b11bee4fa2bbe5cd94479fd", 1); // Bvar_Mines_BossFight_Done // powers elevator as well
            ret.Add("67c2e14989179794caa05fcba09c99f3", 0); // Bvar_ClockworkCastle_InterludeTwo_Done // use as key for Mine state
            ret.Add("775d6e50cf2ef0f4d9d8a98a89bf4a02", 1); // Bvar_Outpost_ExitMine_Done // needs to be 0 when you beat Malkomud
            ret.Add("d44f0265bc66e4145919ba811cc784aa", 1); // Bvar_SleeperElevator_SleepingSerpent_Done
            // Abandoned Wizard Flags
            ret.Add("cd680b2ad619ca14c869c23e8cfcc55b", 1); // Bvar_WizardLab_Entrance_Done
            ret.Add("f911c403f7589884b84287dee465fe72", 1); // Bvar_WizardLab_LabIntro_Done
            ret.Add("3173d82ac3e376a439a2c299bdd4357a", 1); // Bvar_ArchivistRoom_InterludeAct2_Done
            
            // Wraith Island
            // Lucent
            ret.Add("c83b595e0178d7747b05e0157ba3d9b4", 1); // Bvar_Lucent_InnIntro_Done
            ret.Add("a44dc54092f12b941a46b51a5e0f6ecd", 0); // Bvar_Lucent_QuizIntro_Done
            ret.Add("7dc257cfd3b8ef8488be0839dd5fb233", 0); // Bvar_Lucent_RockDoor_Done // quiz shortcut door
            ret.Add("8907149d432385549b1fc402abf33e21", 1); // Bvar_Lucent_GoToSleep_Done
            ret.Add("602fe0109a8a4c540ac314f8266faf46", 1); // Bvar_Lucent_Breakfast_Done
            ret.Add("13ba1ed494b3460479b17a0b2d36f4ad", 1); // Bvar_CursedWood_TrySavingVillager_Done
            ret.Add("65451f05b96a2514f8b22b603292d406", 0); // Bvar_Lucent_GarlParalyzed_Pending
            ret.Add("d7b9ef1b3a631324a81852c9d0adca56", 1); // Bvar_CursedWood_Exit_Done
            ret.Add("a4bd4ad3131fb09408b0b822b3cc2ffe", 1); // Bvar_Lucent_SeraiJoin_Done
            // Necromancer's Lair
            ret.Add("9ce37b14bae41ae4d84a76614f376739", 1); // Bvar_FerrymanOutpost_CoinGiven_Done
            ret.Add("5d30674170deb0b42ba8e8823f44dc4b", 1); // Bvar_FloodedGraveyard_ReachDock_Done
            ret.Add("3e0e119901a58f143bccb1fd8a471c45", 1); // Bvar_FloodedGraveyard_DukePOS1_Done
            ret.Add("01e319d6f41c9ea468816ca4f0d6e09a", 1); // Bvar_FloodedGraveyard_DukePOS2_Done
            ret.Add("94a30890ca1d5d548b8d83257f496613", 0); // Bvar_FloodedGraveyard_ShowLair_Done // Duke Aventry boss fight
            ret.Add("607233a3c0d45114497fb42dfc689b54", 1); // Bvar_NecromancerLair_IntroSkull_Done
            ret.Add("a8c6606773eec734cb7096046c7b86c4", 1); // Bvar_NecromancerLair_BeamFall_Done
            ret.Add("1b9a8febc97662c4abefc3990a8f3b13", 1); // Bvar_NecromancerLair_GetGraplou_Done
            ret.Add("64ec52712ccd73d4e8e933f3cb0f7a74", 1); // Bvar_NecromancerLair_GraplouSwitchChecked01_Done
            ret.Add("fcef204d18eb34841be493ae05cd48d4", 1); // Bvar_NecromancerLair_LastHookOut_Done
            ret.Add("d6714c482db00fa46aa5e6797f11ef95", 1); // Bvar_NecromancerLair_SoulStone_Done // key for ferry cutscene to not play?
            ret.Add("a968d5e200889b647a3c9bfccc2f93cc", 1); // Bvar_Lucent_SeraiLeave_Done
            ret.Add("b3f4045898ce2044182ddce86a87292b", 1); // Bvar_Lucent_GarlHealed_Done // key for ferry cutscene to not play?
            ret.Add("65eabd26e11809144ab12342eb2a960e", 1); // Bvar_Lucent_PreparationForMansion_Done
            // Haunted Mansion
            ret.Add("8103ac9a2d3553242876908322f9221d", 1); // Bvar_CursedWood_BarrierRemoved_Done
            ret.Add("d28e7b922864f054580267bc52d93d29", 1); // Bvar_HautnedMansion_Intro_Done
            ret.Add("2bc1931d02bb5484682fe21aa768f03c", 1); // Bvar_HautnedMansion_SeraiJoined_Done
            ret.Add("a1b83bdc7debc3548b900b21af499958", 0); // Bvar_HauntedMansion_BotanicalHorror_Done
            ret.Add("c05881039602dec49a5760b43b81150a", 1); // Bvar_HauntedMansion_SeraiLeave_Done
            ret.Add("8d1beea3c7d2890499ce6e38029fb940", 0); // Bvar_HauntedMansion_SealRemoved_Done
            ret.Add("fe9c5c7227b87714eb9f6d87ea89faed", 0); // Bvar_HauntedMansion_DwellerDefeated_Done
            ret.Add("20f78bdeb6ac19b4a9a144c9fd6149d7", 0); // Bvar_Lucent_MoraineInBed_Done
            ret.Add("c67751f7421cee147afc1cb7d643e952", 1); // Bvar_Lucent_Act3_Done // use as key for Lucent state // 0 prevents player from leaving inn if Moraine is in bed?
            ret.Add("164de693261d45541b04c3e0ff4ac7fb", 1); // Bvar_ArchivistRoom_InterludeAct3_Done
            
            // ??? Island
            // Mirth
            ret.Add("c1a6bc90eed602f4d99338fd87f7e76e", 1); // Bvar_Mirth_EndAct4_Done
            ret.Add("3e4083d8c99ea5c4e897d24f9a8c79b1", 1); // Bvar_Mirth_BoatFadeOut_Done
            ret.Add("48d9143bb8860aa40b88e2765b01191d", 1); // Bvar_Mirth_Intro_Done
            ret.Add("c37f5c84dde583a45abe7b73809cb921", 1); // Bvar_Mirth_Phase01_Done
            ret.Add("c23a84cd2f08f9f4ebe7e262eac5e1b9", 0); // Bvar_Mirth_CheckCrypt_Pending
            ret.Add("415047891f21a1c4eb03c90832e51192", 1); // Bvar_Mirth_CryptFound_Done
            ret.Add("6ef6c263c3529dd429ddda8c2f45b74d", 1); // Bvar_Mirth_Phase02_Done
            ret.Add("2e27bf7d3fb0c5946a6c29ed4a74828b", 1); // Bvar_Mirth_Phase03_Done
            ret.Add("53473040c15e1e94f902a5a60d64227c", 1); // Bvar_Crypt_Intro_Done

            //SeaOfNightmare
            ret.Add("a6905e263b22db349b7cf1f346357256", 0); //Bvar_AirElementalSkyland_GetZephyrWind_Done
            ret.Add("f99b3027ec96e2e4497a52432c6b0abc", 1); //Bvar_StormCallerIsland_GetPearl


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
