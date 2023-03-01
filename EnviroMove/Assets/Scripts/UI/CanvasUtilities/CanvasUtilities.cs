using Archi.Service.Interface;
using Attributes;
using UnityEngine;

namespace UI
{
    public class CanvasUtilities : MonoBehaviour
    {
        [ServiceDependency]public IGameService m_Game;
        [ServiceDependency] private IToolService m_Tool;

        public virtual void Init()
        {
            
        }
        
        public void ChangeScene(int sceneIndex)
        {
            m_Game.ChangeScene((Enums.SceneType)sceneIndex);
        }

        public void ShowTool()
        {
            m_Tool.ShowTool();
        }
        
        public void ShowLevelSelector()
        {
            m_Tool.ShowLevels();
        }

        public void SaveData()
        {
            m_Tool.SaveData();
        }
    }
}
