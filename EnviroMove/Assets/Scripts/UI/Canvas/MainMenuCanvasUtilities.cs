using System.Collections.Generic;
using Archi.Service.Interface;
using Attributes;
using DG.Tweening;
using Levels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Canvas
{
    public class MainMenuCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] private ILevelService m_Level;
        [ServiceDependency] private IToolService m_Tool;
        [ServiceDependency] private IDataBaseService m_Data;
        [SerializeField] private float animationDuration = 0.5f;

        [Header("Viewport Information")]
        [SerializeField] private UnityEngine.Canvas mainCanvas = null;
        [SerializeField] private RectTransform mainMenuTransform = null;
        [SerializeField] private RectTransform viewportTransform = null;
        [SerializeField] private Vector2 minMaxViewportXValue = new();
        [SerializeField, Range(0, 1)] private float changeScreenSpeed = 0.2f;
        [SerializeField] private TMP_InputField inputField;
        private bool isInCreateMenu = false;
        
        [Header("Bottom Bar Information")]
        [SerializeField] private List<RectTransform> bottomBarButtonsRect = new();
        [SerializeField] private Vector2 buttonSizes = new();
        [SerializeField, Range(0,1 )] private float changeSizeSpeed = 0.1f;
        
        [Header("Level Selection Information")]
        [SerializeField] private RectTransform levelSelectionTransform = null;
        [SerializeField] private RectTransform seasonViewportTransform = null;
        [SerializeField] private RectTransform maskTransform = null;
        [SerializeField] private Vector2 minMaxlevelSelectionXValue = new();
        [SerializeField] private float movePanelXValue = 1820;
        [SerializeField] private float animationAmplitude = 1.25f;
        private bool isLevelSelectionOpen = false;
        private int levelSelectionID = 0;

        [Header("Level Create Information")]
        [SerializeField] private Transform layout;
        [SerializeField] private GameObject levelBox;
        [SerializeField] private GameObject contentBox;
        
        public override void Init()
        {
            var saver = GetComponentInChildren<SaveTester>();
            if (saver){ saver.m_Database = m_Data; }
            
            LevelInfo[] infos = m_Data.GetAllLevelInfos();
            Debug.Log($"Get {infos.Length} infos");
            foreach (var info in infos)
            {
                var go = Object.Instantiate(levelBox, layout);
                go.transform.SetParent(contentBox.transform);
                
                var box = go.GetComponent<LevelBox>();
                box.SetupBox(info, m_Tool, m_Data);
            }
        }
        
        private MovementDirection moveDir = MovementDirection.None;
        private Vector2 startPosition = new();
        private bool hasMove = false;
        private int pageID = 0;
        
        private void Update() {
            MoveCurrentPageWithInput();
            ChangeBottomBarButtonSize();
        }

        private RectTransform currentTransform = null;
        private Vector3 viewportStartPosition = new();
        private int currentID = 0;
        private Vector2 delatPos = new();
        private Vector2 position = new();
        private Vector2 minMaxViewportValue = new();
        
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
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(maskTransform, position, mainCanvas.worldCamera, out Vector2 rectPos);
                        if(!maskTransform.rect.Contains(rectPos)) OpenCloseSelectionlevel(false);
                    }
                    break;
                
                case TouchPhase.Moved:
                    if (Mathf.Abs(delatPos.y) > Mathf.Abs(delatPos.x) && moveDir == MovementDirection.None) moveDir = MovementDirection.Y;
                    else if(Mathf.Abs(delatPos.y) <= Mathf.Abs(delatPos.x) && moveDir == MovementDirection.None) moveDir = MovementDirection.X;
                    hasMove = true;
                    
                    if (CantMovePanel()) return;
                    if (moveDir == MovementDirection.Y) {
                        if (pageID != 0) return;
                        if (currentTransform.localPosition.y <= 0 && delatPos.y < 0 && !isInCreateMenu) return; 
                        if (currentTransform.localPosition.y >= 4053 && delatPos.y > 0 && isInCreateMenu) return;
                        
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
                        if (Mathf.Abs(currentTransform.localPosition.y - viewportStartPosition.y) < mainMenuTransform.sizeDelta.y / 10f) return;
                        isInCreateMenu = !isInCreateMenu;
                        return;
                    }

                    if (Mathf.Abs(currentTransform.localPosition.x - viewportStartPosition.x) < mainMenuTransform.sizeDelta.x / 6f) return;
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
            return isMainPage ? new Vector3(mainMenuTransform.sizeDelta.x * pageID, isInCreateMenu ? 4053f : 0, 0) : new Vector3(movePanelXValue * levelSelectionID, 0, 0);
        }
        #endregion Panel Movement Helper

        /// <summary>
        /// Switch page when button is pressed
        /// </summary>
        /// <param name="id"></param>
        public void MoveToPage(int id) {
            isInCreateMenu = false;
            pageID = id;
        }

        public void ShowLevelSelector() => m_Tool.ShowLevels();

        public void SetUsername()
        {
            if(inputField.text != "")m_Data.SetUsername(inputField.text);
        } 
        
        public void OpenCloseInformationPanel(GameObject informationPanel) {
            informationPanel.SetActive(!informationPanel.activeSelf);
        }

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
        
        public void OpenCloseSelectionlevel(bool open) {
            isLevelSelectionOpen = open;
            
            if (isLevelSelectionOpen) {
                levelSelectionTransform.DOScale(new Vector3(1, 1, 1), animationDuration).SetEase(Ease.OutBack, animationAmplitude);
            }
            else {
                levelSelectionTransform.DOScale(Vector3.zero, animationDuration / 2f);
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

        public void GoToCurrencyPos(ScrollRect scrollRect) {
            scrollRect.verticalNormalizedPosition = 0;
        }

        public void GoToCreate(bool goCreate) => isInCreateMenu = goCreate;
        
        
        #region LoadLevel
        public void LoadLevel(string levelName)
        {
            m_Level.LoadLevel(m_Data.GetLevelByName(levelName));
        }

        private LevelData dataToTest;
        public void LoadLevelData(string data)
        {
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

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1)
        {
            m_Level.LoadLevel(dataToTest);
            SceneManager.sceneLoaded -= AsyncTestLevel;
            
        }
        #endregion LoadLevel
        
        public void OpenTool() {
            m_Tool.ShowTool();
        }

    }
}

public enum MovementDirection {
    X, Y, None
}
