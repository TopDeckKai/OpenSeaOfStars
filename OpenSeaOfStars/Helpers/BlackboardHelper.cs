using Il2Cpp;
using UnityEngine;
using Il2CppSabotage.Blackboard;
using Il2CppSabotage.Localization;
using Il2CppSabotage.Graph.BehaviorTree.Nodes.Leaf.Blackboard;
using HarmonyLib;
using Il2CppSabotage.Graph.Core;
using MelonLoader;
using static UnityEngine.UIElements.StyleVariableResolver;

namespace OpenSeaOfStars.Helpers
{
    public class BlackboardHelper : MelonLogger
    {
        private OpenSeaOfStarsMod mod;
        private BlackboardManager blackboardManager;

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

            if (blackboardManager != null)
            {
                tryReadBlackboardManager();
            }
        }

        internal void tryReadBlackboardManager()
        {
            if (OpenSeaOfStarsMod.debug)
            {
                
            }
        }

        internal void AddBlackboardValue(string key, int value)
        {
            if (!blackboardManager.blackboardValues.ContainsKey(key))
            {
                blackboardManager.blackboardValues.Add(key, value);
            }
            else
            {
                blackboardManager.blackboardValues[key] = value;
            }
        }

        [HarmonyPatch(typeof(BlackboardManager), "GetBlackboardValue", new Type[] { typeof(BlackboardVariable) })]
        private static class GetBlackboardPatch
        {
            private static void Prefix(BlackboardVariable variable)
            {
                if (OpenSeaOfStarsMod.debug && variable != null && (!variable.guid.Equals("8ff406c2ce93c624c8d5a41cfa444937") && !variable.guid.Equals("eade193956f385243bbd0ab47aee2ee9"))) //update thread logs excluded
                {
                    Msg("GET: " + variable.name + "  " + variable.guid + "  ");
                }
            }

            private static void Postfix(BlackboardVariable variable, int __result)
            {
                if (variable != null && (!variable.guid.Equals("8ff406c2ce93c624c8d5a41cfa444937") && !variable.guid.Equals("eade193956f385243bbd0ab47aee2ee9"))) //update thread logs excluded
                {
                    Msg("GET Result: " + __result);
                }
            }
        }

        //TODO: Find a way to get the name from the other (actually used) set blackboard value method (String for uuid instead of BlackboardVariable)
        [HarmonyPatch(typeof(BlackboardManager), "SetBlackboardValue", new Type[] { typeof(BlackboardVariable), typeof(int) })]
        private static class SetBlackboardPatch
        {
            private static void Prefix(BlackboardVariable variable, int value)
            {
                if (OpenSeaOfStarsMod.debug && variable != null)
                {
                    Msg("SET: " + variable.name + "  " + variable.guid + "  ");
                }
            }

            private static void Postfix()
            {
                // The code inside this method will run after 'PrivateMethod' has executed
            }
        }
    }
}
