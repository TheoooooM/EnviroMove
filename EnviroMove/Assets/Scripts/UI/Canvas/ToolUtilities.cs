using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEngine.SceneManagement;
using LevelData = Levels.LevelData;

namespace UI.Canvas
{
    public class ToolUtilities : CanvasUtilities
    {
        [ServiceDependency] private IToolService m_Tool;

        public override void Init() { }
        
        public void LaunchTool()
        {    
            SceneManager.LoadScene("Tool");
            m_Tool.ShowTool();
        }
    
        public void ChangePrefab(int index)
        {
            m_Tool.ChangePrefab(index);
        }

        private LevelData GetEditorData()
        {
            throw new System.NotImplementedException();
        }

        
    }
}
