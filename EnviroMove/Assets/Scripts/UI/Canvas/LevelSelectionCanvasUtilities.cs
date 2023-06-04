using System;
using System.Collections.Generic;
using Archi.Service.Interface;
using Attributes;
using DG.Tweening;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Canvas {
    public class LevelSelectionCanvasUtilities : CanvasUtilities {
        [ServiceDependency] private ILevelService m_Level;
        [ServiceDependency] private IToolService m_Tool;
        [ServiceDependency] private IDataBaseService m_Data;
        [ServiceDependency] private IInterfaceService m_thisInterface;

        [SerializeField] private UnityEngine.Canvas springCanvas = null;
        [SerializeField] private UnityEngine.Canvas autumnCanvas = null;
        [SerializeField] private UnityEngine.Canvas winterCanvas = null;
        [SerializeField] private List<RectTransform> seasonButtons = new();
        
        [SerializeField] private float moveDistance = 2.9f;
        [SerializeField] private float moveTouchSpeed = 0.01f;
        [SerializeField] private float moveCameraSpeed = 2.9f;
        private Transform cameraTransform = null;
        private int seasonID = -1;

        [SerializeField] private List<RectTransform> springButtonTransform = new();
        [SerializeField] private List<RectTransform> autumnButtonTransform = new();
        [SerializeField] private List<RectTransform> winterButtonTransform = new();

        private Vector3 startPos = new();

        /// <summary>
        /// Set the position of the camera at the start
        /// </summary>
        public override void Init() {
            m_thisInterface.HideLoadingScreen();
            AnimateButtons();
        }

        private void AnimateButtons() {
            Sequence spring = DOTween.Sequence();
            spring.SetLoops(-1).SetAutoKill(false);
            foreach (RectTransform rect in springButtonTransform) {
                spring.Append(rect.DOPunchScale(new Vector3(.05f, .05f, .05f), 1, 1));
            }
            
            Sequence autumn = DOTween.Sequence();
            autumn.SetLoops(-1).SetAutoKill(false);
            foreach (RectTransform rect in autumnButtonTransform) {
                autumn.Append(rect.DOPunchScale(new Vector3(.05f, .05f, .05f), 1, 1));
            }
            
            Sequence winter = DOTween.Sequence();
            winter.SetLoops(-1).SetAutoKill(false);
            foreach (RectTransform rect in winterButtonTransform) {
                winter.Append(rect.DOPunchScale(new Vector3(.05f, .05f, .05f), 1, 1));
            }
        }
        
        public void InitValue(PageDirection dir) {
            cameraTransform = Camera.main.transform;
            seasonID = dir switch {
                PageDirection.LevelSelection_Spring => -1,
                PageDirection.LevelSelection_Autumn => 0,
                PageDirection.LevelSelection_Winter => 1,
                _ => -1
            };
            springCanvas.worldCamera = Camera.main;
            autumnCanvas.worldCamera = Camera.main;
            winterCanvas.worldCamera = Camera.main;
            cameraTransform.position = GetTargetDirection();
        }
        
        private void Update() {
            MoveWithTouch();
            ChangeButtonSize();
        }

        private void ChangeButtonSize() {
            for (int i = 0; i < seasonButtons.Count; i++) {
                seasonButtons[i].sizeDelta = Vector2.Lerp(seasonButtons[i].sizeDelta, i == seasonID + 1 ? new Vector2(400, 400) : new Vector2(300, 300), 0.05f);
            }
        }
        
        
        private void MoveWithTouch() {
            if (Input.touchCount == 0) {
                if(cameraTransform != null) cameraTransform.position = Vector3.Lerp(cameraTransform.position, GetTargetDirection(), moveCameraSpeed);
                return;
            }
            
            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    startPos = Input.GetTouch(0).position;
                    break;
                
                case TouchPhase.Moved:
                    float movementX = -Input.GetTouch(0).deltaPosition.x * moveTouchSpeed;
                    if (cameraTransform.position.x + movementX >= -2.9f && cameraTransform.position.x + movementX <= 2.9f) {
                        cameraTransform.position += new Vector3(movementX, 0, 0);
                    }
                    break;
                
                case TouchPhase.Ended:
                    float Direction = Input.GetTouch(0).position.x - startPos.x;
                    switch (Direction) {
                        case < 0 when seasonID < 1:
                            seasonID++;
                            break;
                        case > 0 when seasonID > -1:
                            seasonID--;
                            break;
                    }
                    break;
            }
        }

        #region Season
        /// <summary>
        /// Switch the current season
        /// </summary>
        /// <param name="id"></param>
        public void SwitchSeason(int id) => seasonID = id;
        
        /// <summary>
        /// Get the vector3 of the direction for the camera
        /// </summary>
        /// <returns></returns>
        private Vector3 GetTargetDirection() => new Vector3(moveDistance * seasonID, 4, 0);
        #endregion

        #region LoadLevel
        private LevelData dataToTest;
        /// <summary>
        /// Load a level based on a SO
        /// </summary>
        /// <param name="data"></param>
        public void LoadLevelData(LevelSO data) {
            m_thisInterface.SetNextLevelSO(data.Nextlevel, data.Id);
            dataToTest = (LevelData)data.LevelData;
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeScene("InGame");
            });
        }
        
        /// <summary>
        /// Load the level after that the scene is loaded
        /// </summary>
        /// <param name="arg0"></param>
        /// <param name="arg1"></param>
        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1) {
            SceneManager.sceneLoaded -= AsyncTestLevel;
            m_Level.LoadLevel(dataToTest);
        }
        #endregion
        
        /// <summary>
        /// Animate the button
        /// </summary>
        /// <param name="button"></param>
        public void ButtonAnimation(RectTransform button) {
            button.DORewind();
            button.DOPunchScale(new Vector3(-.075f, -.075f, -.075f), 0.8f / 2f, 1);
        }

        /// <summary>
        /// Change Scene
        /// </summary>
        /// <param name="sceneName"></param>
        public void GoToMainMenu(string sceneName) {
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                ChangeSceneWithClouds(sceneName, m_thisInterface);
            });
        }

        public void SetCurrentPageAndValue() {
            switch (seasonID) {
                case -1 :
                    m_thisInterface.SetTargetPage(PageDirection.LevelSelection_Spring, 0);
                    break;
                
                case 0 :
                    m_thisInterface.SetTargetPage(PageDirection.LevelSelection_Autumn, 0);
                    break;
                
                case 1 :
                    m_thisInterface.SetTargetPage(PageDirection.LevelSelection_Winter, 0);
                    break;
            }
        }
    }
}