using System;
using Il2Cpp;
using Il2CppSystem.Linq;
using UnityEngine.Animations;

namespace OpenSeaOfStars.Helpers
{
    public class LevelHelper
    {
        private class LevelPatch
        {
            public string levelUUID = "";
        }

        private Dictionary<string, LevelPatch> levelReferences = new Dictionary<string, LevelPatch>() {
            { "SeaOfNightmare_WorldMap" , new LevelPatch() { levelUUID = "539d52ad02b071c45837d1329f8cbcc5" } }
        };

        public void loadLevel(string levelName)
        {
            if (levelReferences.Keys.Any(name => name.Equals(levelName))) {
                LevelPatch levelPatch = levelReferences[levelName];

                // For some reason, cannot look into without indexing :(
                foreach (LevelReference levRef in LevelManager.Instance.levelDefinitionPerLevel.Keys)
                {
                    if (levRef.levelDefinitionGuid.Equals(levelPatch.levelUUID))
                    {
                        LevelLoading levelLoading = new LevelLoading();
                        levelLoading.levelToLoad = levRef;
                        levelLoading.showTransition = true;
                        levelLoading.removeTransitionWhenDone = true;

                        GameplayLevelInitializerParams initializerParams = new GameplayLevelInitializerParams();
                        initializerParams.isBoat = true;
                        initializerParams.spawnPosition = new UnityEngine.Vector3 (6.5f, 0f, -15f);
                        initializerParams.spawnPositionSet = true;

                        levelLoading.initializerParams = initializerParams;

                        LevelManager.Instance.LoadLevel(levelLoading);
                        break;
                    }
                }
            }
        }
    }
}
