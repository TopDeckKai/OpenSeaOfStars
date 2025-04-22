using Il2Cpp;
using Il2CppSabotage.Localization;

namespace OpenSeaOfStars.Helpers
{
    public class ActivityHelper
    {
        private OpenSeaOfStarsMod mod;
        private ActivityManager activityManager;

        public ActivityHelper(OpenSeaOfStarsMod mainMod)
        {
            mod = mainMod;
        }

        internal void initActivityManager()
        {
            try
            {
                activityManager = ActivityManager.Instance;
                mod.LoggerInstance.Msg($"Activity Manager Found!!");
            }
            catch
            {
                mod.LoggerInstance.Msg($"Unable to find ActivityManager");
            }

            if (activityManager != null)
            {
                ActivityData archipelagoActivity = new ActivityData();
                archipelagoActivity.name = "Archipelago";
                archipelagoActivity.guid = "ce3bffc8dd7d47a4b9f49d5a85bb218d";
                archipelagoActivity.activityIndex = 46;
                archipelagoActivity.activityNameLoc = LocalizationId.Empty;
                archipelagoActivity.subActivities = new Il2CppSystem.Collections.Generic.List<ActivityData.SubActivity>();

                // TODO: Make this actually work!
                activityManager.allActivityData.Add(new ActivityReference("ce3bffc8dd7d47a4b9f49d5a85bb218d"), archipelagoActivity);

                #if DEBUG
                mod.LoggerInstance.Msg($"ALL ACTIVITIES: " + activityManager.allActivityData.Count);
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<ActivityReference, ActivityData> entry in activityManager.allActivityData)
                {
                    try
                    {
                        mod.LoggerInstance.Msg(entry.Key.activityGuid + "  " + entry.Value.name + "  " + entry.Value.activityIndex + "  " + entry.Value.subActivities.Count + "  " + entry.Value.guid);
                    }
                    catch (System.Exception ex) { mod.LoggerInstance.Msg($"cannot get activity: " + ex.Message + "\n" + ex.StackTrace.ToString()); }
                }
                #endif
            }
        }
    }
}
