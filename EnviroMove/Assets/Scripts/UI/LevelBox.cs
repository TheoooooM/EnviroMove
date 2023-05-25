using Archi.Service.Interface;
using DG.Tweening;
using Levels;
using TMPro;
using UI.Canvas;
using UnityEngine;
using UnityEngine.UI;

public class LevelBox : MonoBehaviour
{
    private IToolService m_Tool;
    private IDataBaseService m_dataBase;

    [Header("Base map information")]
    [SerializeField] private TextMeshProUGUI levelNameTxt = null;
    [SerializeField] private TextMeshProUGUI creatorNameTxt = null;
    [SerializeField] private Image difficultyImg = null;

    [Header("Social level information")]
    [SerializeField] private Image daylyIcon = null;
    [SerializeField] private Image trendingIcon = null;
    [SerializeField] private TextMeshProUGUI downloadAmountTxt = null;
    [SerializeField] private TextMeshProUGUI likedAmountTxt = null;

    [Header("Reward information")]
    [SerializeField] private TextMeshProUGUI carrotAmountTxt = null;
    [SerializeField] private TextMeshProUGUI goldAmountTxt = null;
    private LevelInfo info;

    /// <summary>
    /// Set up the box
    /// </summary>
    /// <param name="levelInfo"></param>
    /// <param name="toolService"></param>
    /// <param name="dataBaseService"></param>
    public void SetupBox(LevelInfo levelInfo, IToolService toolService, IDataBaseService dataBaseService, MainMenuCanvasUtilities target = null)
    {
        m_dataBase = dataBaseService;
        m_Tool = toolService;
        info = levelInfo;

        levelNameTxt.text = info.levelName;
        creatorNameTxt.text = $"by : {info.creator}";
        //difficultyImg
        
        //dailyIcon
        //trendingIcon
        downloadAmountTxt.text = info.timesPlay.ToString();
        likedAmountTxt.text = info.like.ToString();
        
        carrotAmountTxt.text = info.carrotAmount.ToString();
        goldAmountTxt.text = info.goldValue.ToString();
        if(target != null) GetComponent<Button>().onClick.AddListener(target.SetCurrentPageAndValue);
    }
    
    public void LoadLevelInTool()
    {
        //Debug.Log(info.levelFilePath);
        //Debug.Log(m_dataBase.GetLevel(info.levelFilePath));
        //Debug.Log(m_dataBase.GetLevel(info.levelFilePath).id);
        m_Tool.OpenLevel(m_dataBase.GetLevel(info.levelFilePath));
    }
    
    public void ButtonAnimation(RectTransform button) {
        button.DORewind();
        button.DOPunchScale(new Vector3(-.075f, -.075f, -.075f), .5f / 2f, 1);
    }
}
