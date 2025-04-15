using Il2Cpp;
using UnityEngine;
using Il2CppSabotage.Blackboard;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using File = Il2CppSystem.IO.File;
using Object = UnityEngine.Object;

namespace OpenSeaOfStars.Helpers
{
    public class BlackboardHelper : MelonLogger
    {
        private OpenSeaOfStarsMod mod;
        private BlackboardManager blackboardManager;

        private static Dictionary<string, string> blackboardVariables = new();
        private static string bVarFileLocation = "bVars.txt";

        public BlackboardHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
        }

        internal void initBlackboardManager(bool debug)
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
            if (OpenSeaOfStarsMod.debug)
            {
                
            }
        }
        
        internal void FindNewBlackboardVars()
        {
            if (OpenSeaOfStarsMod.debug)
            {
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
            }
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
                if (OpenSeaOfStarsMod.debug && variable != null)
                {
                    OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"SET: {variable.guid} {variable.name}: {value}");
                }
            }
        }
        [HarmonyPatch(typeof(BlackboardManager), "SetBlackboardValue", typeof(string), typeof(int))]
        private static class SetBlackboardPatchGuid
        {
            private static void Postfix(string guid, int value)
            {
                if (OpenSeaOfStarsMod.debug && !string.IsNullOrEmpty(guid))
                {
                    OpenSeaOfStarsMod.OpenInstance.LoggerInstance.Msg($"SET: {guid} {blackboardVariables[guid]}: {value}");
                }
            }
        }
    }
}
