using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Canvas
{
    public class GameOverCanvasUtilities : CanvasUtilities
    {

        [ServiceDependency] private IToolService m_tool;
        [ServiceDependency] private ILevelService m_level;
        [ServiceDependency] private IInterfaceService m_thisInterface;

        [SerializeField] private Button nextLevelButton = null;
        [SerializeField] private Button editLevelButton = null;
        private LevelSO nextLevel = null;
        
        public override void Init() {
            nextLevel = m_thisInterface.GetNextLevelSO();
            nextLevelButton.gameObject.SetActive(nextLevel != null);
            editLevelButton.gameObject.SetActive(m_thisInterface.GetTargetPage() == PageDirection.Create);
        }

        public void EditLevel() {
            m_tool.OpenLevel(m_level.GetCurrentLevelData());
        }

        private LevelData dataTemp;
        public void Replay()
        {
            dataTemp = m_level.GetCurrentLevelData();
            SceneManager.sceneLoaded += AsyncTestLevel;
            ChangeScene("InGame");
            
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
            m_thisInterface.SetNextLevelSO(nextLevel.Nextlevel);
            dataTemp = (LevelData)nextLevel.LevelData;
            SceneManager.sceneLoaded += AsyncTestLevel;
            ChangeScene("InGame");
        }
    }
}