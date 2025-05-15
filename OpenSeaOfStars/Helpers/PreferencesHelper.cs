using MelonLoader;
using UnityEngine;

namespace OpenSeaOfStars.Helpers
{
    public class PreferencesHelper
    {
        private static readonly KeyCode DEFAULT_SAVE_KEY = KeyCode.K;
        private static readonly KeyCode DEFAULT_RETURN_TO_TITLE_KEY = KeyCode.P;
        private static readonly KeyCode DEFAULT_TOGGLE_ENCOUNTERS_KEY = KeyCode.E;
        private static readonly KeyCode DEFAULT_RESET_TIME_OF_DAY_FLAG_KEY = KeyCode.Alpha2;
        private static readonly KeyCode DEFAULT_ADD_ZALE_KEY = KeyCode.Z;
        private static readonly KeyCode DEFAULT_ADD_VALERE_KEY = KeyCode.V;
        private static readonly KeyCode DEFAULT_ADD_GARL_KEY = KeyCode.G;
        private static readonly KeyCode DEFAULT_ADD_SERAI_KEY = KeyCode.S;
        private static readonly KeyCode DEFAULT_ADD_RESHAN_KEY = KeyCode.R;
        private static readonly KeyCode DEFAULT_ADD_BST_KEY = KeyCode.B;
        private static readonly KeyCode DEFAULT_ADD_TEAKS_KEY = KeyCode.T;
        private static readonly KeyCode DEFAULT_LOG_INVENTORY_KEY = KeyCode.I;

        private MelonPreferences_Category? general;
        internal MelonPreferences_Entry<bool>? DebugMode { get; private set; }
        internal bool TryDebugModeValue => DebugMode?.Value ?? false;

        private MelonPreferences_Category? commands;
        internal MelonPreferences_Entry<KeyCode>? SaveKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? ReturnToTitleKey {  get; private set; }
        internal MelonPreferences_Entry<KeyCode>? ToggleEncountersKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? ResetTimeOfDayFlagKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddZaleKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddValereKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddGarlKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddSeraiKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddReshanKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddBstKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? AddTeaksKey { get; private set; }
        internal MelonPreferences_Entry<KeyCode>? LogInventoryKey { get; private set; }
        internal KeyCode TrySaveKeyValue => SaveKey?.Value ?? DEFAULT_SAVE_KEY;
        internal KeyCode TryReturnToTitleKeyValue => ReturnToTitleKey?.Value ?? DEFAULT_RETURN_TO_TITLE_KEY;
        internal KeyCode TryToggleEncountersKeyValue => ToggleEncountersKey?.Value ?? DEFAULT_TOGGLE_ENCOUNTERS_KEY;
        internal KeyCode TryResetTimeOfDayFlagKeyValue => ResetTimeOfDayFlagKey?.Value ?? DEFAULT_RESET_TIME_OF_DAY_FLAG_KEY;
        internal KeyCode TryAddZaleKeyValue => AddZaleKey?.Value ?? DEFAULT_ADD_ZALE_KEY;
        internal KeyCode TryAddValereKeyValue => AddValereKey?.Value ?? DEFAULT_ADD_VALERE_KEY;
        internal KeyCode TryAddGarlKeyValue => AddGarlKey?.Value ?? DEFAULT_ADD_GARL_KEY;
        internal KeyCode TryAddSeraiKeyValue => AddSeraiKey?.Value ?? DEFAULT_ADD_SERAI_KEY;
        internal KeyCode TryAddReshanKeyValue => AddReshanKey?.Value ?? DEFAULT_ADD_RESHAN_KEY;
        internal KeyCode TryAddBstKeyValue => AddBstKey?.Value ?? DEFAULT_ADD_BST_KEY;
        internal KeyCode TryAddTeaksKeyValue => AddTeaksKey?.Value ?? DEFAULT_ADD_TEAKS_KEY;
        internal KeyCode TryLogInventoryKeyValue => LogInventoryKey?.Value ?? DEFAULT_LOG_INVENTORY_KEY;

        private MelonPreferences_Category? items;
        private MelonPreferences_Category? flags;
        private MelonPreferences_Category? characters;

        private bool initialized;

        public PreferencesHelper() { } 

        internal void InitMelonPreferences()
        {
            if (initialized) { return; }

            general = MelonPreferences.CreateCategory("OpenSeaOfStars_General");
            DebugMode = general.CreateEntry("DebugMode", true, description: "Whether to enable the mod's debug mode. This enables debugging key commands and some extra logging.");

            commands = MelonPreferences.CreateCategory("OpenSeaOfStars_Commands");
            SaveKey = commands.CreateEntry("Save", DEFAULT_SAVE_KEY);
            SaveKey.Comment = "Save the loaded save slot";
            ReturnToTitleKey = commands.CreateEntry("ReturnToTitle", DEFAULT_RETURN_TO_TITLE_KEY);
            ReturnToTitleKey.Comment = "Return to the title screen";
            ToggleEncountersKey = commands.CreateEntry("ToggleEncounters", DEFAULT_TOGGLE_ENCOUNTERS_KEY);
            ToggleEncountersKey.Comment = "Toggle enemy encounters on or off";
            ResetTimeOfDayFlagKey = commands.CreateEntry("ResetTimeOfDayFlag", DEFAULT_RESET_TIME_OF_DAY_FLAG_KEY);
            ResetTimeOfDayFlagKey.Comment = "Disable Time of Day powers";
            AddZaleKey = commands.CreateEntry("AddZale", DEFAULT_ADD_ZALE_KEY);
            AddZaleKey.Comment = "Add Zale to the party";
            AddValereKey = commands.CreateEntry("AddValere", DEFAULT_ADD_VALERE_KEY);
            AddValereKey.Comment = "Add Valere to the party";
            AddGarlKey = commands.CreateEntry("AddGarl", DEFAULT_ADD_GARL_KEY);
            AddGarlKey.Comment = "Add Garl to the party";
            AddSeraiKey = commands.CreateEntry("AddSerai", DEFAULT_ADD_SERAI_KEY);
            AddSeraiKey.Comment = "Add Serai to the party";
            AddReshanKey = commands.CreateEntry("AddReshan", DEFAULT_ADD_RESHAN_KEY);
            AddReshanKey.Comment = "Add Reshan to the party";
            AddBstKey = commands.CreateEntry("AddBst", DEFAULT_ADD_BST_KEY);
            AddBstKey.Comment = "Add B'st to the party";
            AddTeaksKey = commands.CreateEntry("AddTeaks", DEFAULT_ADD_TEAKS_KEY);
            AddTeaksKey.Comment = "Add Teaks to the party";
            LogInventoryKey = commands.CreateEntry("LogInventory", DEFAULT_LOG_INVENTORY_KEY);
            LogInventoryKey.Comment = "Write inventory items to the log";

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
