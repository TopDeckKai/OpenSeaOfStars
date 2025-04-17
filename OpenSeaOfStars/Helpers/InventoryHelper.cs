using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSabotage.Blackboard;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenSeaOfStars.Helpers;

public class InventoryHelper
{
    private OpenSeaOfStarsMod mod;
    public Dictionary<string, InventoryItem> inventoryItems { get; private set; }
    public Dictionary<string, int> startingItems { get; private set; } = new()
    {
        // key items
        { "08ae384f758e8be4ba4e7d58975838e6", 1 }, // cookie jar
        { "aefb6b3d640e4804d85814203c8baa2c", 1 }, // map
        { "c9447122a421a2640b315d36b2562ad2", 1 }, // graplou
        { "5b77e66a1d52fce4cbab840d6dd157c4", 1 }, // mistral bracelet
        { "31525df92a16eb94ba068dff6876c562", 1 }, // trader's signet
        { "f0faa32c4f2b19e4d9a1c825c5a23771", 1 }, // upgraded coral hammer
        
        // relics
        { "1554ef53341beea43ab50edbe869f560", 1 }, // salient sails
        { "e388991e8f7c5934993153ef7e50232f", 1 }, // parrot
        
        // weapons
        { "a27614e2379ef9e4dac1950a51b3f71d", 1 }, // star shards
        { "8f96ab39454c54641812c57f7b332481", 1 }, // sun blade
        { "95ac264461a94a0458a14a39954e3afc", 1 }, // moon bo
        { "807d9f5c8b8f5514abcf261a36dc1711", 1 }, // aetherwood cork
        { "0f0053be4fa1bf84db338a626d751eb2", 1 }, // mooncradle boy's lid
        
        // armor
        { "911324cc7daad5b41a0af317180a42db", 2 }, // eclipse armor
        { "ebe89a4a81e46c245bd98a337df06eb8", 1 }, // vitric simulacrum
        { "d1d1bcd5dbc41d648a812fbc7d1861b2", 2 }, // cosmic cape
        { "86737d6ef697c204187aea8533ec4244", 1 }, // garl's apron
    };

    public InventoryHelper(OpenSeaOfStarsMod mod)
    {
        this.mod = mod;
        inventoryItems = new Dictionary<string, InventoryItem>();
    }

    public void GetInventoryItems()
    {
        Il2CppReferenceArray<Object> objs = ResourcesAPIInternal.FindObjectsOfTypeAll(Il2CppType.From(typeof(InventoryItem)));
        foreach (Object obj in objs)
        {
            InventoryItem item = obj.Cast<InventoryItem>();
            inventoryItems.Add(item.guid, item);
        }
    }

    public void PrintInventoryItems()
    {
        foreach (InventoryItem item in inventoryItems.Values)
        {
            mod.LoggerInstance.Msg($"{item.name}, {item.guid}");
        }
    }
}