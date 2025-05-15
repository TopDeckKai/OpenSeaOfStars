using MelonLoader;

namespace OpenSeaOfStars.Helpers
{
    public class PreferencesHelper
    {
        private MelonPreferences_Category? general;
        internal MelonPreferences_Entry<bool>? DebugMode { get; private set; }
        internal bool TryDebugModeValue => DebugMode?.Value ?? false;

        private MelonPreferences_Category? commands;
        private MelonPreferences_Category? items;
        private MelonPreferences_Category? flags;
        private MelonPreferences_Category? characters;

        private bool initialized;

        public PreferencesHelper() { } 

        internal void InitMelonPreferences()
        {
            if (initialized) {  return; }

            general = MelonPreferences.CreateCategory("OpenSeaOfStars_General");
            DebugMode = general.CreateEntry("DebugMode", true, description: "Whether to enable the plugin's debug mode. This enables debugging key commands and some extra logging.");

            commands = MelonPreferences.CreateCategory("OpenSeaOfStars_Commands");
            items = MelonPreferences.CreateCategory("OpenSeaOfStars_Items");
            flags = MelonPreferences.CreateCategory("OpenSeaOfStars_Flags");
            characters = MelonPreferences.CreateCategory("OpenSeaOfStars_Characters");

            initialized = true;
        }

        internal static void SaveMelonPreferences()
        {
            MelonPreferences.Save();
        }
    }
}
