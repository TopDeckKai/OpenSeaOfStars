using Il2Cpp;

namespace OpenSeaOfStars.Helpers;

public class LevelHelper
{
    private class LevelPatch
    {
        public string levelUUID = "";
    }

    private Dictionary<string, LevelPatch> levelReferences = new() {
        { "SeaOfNightmare_WorldMap" , new LevelPatch { levelUUID = "539d52ad02b071c45837d1329f8cbcc5" } }
    };

    public void loadLevel(string levelName)
    {
        if (!levelReferences.Keys.Any(name => name.Equals(levelName)))
        {
            return;
        }
            
        LevelPatch levelPatch = levelReferences[levelName];

        // For some reason, cannot look into without indexing :(
        foreach (Il2CppSystem.Collections.Generic.KeyValuePair<LevelReference, LevelDefinition> levRef in LevelManager.Instance.levelDefinitionPerLevel)
        {
            if (levRef.key.levelDefinitionGuid.Equals(levelPatch.levelUUID))
            {
                LevelLoading levelLoading = new()
                {
                    levelToLoad = levRef.key,
                    showTransition = true,
                    removeTransitionWhenDone = true
                };

                GameplayLevelInitializerParams initializerParams = new()
                {
                    isBoat = true,
                    spawnPosition = new UnityEngine.Vector3 (6.5f, 0f, -15f),
                    spawnPositionSet = true
                };

                levelLoading.initializerParams = initializerParams;

                LevelManager.Instance.LoadLevel(levelLoading);
                break;
            }
        }
    }
}