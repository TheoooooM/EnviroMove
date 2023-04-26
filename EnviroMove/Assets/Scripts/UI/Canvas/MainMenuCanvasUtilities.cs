using Archi.Service.Interface;
using Attributes;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UI.Canvas
{
    public class MainMenuCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] IToolService m_Tool;
        [ServiceDependency] IDataBaseService m_Data;
        
        
        [SerializeField] private TMP_InputField inputField;
        
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

        public void SetUsername()
        {
            if(inputField.text != "")m_Data.SetUsername(inputField.text);
        }
    }
}
