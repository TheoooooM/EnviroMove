using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEngine;

namespace UI.Canvas
{
    //IInterfaceService
    public class ToolLevelsCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] private IDataBaseService m_Data;
        [ServiceDependency] private IToolService m_Tool;
    
        [SerializeField] private Transform layout;
        [SerializeField] private GameObject levelBox;

        public override void Init()
        {
            LevelInfo[] infos = m_Data.GetAllLevelInfos();
            Debug.Log($"Get {infos.Length} infos");
            foreach (var info in infos)
            {
                var go = Object.Instantiate(levelBox, layout);
                var box = go.GetComponent<LevelBox>();
                box.SetupBox(info, m_Tool, m_Data);
            }
        }
    }
}
