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

        [Header("Base Viewport Information")]
        public UnityEngine.Canvas mainCanvas = null;
        [SerializeField] private RectTransform mainMenuTransform = null;
        [SerializeField] private CanvasGroup mainCanvasGroup = null;
        
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
        public bool isInReward = false;
        private int stageReward = 0;

        [Header("- LevelSelection Panel Information- ")]
        [SerializeField] private RectTransform levelSelectionTransform = null;
        [SerializeField] private RectTransform seasonViewportTransform = null;
        [SerializeField] private RectTransform maskLevelSelectionTransform = null;
        [SerializeField] private Vector2 minMaxlevelSelectionXValue = new();
        [SerializeField] private float movePanelXValue = 1820;
        [SerializeField] private float animationAmplitude = 1.25f;
        private GameObject worldObj = null;
        private bool isLevelSelectionOpen = false;
        private int levelSelectionID = 0;
        
        [Header("- LevelCreate Panel Information- ")]
        [SerializeField] private RectTransform createMenuTransform = null;
        [SerializeField] private RectTransform moreLevelCreatedTransform = null;
        [SerializeField] private RectTransform maskLevelCreatedTransform = null;
        [SerializeField] private GameObject levelBox = null;
        [SerializeField] private RectTransform contentBox = null;
        [SerializeField] private RectTransform contentPanelBox = null;
        [SerializeField] private GameObject showMoreBtn = null;
        private bool isMoreLevelPanelOpen = false;
        
        
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private List<RectTransform> shopBtnsPunch = new();
        [SerializeField] private List<RectTransform> homeBtnPunch = new();
        [SerializeField] private List<RectTransform> commuBtnPunch = new();
        [SerializeField] private List<RectTransform> createBtnPunch = new();
        
        [Header("Player Skin")]
        [SerializeField] private SkinnedMeshRenderer skinRend = null;
        [SerializeField] private Transform playerTrans = null;
        [SerializeField] private List<Material> skinMat = new();
        
        [Header("DoTween Information")]
        [SerializeField] private float waitTimeShop = 0.4f;
        [SerializeField] private float timeToGoToShop = 0.75f;
        
        /// <summary>
        /// Initialize all the data for the script
        /// </summary>
        public override void Init() {
            var saver = GetComponentInChildren<SaveTester>();
            if (saver){ saver.m_Database = m_Data; }
            ResetCreateMenu();
            LaunchAllAnimation();
            UpdatePlayerSkin();
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
                    isLevelSelectionOpen = false;
                    break;
                
                case PageDirection.Community:
                    pageID = -1;
                    commuScrollRect.verticalNormalizedPosition = value;
                    break;
                
                case PageDirection.Search:
                    pageID = -2;
                    break;
                
                case PageDirection.LevelSelection_Spring:
                    pageID = 0;
                    levelSelectionID = 0;
                    isInCreateMenu = false;
                    isLevelSelectionOpen = true;
                    levelSelectionTransform.localScale = new Vector3(1, 1, 1);
                    playerTrans.gameObject.SetActive(false);
                    break;
                
                case PageDirection.LevelSelection_Autumn:
                    pageID = 0;
                    levelSelectionID = -1;
                    isInCreateMenu = false;
                    isLevelSelectionOpen = true;
                    levelSelectionTransform.localScale = new Vector3(1, 1, 1);
                    playerTrans.gameObject.SetActive(false);
                    break;
                
                case PageDirection.LevelSelection_Winter:
                    pageID = 0;
                    levelSelectionID = -2;
                    isInCreateMenu = false;
                    isLevelSelectionOpen = true;
                    levelSelectionTransform.localScale = new Vector3(1, 1, 1);
                    playerTrans.gameObject.SetActive(false);
                    break;
                
                case PageDirection.Create:
                    pageID = 0;
                    isInCreateMenu = true;
                    isLevelSelectionOpen = false;
                    break;
                
                default: throw new ArgumentOutOfRangeException(nameof(pageDirection), pageDirection, null);
            }
            viewportTransform.localPosition = GetTargetPosition();
            seasonViewportTransform.localPosition = GetTargetPosition(false);
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
            if (pageDirection == PageDirection.Home) {
                if (isLevelSelectionOpen) {
                    pageDirection = levelSelectionID switch {
                        0 => PageDirection.LevelSelection_Spring,
                        -1 => PageDirection.LevelSelection_Autumn,
                        -2 => PageDirection.LevelSelection_Winter,
                        _ => PageDirection.LevelSelection_Spring
                    };
                }
            }
            
            float value = pageDirection switch {
                PageDirection.Shop => shopScrollRect.verticalNormalizedPosition,
                PageDirection.Community => commuScrollRect.verticalNormalizedPosition,
                _ => 0f
            };
            m_thisInterface.SetTargetPage(pageDirection, value);
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
            MoveCurrentPageWithInput();
            ChangeBottomBarButtonSize();
            UpdateTimeOffers();
            createMenuTransform.localPosition = new Vector3(0, -mainMenuTransform.sizeDelta.y, 0);
        }

        private RectTransform currentTransform = null;
        private Vector3 viewportStartPosition = new();
        private int currentID = 0;
        private Vector2 delatPos = new();
        private Vector2 position = new();
        private Vector2 minMaxViewportValue = new();
        private GameObject openedPanel = null;

        /// <summary>
        /// Move the current page when the player swipe the screen
        /// </summary>
        private void MoveCurrentPageWithInput() {
            if (Input.touchCount == 0) {
                moveDir = MovementDirection.None;
                viewportTransform.localPosition = Vector3.Lerp(viewportTransform.localPosition, GetTargetPosition(), changeScreenSpeed);
                seasonViewportTransform.localPosition = Vector3.Lerp(seasonViewportTransform.localPosition, GetTargetPosition(false), changeScreenSpeed);
                return;
            }

            //Set the variable of the current page
            InitCurrentMovement();

            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    InitInputs();
                    if (isLevelSelectionOpen) {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(maskLevelSelectionTransform, position, mainCanvas.worldCamera, out Vector2 rectPos);
                        if(!maskLevelSelectionTransform.rect.Contains(rectPos)) OpenCloseCampaign(false);
                    }
                    else if (isMoreLevelPanelOpen) {
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
                        if (currentTransform.localPosition.y <= 0 && delatPos.y < 0 && !isInCreateMenu) return; 
                        if (currentTransform.localPosition.y >= mainMenuTransform.sizeDelta.y && delatPos.y > 0 && isInCreateMenu) return;
                        
                        currentTransform.localPosition += new Vector3(0, delatPos.y, 0);
                        return;
                    }
                    if (isInCreateMenu) return;
                    currentTransform.localPosition += new Vector3(delatPos.x, 0, 0);
                    break;
                
                case TouchPhase.Ended:
                    if (!hasMove) return;
                    hasMove = false;
                    MovementDirection currentMove = moveDir;
                    moveDir = MovementDirection.None;
                    
                    if (currentMove == MovementDirection.Y) {
                        if (Mathf.Abs(currentTransform.localPosition.y - viewportStartPosition.y) < mainMenuTransform.sizeDelta.y / 30f) return;
                        isInCreateMenu = !isInCreateMenu;
                        return;
                    }

                    if (Mathf.Abs(currentTransform.localPosition.x - viewportStartPosition.x) < mainMenuTransform.sizeDelta.x / 20f) return;
                    currentID = (position.x - startPosition.x) switch {
                        < 0 when currentTransform.localPosition.x > minMaxViewportValue.x => Mathf.Clamp(currentID - 1, -2, 1),
                        > 0 when currentTransform.localPosition.x < minMaxViewportValue.y => Mathf.Clamp(currentID + 1, -2, 1),
                        _ => currentID
                    };
                    if (isLevelSelectionOpen) levelSelectionID = currentID;
                    else pageID = currentID;

                    break;
                
                case TouchPhase.Stationary:
                    if (!hasMove) {
                        if(isLevelSelectionOpen) currentTransform.localPosition = Vector3.Lerp(currentTransform.localPosition, GetTargetPosition(!isLevelSelectionOpen), changeScreenSpeed);
                    }
                    break;
                case TouchPhase.Canceled: break;
            }
        }

        #region Panel Movement Helper
        /// <summary>
        /// Init the variable for the current page movement
        /// </summary>
        private void InitCurrentMovement() {
            currentTransform = isLevelSelectionOpen ? seasonViewportTransform : viewportTransform;
            currentID = isLevelSelectionOpen ? levelSelectionID : pageID;
            minMaxViewportValue = isLevelSelectionOpen ? minMaxlevelSelectionXValue : minMaxViewportXValue;
            delatPos = Input.GetTouch(0).deltaPosition;
            position = Input.GetTouch(0).position;
        }
        /// <summary>
        /// Set variables when the player start touching the screen
        /// </summary>
        private void InitInputs() {
            startPosition = position;
            viewportStartPosition = currentTransform.localPosition;
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
            return (currentTransform.localPosition.x >= minMaxViewportValue.y && delatPos.x > 0) || (currentTransform.localPosition.x <= minMaxViewportValue.x && delatPos.x < 0);
        }
        /// <summary>
        /// Get the target position of the viewport
        /// </summary>
        private Vector3 GetTargetPosition(bool isMainPage = true) {
            return isMainPage ? new Vector3(mainMenuTransform.sizeDelta.x * pageID, isInCreateMenu ? mainMenuTransform.sizeDelta.y : 0, 0) : new Vector3(movePanelXValue * levelSelectionID, 0, 0);
        }
        

        /// <summary>
        /// Move to a certain season
        /// </summary>
        /// <param name="id"></param>
        public void MoveToSeason(int id) => levelSelectionID = id;
        #endregion Panel Movement Helper
        
        public void SetUsername()
        {
            if(inputField.text != "")m_Data.SetUsername(inputField.text);
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

        #region Create Level More Panel
        /// <summary>
        /// Open or close the panel which contains all the created levels
        /// </summary>
        /// <param name="open"></param>
        public void OpenCloseMoreLevels(bool open) {
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

        private void CreateButtonsForLevelCreated() {
            m_Data.RefreshLevelData();
            LevelInfo[] infos = m_Data.GetAllLevelInfos();
            contentPanelBox.sizeDelta = new Vector2(contentPanelBox.sizeDelta.x, (buttonSizes.y + 50) * infos.Length);
            contentPanelBox.localPosition = new Vector3(0, 0, 0);

            Sequence levelApparitionSequence = DOTween.Sequence();
            for (var index = 0; index < infos.Length; index++) {
                Transform btn = CreateButton(contentPanelBox, infos, index);
                btn.transform.localScale = Vector3.zero;
                levelApparitionSequence.Append(btn.DOScale(1, popUpAnimationDuration / 1.25f).SetEase(Ease.OutBack));
            }
        }
        #endregion Create Level More Panel

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
        public void LoadLevel(string levelName) {
            UnLoad3DWorld();
            m_Level.LoadLevel(m_Data.GetLevelByName(levelName));
        }

        /// <summary>
        /// Load the level data
        /// </summary>
        /// <param name="data"></param>
        public void LoadLevelData(string data) {
            UnLoad3DWorld();
            //m_Level.LoadLevel((LevelData)constantLevels.GetLevel(i));
            //dataToTest = (LevelData)constantLevels.GetLevel(i);
            dataToTest = (LevelData)data;
            SceneManager.sceneLoaded += AsyncTestLevel;
            ChangeScene("InGame");
            
            //throw new NotImplementedException();
            /*
            m_Tool.TestLevel();
            m_Tool.SetServiceState(false);
            SceneManager.sceneLoaded += (_,_) => m_Tool.TestLevel();*/
        }
        public void LoadLevelData(LevelSO data) {
            UnLoad3DWorld();
            m_thisInterface.SetNextLevelSO(data.Nextlevel);
            dataToTest = (LevelData)data.LevelData;
            SceneManager.sceneLoaded += AsyncTestLevel;
            ChangeScene("InGame");
        }

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1) {
            m_Level.LoadLevel(dataToTest);
            SceneManager.sceneLoaded -= AsyncTestLevel;
        }
        #endregion LoadLevel
        
        public void OpenTool() {
            UnLoad3DWorld();
            m_Tool.ShowTool();
        }

        #region Rewards
        /// <summary>
        /// Open the reward panel and show the reward on screen
        /// </summary>
        public void GetRewards(Sprite sprite, string rewardName) {
            Sequence rewardSequence = DOTween.Sequence();
            rewardGroup.gameObject.SetActive(true);

            rewardSequence.Append(rewardGroup.DOFade(1, popUpAnimationDuration).OnComplete(() => stageReward = 1));
            rewardSequence.AppendInterval(popUpAnimationDuration);
            rewardImg.sprite = sprite;
            rewardTxt.text = rewardName;
            rewardSequence.Append(rewardImg.rectTransform.DOScale(1, popUpAnimationDuration).SetEase(Ease.OutBack).OnComplete(() => stageReward = 2));
            rewardSequence.Append(rewardTxt.rectTransform.DOScale(1, popUpAnimationDuration).SetEase(Ease.OutBack).OnComplete(() => stageReward = 3));
            isInReward = true;
        }

        /// <summary>
        /// Close the reward panel
        /// </summary>
        private void CloseRewardPanel() {
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
        public void GoToCreatePanel(bool goCreate) => StartCoroutine(WaitForAction(() => GoToCreate(goCreate)));
        public void GoToPage(int pageID) => StartCoroutine(WaitForAction(() => MoveToPage(pageID)));
        public void GoBuyCurrency(ScrollRect scrollRect) => StartCoroutine(WaitForAction(() => GoToCurrencyPos(scrollRect)));
        #endregion Button Events

        #region OpenClose Panels
        /// <summary>
        /// Open or close the selection level panel
        /// </summary>
        /// <param name="open"></param>
        private void OpenCloseCampaign(bool open) {
            isLevelSelectionOpen = open;
            playerTrans.gameObject.SetActive(!isLevelSelectionOpen);
            
            if (isLevelSelectionOpen) {
                //mainCanvasGroup.DOFade(0, popUpAnimationDuration);
                //m_thisInterface.GenerateLoadingScreen("Selection Level", 1);
                //Load3DWorldAsync();
                levelSelectionTransform.DOScale(new Vector3(1, 1, 1), popUpAnimationDuration).SetEase(Ease.OutBack, animationAmplitude);
            }
            else {
                //mainCanvasGroup.DOFade(1, popUpAnimationDuration);
                levelSelectionTransform.DOScale(Vector3.zero, popUpAnimationDuration);
            }
        }

        /// <summary>
        /// Load the 3D world
        /// </summary>
        private void Load3DWorldAsync() {
            if (worldObj == null) {
                AdresseHelper.LoadAssetWithCallback<GameObject>("WorldTest", (obj) => {
                    worldObj = Instantiate(obj);
                    m_thisInterface.HideLoadingScreen();
                });
                return;
            }
            m_thisInterface.HideLoadingScreen();
        }

        /// <summary>
        /// Unload the 3D World
        /// </summary>
        private void UnLoad3DWorld() {
            if(worldObj == null) return;
            Addressables.Release(worldObj);
            worldObj = null;
        }
        #endregion OpenClose Panels
        
        #region GoTo Panels
        /// <summary>
        /// Go to the currency position in the shop
        /// </summary>
        /// <param name="scrollRect"></param>
        private void GoToCurrencyPos(ScrollRect scrollRect) {
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
            currentSkin += addValue;
            if (currentSkin > skinMat.Count - 1) currentSkin = 0;
            else if (currentSkin < 0) currentSkin = skinMat.Count - 1;
            PlayerPrefs.SetInt("PlayerSkin", currentSkin);
            UpdatePlayerSkin();
        }

        private void UpdatePlayerSkin() {
            skinRend.sharedMaterial = skinMat[PlayerPrefs.GetInt("PlayerSkin", 0)];
            playerTrans.DORewind();
            playerTrans.DOPunchScale(new Vector3(75, 75, 75), popUpAnimationDuration, 1);
        }

        #endregion ChangeSkin
    }
}

public enum MovementDirection {
    X, Y, None
}

public enum PageDirection {
    Shop, Home, Community, Search, LevelSelection_Spring, LevelSelection_Autumn, LevelSelection_Winter, Create
}