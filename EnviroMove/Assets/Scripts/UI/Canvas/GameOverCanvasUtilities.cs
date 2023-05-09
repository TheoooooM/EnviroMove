using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Canvas
{
    public class GameOverCanvasUtilities : CanvasUtilities
    {

        [ServiceDependency] private IToolService m_tool;
        [ServiceDependency] private ILevelService m_level;
        
        public override void Init()
        {
            
        }

        public void EditLevel()
        {
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

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1)
        {
            m_level.LoadLevel(dataTemp);
            dataTemp = null;
            SceneManager.sceneLoaded -= AsyncTestLevel;
            
        }
    }
}