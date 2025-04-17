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
        { "08ae384f758e8be4ba4e7d58975838e6", 1 }, // cookie jar
        { "aefb6b3d640e4804d85814203c8baa2c", 1 }, // map
        { "c9447122a421a2640b315d36b2562ad2", 1 }, // graplou
        { "5b77e66a1d52fce4cbab840d6dd157c4", 1 }, // push bracelet
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