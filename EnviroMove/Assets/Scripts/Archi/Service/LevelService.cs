using Archi.Service.Interface;
using Attributes;
using Levels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using LevelData = Levels.LevelData;

namespace Archi.Service
{
    public class LevelService : Service, ILevelService
    {
        [DependeOnService] private IInterfaceService m_Interface;

        protected override void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneInit;
        }

        protected override void OnSceneInit(Scene scene, LoadSceneMode loadMode)
        {
            Debug.Log($"LoadScene : {scene.name}");
            if (scene.name == "Levels")
            {
                m_Interface.DrawCanvas(Enums.MajorCanvas.levels);
            }
        }

        public Level LoadLevel(LevelData data, GameObject levelContainer = null)
        {
            m_Interface.GenerateLoadingScreen("Load Level", 1);
            Level level;
            if (levelContainer) level = levelContainer.AddComponent<Level>();
            else
            {
                var go = Object.Instantiate(new());
                level = go.AddComponent<Level>();
            }
            SetObjectDependencies(level);
            level.onFinishGenerate += OnLoadLevel;
            level.levelData = data;
            level.GenerateLevel(data);
            return level;
        }

        void OnLoadLevel()
        {
           m_Interface.HideLoadingScreen(); 
        }

        public LevelData GetData(Level level)
        {
            return level.levelData;
        }
    }
}