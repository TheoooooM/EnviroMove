using System;
using System.Collections;
using System.Collections.Generic;
using Archi.Service.Interface;
using Attributes;
using DG.Tweening;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Canvas
{
    public class MainMenuCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] private ILevelService m_Level;
        [ServiceDependency] private IToolService m_Tool;
        [ServiceDependency] private IDataBaseService m_Data;
        [ServiceDependency] private IInterfaceService m_thisInterface;

        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float popUpAnimationDuration = 0.25f;
        [SerializeField] private float animationAmplitude = 1.25f;
        
        [Header("Base Viewport Information")]
        public UnityEngine.Canvas mainCanvas = null;
        [SerializeField] private RectTransform mainMenuTransform = null;

        [Header("- Home Panel Information")]
        [SerializeField] private TextMeshProUGUI playerName = null;
        
        [Header("Bottom Bar Information")]
        [SerializeField] private List<RectTransform> bottomBarButtonsRect = new();
        [SerializeField] private Vector2 buttonSizes = new();
        [SerializeField, Range(0,1 )] private float changeSizeSpeed = 0.1f;
        
        [Header("Movement Viewport Data")]
        [SerializeField] private RectTransform viewportTransform = null;
        [SerializeField] private Vector2 minMaxViewportXValue = new();
        [SerializeField, Range(0, 1)] private float changeScreenSpeed = 0.2f;
        [SerializeField] private ScrollRect shopScrollRect = null;
        [SerializeField] private ScrollRect commuScrollRect = null;
        private bool isInCreateMenu = false;

        [Header("- Shop Panel Information- ")]
        [SerializeField] private TextMeshProUGUI specialOfferTxt = null;
        [SerializeField] private TextMeshProUGUI bundleOfferTxt = null;
        [SerializeField] private TextMeshProUGUI daysOfferTxt = null;
        private DateTime currentTime;
        
        [Header("- Community Panel Information- ")]
        [SerializeField] private TextMeshProUGUI specialEventTxt = null;
        [SerializeField] private TextMeshProUGUI otherEventTxt = null;
        [SerializeField] private TextMeshProUGUI mapOfDayTxt = null;
        
        [Header("- Rewards Panel information- ")]
        [SerializeField] private CanvasGroup rewardGroup = null;
        [SerializeField] private Image rewardImg = null;
        [SerializeField] private TextMeshProUGUI rewardTxt = null;
        [SerializeField] private List<Sprite> goldRewardSprite = new();
        [SerializeField] private List<Sprite> skinRewardSprite = new();
        [SerializeField] private TextMeshProUGUI goldTxt = null;
        public bool isInReward = false;
        private int stageReward = 0;

        [Header("- LevelCreate Panel Information- ")]
        [SerializeField] private RectTransform createMenuTransform = null;
        [SerializeField] private RectTransform moreLevelCreatedTransform = null;
        [SerializeField] private RectTransform maskLevelCreatedTransform = null;
        [SerializeField] private GameObject levelBox = null;
        [SerializeField] private RectTransform contentBox = null;
        [SerializeField] private RectTransform contentPanelBox = null;
        [SerializeField] private GameObject showMoreBtn = null;
        private bool isMoreLevelPanelOpen = false;

        [Header("Username Information")]
        [SerializeField] private GameObject usernameGam = null;
        [SerializeField] private Button applyUsernameBtn = null;
        [SerializeField] private TMP_InputField usernameInputField;
        private bool isInUsername = false;

        [SerializeField] private List<RectTransform> shopBtnsPunch = new();
        [SerializeField] private List<RectTransform> homeBtnPunch = new();
        [SerializeField] private List<RectTransform> commuBtnPunch = new();
        [SerializeField] private List<RectTransform> createBtnPunch = new();
        
        [Header("Player Skin")]
        [SerializeField] private SkinnedMeshRenderer skinRend = null;
        [SerializeField] private Transform playerTrans = null;
        [SerializeField] private AnimationCurve playerSizeCurve = null;
        [SerializeField] private List<Material> skinMat = new();
        [SerializeField] private List<Sprite> banners = new();
        [SerializeField] private Image bannerImg = null;

        [Header("DoTween Information")]
        [SerializeField] private float waitTimeShop = 0.4f;
        [SerializeField] private float timeToGoToShop = 0.75f;
        
        /// <summary>
        /// Initialize all the data for the script
        /// </summary>
        public override void Init() {
            Time.timeScale = 1;
            SetBanner();
            var saver = GetComponentInChildren<SaveTester>();
            if (saver){ saver.m_Database = m_Data; }

            if (PlayerPrefs.GetInt("HasMadeTutorial", 0) == 0) SetPageToUsername();
            playerName.text = PlayerPrefs.GetString("Username");
            
            ResetCreateMenu();
            LaunchAllAnimation();
            UpdatePlayerSkin();
            InitSkinSize();

            if (PlayerPrefs.GetInt("GoldReward", 0) > 0) {
                StartCoroutine(WaitForReward());
            }
            GameObject.FindWithTag("Splash").SetActive(false);
        }
        
        private IEnumerator WaitForReward() {
            yield return new WaitForSeconds(1.5f);
            GetRewards(RewardType.Gold, $"{PlayerPrefs.GetInt("GoldReward")} golds", PlayerPrefs.GetInt("GoldReward"));
            PlayerPrefs.SetInt("GoldReward", 0);
        }
        
        #region SetPage
        /// <summary>
        /// Initialize the position of the player based on the last position while the game was open
        /// </summary>
        public void InitValue(PageDirection pageDirection, float value) {
            Debug.Log(pageDirection + " " + value);
            switch (pageDirection) {
                case PageDirection.Shop:
                    pageID = 1;
                    shopScrollRect.verticalNormalizedPosition = value;
                    break;
                
                case PageDirection.Home:
                    pageID = 0;
                    isInCreateMenu = false;
                    break;
                
                case PageDirection.Community:
                    pageID = -1;
                    commuScrollRect.verticalNormalizedPosition = value;
                    break;
                
                case PageDirection.Search:
                    pageID = -2;
                    break;
                
                case PageDirection.Create:
                    pageID = 0;
                    isInCreateMenu = true;
                    break;
                
                default:
                    break;
            }
            viewportTransform.localPosition = GetTargetPosition();
        }

        /// <summary>
        /// Set the current page and value
        /// </summary>
        public void SetCurrentPageAndValue() {
            PageDirection pageDirection = pageID switch {
                -2 => PageDirection.Search,
                -1 => PageDirection.Community,
                0 => isInCreateMenu ? PageDirection.Create : PageDirection.Home,
                1 => PageDirection.Shop,
                _ => PageDirection.Home
            };

            float value = pageDirection switch {
                PageDirection.Shop => shopScrollRect.verticalNormalizedPosition,
                PageDirection.Community => commuScrollRect.verticalNormalizedPosition,
                _ => 0f
            };
            m_thisInterface.SetTargetPage(pageDirection, value);
        }

        /// <summary>
        /// Set the page to username
        /// </summary>
        private void SetPageToUsername() {
            isInUsername = true;
            playerTrans.gameObject.SetActive(false);
            usernameGam.SetActive(true);
        }
        #endregion SetPage
        
        #region Creation Level Buttons
        /// <summary>
        /// Reset the create buttons
        /// </summary>
        private void ResetCreateMenu() {
            for (int i = contentBox.childCount - 1; i > 0; i--) {
                Destroy(contentBox.GetChild(i).gameObject);
            }
            
            CreateLevelBtns();
        }

        /// <summary>
        /// Create the buttons for the level the player create
        /// </summary>
        private void CreateLevelBtns() {
            m_Data.RefreshLevelData();
            LevelInfo[] infos = m_Data.GetAllLevelInfos();
            showMoreBtn.SetActive(infos.Length > 3);
            
            int numberOfObjectToSpawn = Mathf.Abs(Mathf.FloorToInt(contentBox.rect.height / 600f));
            for (var index = 0; index < Mathf.Clamp(infos.Length,0,numberOfObjectToSpawn); index++) {
                CreateButton(contentBox, infos, index);
            }
        }
        
        /// <summary>
        /// Create a button in the right target
        /// </summary>
        /// <param name="targetTransform"></param>
        /// <param name="index"></param>
        private Transform CreateButton(Transform targetTransform, LevelInfo[] infos, int index) {
            GameObject go = Instantiate(levelBox, targetTransform);
            var box = go.GetComponent<LevelBox>();
            var info = infos[index];
            box.SetupBox(info, m_Tool, m_Data, this);
            return go.transform;
        }
        #endregion Creation Level Buttons
        
        private MovementDirection moveDir = MovementDirection.None;
        private Vector2 startPosition = new();
        private bool hasMove = false;
        private int pageID = 0;
        
        private void Update() {
            if (isInUsername) {
                applyUsernameBtn.interactable = usernameInputField.text != "";
                return;
            }
            MoveCurrentPageWithInput();
            ChangeBottomBarButtonSize();
            UpdateTimeOffers();
            
            goldTxt.text = PlayerPrefs.GetInt("Gold", 0).ToString();
            createMenuTransform.localPosition = new Vector3(0, -mainMenuTransform.sizeDelta.y, 0);
        }
        
        private Vector3 viewportStartPosition = new();
        private Vector2 delatPos = new();
        private Vector2 position = new();
        private GameObject openedPanel = null;

        /// <summary>
        /// Move the current page when the player swipe the screen
        /// </summary>
        private void MoveCurrentPageWithInput() {
            if (Input.touchCount == 0) {
                moveDir = MovementDirection.None;
                viewportTransform.localPosition = Vector3.Lerp(viewportTransform.localPosition, GetTargetPosition(), changeScreenSpeed);
                return;
            }

            //Set the variable of the current page
            InitCurrentMovement();

            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    InitInputs();
                    if (isMoreLevelPanelOpen) {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(maskLevelCreatedTransform, position, mainCanvas.worldCamera, out Vector2 rectPos);
                        if(!maskLevelCreatedTransform.rect.Contains(rectPos)) OpenCloseMoreLevels(false);
                    }

                    if (isInReward) {
                        if(stageReward == 3) CloseRewardPanel();
                    }
                    break;
                
                case TouchPhase.Moved:
                    if (Mathf.Abs(delatPos.y) > Mathf.Abs(delatPos.x) && moveDir == MovementDirection.None) moveDir = MovementDirection.Y;
                    else if(Mathf.Abs(delatPos.y) <= Mathf.Abs(delatPos.x) && moveDir == MovementDirection.None) moveDir = MovementDirection.X;
                    hasMove = true;
                    
                    if (CantMovePanel()) return;
                    if(isInReward || isMoreLevelPanelOpen) return;
                    if (moveDir == MovementDirection.Y) {
                        if (pageID != 0) return;
                        if (viewportTransform.localPosition.y <= 0 && delatPos.y < 0 && !isInCreateMenu) return; 
                        if (viewportTransform.localPosition.y >= mainMenuTransform.sizeDelta.y && delatPos.y > 0 && isInCreateMenu) return;
                        
                        viewportTransform.localPosition += new Vector3(0, delatPos.y, 0);
                        return;
                    }
                    if (isInCreateMenu) return;
                    viewportTransform.localPosition += new Vector3(delatPos.x, 0, 0);
                    break;
                
                case TouchPhase.Ended:
                    if (!hasMove) return;
                    hasMove = false;
                    MovementDirection currentMove = moveDir;
                    moveDir = MovementDirection.None;
                    
                    if (currentMove == MovementDirection.Y) {
                        if (Mathf.Abs(viewportTransform.localPosition.y - viewportStartPosition.y) < mainMenuTransform.sizeDelta.y / 40f) return;
                        isInCreateMenu = !isInCreateMenu;
                        return;
                    }

                    if (Mathf.Abs(viewportTransform.localPosition.x - viewportStartPosition.x) < mainMenuTransform.sizeDelta.x / 30f) return;
                    pageID = (position.x - startPosition.x) switch {
                        < 0 when viewportTransform.localPosition.x > minMaxViewportXValue.x => Mathf.Clamp(pageID - 1, -2, 1),
                        > 0 when viewportTransform.localPosition.x < minMaxViewportXValue.y => Mathf.Clamp(pageID + 1, -2, 1),
                        _ => pageID
                    };

                    break;
                
                case TouchPhase.Stationary: break;
                case TouchPhase.Canceled: break;
            }
        }

        #region Panel Movement Helper
        /// <summary>
        /// Init the variable for the current page movement
        /// </summary>
        private void InitCurrentMovement() {
            delatPos = Input.GetTouch(0).deltaPosition;
            position = Input.GetTouch(0).position;
        }
        /// <summary>
        /// Set variables when the player start touching the screen
        /// </summary>
        private void InitInputs() {
            startPosition = position;
            viewportStartPosition = viewportTransform.localPosition;
            if (openedPanel != null) {
                openedPanel.transform.DOScale(0, popUpAnimationDuration);
                StartCoroutine(RemovePanelFromVariable(openedPanel));
            }
        }
        /// <summary>
        /// Check if the panel can be moved
        /// </summary>
        /// <returns></returns>
        private bool CantMovePanel() {
            return (viewportTransform.localPosition.x >= minMaxViewportXValue.y && delatPos.x > 0) || (viewportTransform.localPosition.x <= minMaxViewportXValue.x && delatPos.x < 0);
        }

        /// <summary>
        /// Get the target position of the viewport
        /// </summary>
        private Vector3 GetTargetPosition() => new(mainMenuTransform.sizeDelta.x * pageID, isInCreateMenu ? mainMenuTransform.sizeDelta.y : 0, 0);
        #endregion Panel Movement Helper
        
        public void SetUsername() {
            if (usernameInputField.text != "") {
                PlayerPrefs.SetInt("HasMadeTutorial", 1);
                m_Data.SetUsername(usernameInputField.text);
            }
        } 
        
        #region Information Panel
        /// <summary>
        /// Open an information panel
        /// </summary>
        /// <param name="informationPanel"></param>
        public void OpenCloseInformationPanel(GameObject informationPanel) {
            if (openedPanel == informationPanel) return;
            
            openedPanel = informationPanel;
            openedPanel.transform.DOScale(1, popUpAnimationDuration).SetEase(Ease.OutBack);
        }

        /// <summary>
        /// Remove the panel from the variable
        /// </summary>
        /// <returns></returns>
        private IEnumerator RemovePanelFromVariable(GameObject panel) {
            yield return new WaitForSeconds(.25f);
            if(openedPanel == panel) openedPanel = null;
        }
        #endregion Information Panel
        
        /// <summary>
        /// Change the size of the buttons at the bottom bar
        /// </summary>
        private void ChangeBottomBarButtonSize() {
            int currentPage = (pageID * -1) + 1;
            for (int i = 0; i < bottomBarButtonsRect.Count; i++) {
                Vector2 buttonSize = bottomBarButtonsRect[i].sizeDelta;
                bottomBarButtonsRect[i].sizeDelta = Vector2.Lerp(buttonSize, new Vector2((i == currentPage ? buttonSizes.y : buttonSizes.x), buttonSize.y), changeSizeSpeed);
            }
        }

        private void CreateButtonsForLevelCreated() {
            m_Data.RefreshLevelData();
            LevelInfo[] infos = m_Data.GetAllLevelInfos();
            contentPanelBox.sizeDelta = new Vector2(contentPanelBox.sizeDelta.x, (buttonSizes.y + 50) * infos.Length);
            contentPanelBox.localPosition = new Vector3(0, 0, 0);

            Sequence levelApparitionSequence = DOTween.Sequence();
            for (var index = 0; index < infos.Length; index++) {
                Transform btn = CreateButton(contentPanelBox, infos, index);
                btn.transform.localScale = Vector3.zero;
                if(index <= 7) levelApparitionSequence.Append(btn.DOScale(1, popUpAnimationDuration / 1.6f).SetEase(Ease.OutBack));
                else btn.transform.localScale = new Vector3(1,1,1);
            }
        }

        /// <summary>
        /// Animate the button
        /// </summary>
        /// <param name="button"></param>
        public void ButtonAnimation(RectTransform button) {
            button.DORewind();
            button.DOPunchScale(new Vector3(-.075f, -.075f, -.075f), animationDuration / 2f, 1);
        }
        
        #region LoadLevel
        private LevelData dataToTest;
        public void LoadLevel(string levelName) => m_Level.LoadLevel(m_Data.GetLevelByName(levelName));

        /// <summary>
        /// Load the level data
        /// </summary>
        /// <param name="data"></param>
        public void LoadLevelData(string data) {
            dataToTest = (LevelData)data;
            SceneManager.sceneLoaded += AsyncTestLevel;
            ChangeScene("InGame");
        }
        public void LoadLevelData(LevelSO data) {
            m_thisInterface.SetNextLevelSO(data.Nextlevel, data.Id);
            dataToTest = (LevelData)data.LevelData;
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeScene("InGame");
            });
        }

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1) {
            m_Level.LoadLevel(dataToTest);
            SceneManager.sceneLoaded -= AsyncTestLevel;
        }
        #endregion LoadLevel
        
        #region Rewards
        /// <summary>
        /// Open the reward panel and show the reward on screen
        /// </summary>
        public void GetRewards(RewardType type, string rewardName, int rewardID = 0, int goldCost = 0) {
            Sequence rewardSequence = DOTween.Sequence();
            rewardGroup.gameObject.SetActive(true);

            rewardSequence.Append(rewardGroup.DOFade(1, popUpAnimationDuration).OnComplete(() => stageReward = 1));
            rewardSequence.AppendInterval(popUpAnimationDuration);

            switch (type) {
                case RewardType.Gold:
                    rewardImg.sprite =  rewardID switch {
                        400 => goldRewardSprite[0],
                        1300 => goldRewardSprite[1],
                        6600 => goldRewardSprite[2],
                        14000 => goldRewardSprite[3],
                        30000 => goldRewardSprite[4],
                        62000 => goldRewardSprite[5],
                        _ => goldRewardSprite[3]
                    };
                    PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold", 0) + rewardID);
                    break;
                
                case RewardType.Skin:
                    AddSkin(rewardID);

                    rewardImg.sprite = rewardID switch {
                        1 => skinRewardSprite[0],
                        2 => skinRewardSprite[1],
                        5 => skinRewardSprite[2],
                        _ => throw new ArgumentOutOfRangeException(nameof(rewardID))
                    };
                    break;
            }
            
            if(goldCost > 0) PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold", 0) - goldCost);

            rewardTxt.text = rewardName;
            rewardSequence.Append(rewardImg.rectTransform.DOScale(1, popUpAnimationDuration).SetEase(Ease.OutBack).OnComplete(() => {
                stageReward = 2;
                rewardImg.rectTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 1, 1).SetLoops(-1).SetAutoKill(false);
            }));
            rewardSequence.Append(rewardTxt.rectTransform.DOScale(1, popUpAnimationDuration).SetEase(Ease.OutBack).OnComplete(() => stageReward = 3));
            isInReward = true;
        }

        /// <summary>
        /// Close the reward panel
        /// </summary>
        private void CloseRewardPanel() {
            rewardImg.rectTransform.DOKill();
            rewardImg.rectTransform.localScale = Vector3.zero;
            rewardTxt.rectTransform.localScale = Vector3.zero;
            rewardGroup.DOFade(0, popUpAnimationDuration).OnComplete(() => rewardGroup.gameObject.SetActive(false));
            isInReward = false;
            stageReward = 0;
        }
        #endregion Rewards

        #region Button Events
        /// <summary>
        /// Wait before doing anything
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator WaitForAction(UnityAction action) {
            yield return new WaitForSeconds(0.075f);
            action.Invoke();
        }
        
        public void OpenCloseCampaignLevels(bool open) => StartCoroutine(WaitForAction(() => OpenCloseCampaign(open)));
        public void OpenCloseCreatedLevels(bool open) => StartCoroutine(WaitForAction(() => OpenCloseMoreLevels(open)));
        public void GoToCreatePanel(bool goCreate) => StartCoroutine(WaitForAction(() => GoToCreate(goCreate)));
        public void GoToPage(int pageID) => StartCoroutine(WaitForAction(() => MoveToPage(pageID)));
        public void OpenEditorTool() => StartCoroutine(WaitForAction(OpenTool));
        public void GoBuyCurrency() => StartCoroutine(WaitForAction(GoToCurrencyPos));
        #endregion Button Events

        #region OpenClose Panels
        /// <summary>
        /// Open or close the selection level panel
        /// </summary>
        /// <param name="open"></param>
        private void OpenCloseCampaign(bool open) {
            m_thisInterface.GenerateLoadingScreen("Roadmap", 3, () => {
                SceneManager.sceneLoaded += DrawRoadMap;
                ChangeScene("RoadMap");
            });
        }

        /// <summary>
        /// Draw the roadmap
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private void DrawRoadMap(Scene arg0, LoadSceneMode arg1) {
            SceneManager.sceneLoaded -= DrawRoadMap;
            m_thisInterface.DrawCanvas(Enums.MajorCanvas.RoadMap);
        }

        /// <summary>
        /// Open or close the panel which contains all the created levels
        /// </summary>
        /// <param name="open"></param>
        private void OpenCloseMoreLevels(bool open) {
            isMoreLevelPanelOpen = open;
            
            if (isMoreLevelPanelOpen) {
                moreLevelCreatedTransform.DOScale(new Vector3(1, 1, 1), popUpAnimationDuration).SetEase(Ease.OutBack, animationAmplitude)
                    .OnComplete(CreateButtonsForLevelCreated);
            }
            else {
                moreLevelCreatedTransform.DOScale(Vector3.zero, popUpAnimationDuration).OnComplete(() => {
                    for (int i = contentPanelBox.childCount - 1; i >= 0; i--) {
                        Destroy(contentPanelBox.GetChild(i).gameObject);
                    }
                });
            }
        }
        #endregion OpenClose Panels
        
        #region GoTo Panels
        /// <summary>
        /// Go to the currency position in the shop
        /// </summary>
        /// <param name="scrollRect"></param>
        private void GoToCurrencyPos() {
            Sequence shopSequence = DOTween.Sequence();
            shopSequence.AppendInterval(waitTimeShop);
            shopSequence.Append(shopScrollRect.DONormalizedPos(new Vector2(0, 0), timeToGoToShop));
        }
        
        /// <summary>
        /// Go to the create menu
        /// </summary>
        /// <param name="goCreate"></param>
        private void GoToCreate(bool goCreate) => isInCreateMenu = goCreate;
        
        /// <summary>
        /// Switch page when button is pressed
        /// </summary>
        /// <param name="id"></param>
        public void MoveToPage(int id) {
            isInCreateMenu = false;
            pageID = id;
        }

        private void OpenTool() {
            m_Tool.ShowTool();
        }
        #endregion GoTo Panels
        
        #region Timer Information
        /// <summary>
        /// Update the time of the offers
        /// </summary>
        private void UpdateTimeOffers() {
            DateTime currentTime = DateTime.Now;
            DateTime dayTargetTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 59, 59);
            DateTime specialOfferTargetTime = new DateTime(2023, 6, 11, 23, 59, 59);
            DateTime otherEventTargetTime = new DateTime(2023, 6, 8, 23, 59, 59);
            TimeSpan timeRemainingDay= dayTargetTime - currentTime;
            TimeSpan timeRemainingOffer = specialOfferTargetTime - currentTime;
            TimeSpan timeRemainingOtherEvent = otherEventTargetTime - currentTime;
            
            specialOfferTxt.text = $"end in : {timeRemainingOffer.Days}d {timeRemainingOffer.Hours}h";
            bundleOfferTxt.text = $"end in : {timeRemainingOffer.Days}d {timeRemainingOffer.Hours}h";
            specialEventTxt.text = $"end in : {timeRemainingOffer.Days}d {timeRemainingOffer.Hours}h";
            otherEventTxt.text = $"end in : {timeRemainingOtherEvent.Days}d {timeRemainingOtherEvent.Hours}h";
            
            daysOfferTxt.text = $"end in : {timeRemainingDay.Hours}h {timeRemainingDay.Minutes}min";
            mapOfDayTxt.text = $"end in : {timeRemainingDay.Hours}h {timeRemainingDay.Minutes}min";
        }
        #endregion Timer Information
        
        #region Btn Animation
        /// <summary>
        /// Launch the animations of all the buttons
        /// </summary>
        private void LaunchAllAnimation() {
            AnimateAllBtns(homeBtnPunch);
            AnimateAllBtns(shopBtnsPunch, 0.025f);
            AnimateAllBtns(commuBtnPunch, 0.025f);
        }
        
        /// <summary>
        /// Animate all the rect inside the list
        /// </summary>
        /// <param name="btns"></param>
        private void AnimateAllBtns(List<RectTransform> btns, float size = 0.1f) {
            Sequence btnSequence = DOTween.Sequence();
            btnSequence.SetLoops(-1).SetAutoKill(false);
            btnSequence.AppendInterval(Random.Range(10, 15));
            foreach (RectTransform rect in btns) {
                btnSequence.Append(rect.DOPunchScale(new Vector3(size, size, size), animationDuration * 2, 2));
                btnSequence.AppendInterval(0.1f);
            }
        }
        #endregion btn Animation
        
        #region ChangeSkin
        /// <summary>
        /// Change the current skin of the player
        /// </summary>
        /// <param name="addValue"></param>
        public void ChangeCurrentSkin(int addValue) {
            int currentSkin = PlayerPrefs.GetInt("PlayerSkin", 0);
            String skins = PlayerPrefs.GetString("Skins", "0346");
            currentSkin += addValue;
            var index = 0;
            while (!skins.Contains($"{currentSkin}"))
            {
                currentSkin+= addValue;
                if (currentSkin > skinMat.Count - 1) currentSkin = 0;
                else if (currentSkin < 0) currentSkin = skinMat.Count - 1;

                index++;
                if (index > 10) throw new ArgumentOutOfRangeException("While Break");
            }

            PlayerPrefs.SetInt("PlayerSkin", currentSkin);
            UpdatePlayerSkin();
        }

        private void AddSkin(int index)
        {
            var currentSkins = PlayerPrefs.GetString("Skins", "0346");
            if (!currentSkins.Contains($"{index}"))
            {
                currentSkins += $"{index}";
                PlayerPrefs.SetString("Skins", currentSkins);
                Debug.Log("Now Skins : " + currentSkins);
            }
            
        }

        private void UpdatePlayerSkin() {
            skinRend.sharedMaterial = skinMat[PlayerPrefs.GetInt("PlayerSkin", 0)];
            playerTrans.DORewind();
            playerTrans.DOPunchScale(new Vector3(75, 75, 75), popUpAnimationDuration, 1);
        }

        /// <summary>
        /// Init the size of the skins
        /// </summary>
        private void InitSkinSize() {
            float sizeRatio = Mathf.Clamp(mainMenuTransform.rect.height / 4060f, 0f, 1f);
            float Playerscale = 700 * playerSizeCurve.Evaluate(sizeRatio);
            playerTrans.localScale = new Vector3(Playerscale, Playerscale, Playerscale);
        }
        
        #endregion ChangeSkin

        public void ChangeCurrentBanner() {
            int currentBannerID = PlayerPrefs.GetInt("Banner", 0) + 1;
            
            if (currentBannerID > banners.Count - 1) currentBannerID = 0;
            else if (currentBannerID < 0) currentBannerID = banners.Count - 1;
            
            PlayerPrefs.SetInt("Banner", currentBannerID);
            SetBanner();
        }
        
        private void SetBanner() => bannerImg.sprite = banners[PlayerPrefs.GetInt("Banner", 0)];
    }
}

public enum MovementDirection {
    X, Y, None
}

public enum PageDirection {
    Shop, Home, Community, Search, LevelSelection_Spring, LevelSelection_Autumn, LevelSelection_Winter, Create
}

public enum RewardType {
    Gold, Skin
}