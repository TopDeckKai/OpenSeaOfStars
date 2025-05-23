﻿using UnityEngine;
using Il2CppSabotage.Blackboard;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using File = Il2CppSystem.IO.File;
using Object = UnityEngine.Object;

namespace OpenSeaOfStars.Helpers
{
    public class BlackboardHelper
    {
        private OpenSeaOfStarsMod mod;
        private BlackboardManager blackboardManager;

        private static Dictionary<string, int> skipSetGuids = new()
        {
            {"7d2f0bce0c57c4b4cbb6c525e5b6bc2b", 0}, // Teaks camping, sometimes turns off after cutscenes
        };

        private static Dictionary<(string guid, int value), Dictionary<string, int>> triggers = new()
        {

            { ("67c2e14989179794caa05fcba09c99f3", 1), new Dictionary<string, int> {{"775d6e50cf2ef0f4d9d8a98a89bf4a02", 0}} }, // beating Malkomud (watching the acolyte cutscene)
            { ("fe9c5c7227b87714eb9f6d87ea89faed", 1), new Dictionary<string, int> {{"c67751f7421cee147afc1cb7d643e952", 1}} }, // beating Dweller of Woe
        };

        private static Dictionary<string, string> blackboardVariables = new();
        private static string bVarFileLocation = "bVars.txt";

        public BlackboardHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
        }

        internal void initBlackboardManager()
        {
            try
            {
                blackboardManager = BlackboardManager.Instance;
                mod.LoggerInstance.Msg($"blackboardManager Found!!");
            }
            catch
            {
                mod.LoggerInstance.Msg($"Unable to find blackboardManager");
            }

            if (File.Exists(bVarFileLocation))
            {
                using StreamReader reader = new(bVarFileLocation);
                while (reader.Peek() >= 0)
                {
                    string? line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] bVar = line.Split(",");
                        if (!blackboardVariables.ContainsKey(bVar[0]))
                        {
                            blackboardVariables.Add(bVar[0], bVar[1]);
                        }
                    }
                }
                mod.LoggerInstance.Msg($"Read {blackboardVariables.Count} bVars from file");
            }
        }

        internal void tryReadBlackboardManager()
        {
            
        }
        
        internal void FindNewBlackboardVars()
        {
            #if DEBUG
            Il2CppReferenceArray<Object> objs = ResourcesAPIInternal.FindObjectsOfTypeAll(Il2CppType.From(typeof(BlackboardVariable)));
            int added = 0;
            foreach (Object obj in objs)
            {
                BlackboardVariable bVar = obj.Cast<BlackboardVariable>();
                if (!blackboardVariables.ContainsKey(bVar.guid))
                {
                    blackboardVariables.Add(bVar.guid, bVar.name);
                    using StreamWriter writer = new(bVarFileLocation, true);
                    writer.WriteLine($"{bVar.guid},{bVar.name}");
                    mod.LoggerInstance.Msg($"{bVar.guid}, {bVar.name}");
                    added++;
                }
            }

            if (added > 0)
            {
                mod.LoggerInstance.Msg($"Found {added} new bVars");
            }
            #endif
        }

        public bool GetBlackboardValue(string key, out int value)
        {
            if (blackboardManager.blackboardValues.ContainsKey(key))
            {
                value = blackboardManager.blackboardValues[key];
                return true;
            }

            value = int.MinValue;
            return false;
        }
        public void AddBlackboardValue(string key, int value)
        {
            blackboardManager.SetBlackboardValue(key, value);
        }

        public static string GetBlackboardGuidFromName(string name)
        {
            try
            {
                return blackboardVariables.First(kvp => kvp.Value == name).Key;
            }
            catch (Exception)
            {
                return "";
            }
        }

        [HarmonyPatch(typeof(BlackboardManager), "GetBlackboardValue", typeof(BlackboardVariable))]
        private static class GetBlackboardPatch
        {
            private static void Postfix(BlackboardVariable variable, int __result)
            {
                if (variable != null && (!variable.guid.Equals("8ff406c2ce93c624c8d5a41cfa444937") && !variable.guid.Equals("eade193956f385243bbd0ab47aee2ee9"))) //update thread logs excluded
                {
                    OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"GET {variable.guid} {variable.name}: {__result}");
                }
            }
        }

        [HarmonyPatch(typeof(BlackboardManager), "SetBlackboardValue", typeof(BlackboardVariable), typeof(int))]
        private static class SetBlackboardPatch
        {
            private static void Postfix(BlackboardVariable variable, int value)
            {
                #if DEBUG
                if (variable != null)
                {
                    OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"SET: {variable.guid} {variable.name}: {value}");
                }
                #endif
            }
        }
        [HarmonyPatch(typeof(BlackboardManager), "SetBlackboardValue", typeof(string), typeof(int))]
        private static class SetBlackboardPatchGuid
        {
            private static bool Prefix(string guid, int value)
            {
                return string.IsNullOrEmpty(guid) || !skipSetGuids.ContainsKey(guid) || skipSetGuids[guid] != value;
            }
            private static void Postfix(string guid, int value)
            {
                if (string.IsNullOrEmpty(guid))
                {
                    return;
                }
                
                #if DEBUG
                OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"SET: {guid} {blackboardVariables[guid]}: {value}");
                #endif
                
                if (triggers.TryGetValue((guid, value), out Dictionary<string, int> bVars))
                {
                    foreach (KeyValuePair<string, int> bVar in bVars)
                    {
                        BlackboardManager.instance.SetBlackboardValue(bVar.Key, bVar.Value);
                    }
                }
            }
        }
    }
}
