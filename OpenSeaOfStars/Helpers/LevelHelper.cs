using Il2Cpp;
using UnityEngine;

namespace OpenSeaOfStars.Helpers;

public class LevelHelper
{
    private class LevelPatch
    {
        public string levelUUID = "";
        public bool isBoat = false;
        public Vector3 position = Vector3.zero;
    }

    private Dictionary<string, LevelPatch> levelReferences = new() {
        { "SeaOfNightmare_WorldMap" , new LevelPatch { levelUUID = "539d52ad02b071c45837d1329f8cbcc5", isBoat = true, position = new Vector3(6.5f, 0f, -15f) } },
        { "StormCallerIslandDefinition" , new LevelPatch { levelUUID = "3d1c3e6c6c2511743ac0278f551d299c", isBoat = false, position = new Vector3(17.7412f, 21.002f, 15.5927f) } }
    };

    public void loadLevel(string levelName)
    {
        if (!levelReferences.Keys.Any(name => name.Equals(levelName)))
        {
            return;
        }
            
        LevelPatch levelPatch = levelReferences[levelName];

        Il2CppSystem.Collections.Generic.Dictionary<LevelReference, LevelDefinition>.KeyCollection levelDefinitionKeys = LevelManager.Instance.levelDefinitionPerLevel.Keys;
        foreach (LevelReference levRef in levelDefinitionKeys)
        {
            if (levRef.levelDefinitionGuid.Equals(levelPatch.levelUUID))
            {
                LevelLoading levelLoading = new()
                {
                    levelToLoad = levRef,
                    showTransition = true,
                    removeTransitionWhenDone = true
                };

                GameplayLevelInitializerParams initializerParams = new()
                {
                    isBoat = levelPatch.isBoat,
                    spawnPosition = levelPatch.position,
                    spawnPositionSet = true
                };

                levelLoading.initializerParams = initializerParams;

                LevelManager.Instance.LoadLevel(levelLoading);
                break;
            }
        }
    }
}