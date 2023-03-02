using Archi.Service.Interface;
using Attributes;
using UnityEngine;

namespace UI.Canvas
{
    public class CanvasUtilities : MonoBehaviour
    {
        [ServiceDependency] private IGameService m_Game;
        [ServiceDependency] private IToolService m_Tool;
        [ServiceDependency] private IDataBaseService m_Data;

        public virtual void Init()
        {
            var saver = GetComponentInChildren<SaveTester>();
            if (saver){ saver.m_Database = m_Data; }
        }
        
        public void ChangeScene(int sceneIndex)
        { }

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
