using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Canvas
{
    public class GameOverCanvasUtilities : CanvasUtilities {
        [ServiceDependency] private IToolService m_tool;
        [ServiceDependency] private ILevelService m_level;
        [ServiceDependency] private IInterfaceService m_thisInterface;

        [SerializeField] private Button nextLevelButton = null;
        [SerializeField] private Button editLevelButton = null;
        [SerializeField] private MenuType menuType = MenuType.GameOver;
        private LevelSO nextLevel = null;
        
        public override void Init() {
            nextLevel = m_thisInterface.GetNextLevelSO();
            if(nextLevelButton != null) nextLevelButton.gameObject.SetActive(nextLevel != null && menuType == MenuType.Win);
            if(editLevelButton != null) editLevelButton.gameObject.SetActive(m_thisInterface.GetTargetPage() == PageDirection.Create);
        }

        public void EditLevel() {
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                m_tool.OpenLevel(m_level.GetCurrentLevelData());
            });
        }

        private LevelData dataTemp;
        public void Replay(bool fromPause) {
            Time.timeScale = 1;
            dataTemp = m_level.GetCurrentLevelData();
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeScene("InGame");
            });

            //throw new NotImplementedException();
            /*
            m_Tool.TestLevel();
            m_Tool.SetServiceState(false);
            SceneManager.sceneLoaded += (_,_) => m_Tool.TestLevel();*/
        }

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1) {
            m_level.LoadLevel(dataTemp);
            dataTemp = null;
            SceneManager.sceneLoaded -= AsyncTestLevel;
        }

        /// <summary>
        /// Load the next level
        /// </summary>
        public void LoadNextLevel() {
            Time.timeScale = 1;
            m_thisInterface.SetNextLevelSO(nextLevel.Nextlevel);
            dataTemp = (LevelData)nextLevel.LevelData;
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeScene("InGame");
            });
        }

        public void SetMLevel(ILevelService m_levelUp , IInterfaceService interfaceService) {
            m_level = m_levelUp;
            m_thisInterface = interfaceService;
        }

        /// <summary>
        /// Change Scene
        /// </summary>
        /// <param name="sceneName"></param>
        public void GoToMainMenu(string sceneName) {
            Time.timeScale = 1;
            
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                //SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeSceneWithClouds(sceneName, m_thisInterface);
            });
        }
    }
}

public enum MenuType {
    GameOver,
    Win,
    Pause
}