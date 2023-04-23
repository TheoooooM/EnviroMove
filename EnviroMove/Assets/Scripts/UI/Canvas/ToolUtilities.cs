using Archi.Service.Interface;
using Attributes;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LevelData = Levels.LevelData;

namespace UI.Canvas
{
    public class ToolUtilities : CanvasUtilities
    {
        [ServiceDependency] private IToolService m_Tool;
        [SerializeField] private TMP_InputField inputField;

        public override void Init() { }
        
        public void LaunchTool()
        {    
            SceneManager.LoadScene("Tool");
            m_Tool.ShowTool();
        }

        public void ReturnButton()
        {
            m_Tool.ShowLevels();
        }
    
        public void ChangePrefab(int index)
        {
            m_Tool.ChangePrefab(index);
        }

        private LevelData GetEditorData()
        {
            throw new System.NotImplementedException();
        }

        public void SaveDatas()
        {
            m_Tool.SaveData(inputField.text);
        }
        
        public void SwitchMode(int index)
        {
            m_Tool.SwitchMode(index);
        }
        
        public void TestLevel(string sceneName)
        {
            ChangeScene(sceneName);
            m_Tool.TestLevel();
            m_Tool.SetServiceState(false);
            SceneManager.sceneLoaded += (_,_) => m_Tool.TestLevel();
        }
    }
}
