using Archi.Service.Interface;
using Attributes;
using Levels;
using UI;
using UnityEngine;

public class LevelSelectorCanvasUtilities : CanvasUtilities
{
    [ServiceDependency] private IDataBaseService m_Data;
    
    [SerializeField] private Transform layout;
    [SerializeField] private GameObject levelBox;

    public override void Init()
    {
        LevelInfo[] infos = m_Data.GetAllLevelInfos();
        Debug.Log($"Get {infos.Length} infos");
        foreach (var info in infos)
        {
            var go = Instantiate(levelBox, layout);
            var box = go.GetComponent<LevelBox>();
            Debug.Log(box);
            Debug.Log(box.text);
            Debug.Log(box.text.text);
            Debug.Log(info);
            box.text.text = info.levelName;
            Debug.Log($"Create {info.levelName} levelBox");
        }
    }
}
