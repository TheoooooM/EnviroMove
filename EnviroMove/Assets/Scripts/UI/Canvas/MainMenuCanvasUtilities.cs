using Archi.Service.Interface;
using Attributes;
using UnityEditor;

namespace UI.Canvas
{
    public class MainMenuCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] IToolService m_Tool;
        [ServiceDependency] IDataBaseService m_Data;
        
        public override void Init()
        {
            var saver = GetComponentInChildren<SaveTester>();
            if (saver){ saver.m_Database = m_Data; }
        }
        
        public void ShowLevels()
        {
            
        }
        
        public void ShowLevelSelector()
        {
            m_Tool.ShowLevels();
        }
    }
}
