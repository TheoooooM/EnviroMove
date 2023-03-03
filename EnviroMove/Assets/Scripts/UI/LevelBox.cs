using Archi.Service.Interface;
using Levels;
using TMPro;
using UnityEngine;

public class LevelBox : MonoBehaviour
{
    private IToolService m_Tool;
    private IDataBaseService m_dataBase;

    private TMP_Text text;
    private LevelInfo info;

    public void SetupBox(LevelInfo levelInfo, IToolService toolService, IDataBaseService dataBaseService)
    {
        m_dataBase = dataBaseService;
        m_Tool = toolService;

        text = GetComponentInChildren<TMP_Text>();
        info = levelInfo;
        text.text = levelInfo.levelName;
    }
    
    public void LoadLevelInTool()
    {
        Debug.Log(info.levelFilePath);
        Debug.Log(m_dataBase.GetLevel(info.levelFilePath));
        Debug.Log(m_dataBase.GetLevel(info.levelFilePath).id);
        m_Tool.OpenLevel(m_dataBase.GetLevel(info.levelFilePath));
    }
}
