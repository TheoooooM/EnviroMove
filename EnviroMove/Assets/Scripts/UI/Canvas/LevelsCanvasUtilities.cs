using Archi.Service.Interface;
using Attributes;
using DefaultNamespace;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Canvas
{
    public class LevelsCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] private IDataBaseService m_Data;
        [ServiceDependency] private ILevelService m_Level;

        [SerializeField] private ConstantLevelSO constantLevels;
        
        public override void Init() { }
        
        public void LoadLevel(string levelName)
        {
            m_Level.LoadLevel(m_Data.GetLevelByName(levelName));
        }

        private LevelData dataToTest;
        public void LoadLevelData(string data)
        {
            //m_Level.LoadLevel((LevelData)constantLevels.GetLevel(i));

            //dataToTest = (LevelData)constantLevels.GetLevel(i);
            dataToTest = (LevelData)data;
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
            m_Level.LoadLevel(dataToTest);
            SceneManager.sceneLoaded -= AsyncTestLevel;
            
        }

    }
}
