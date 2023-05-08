using System;
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

        [Header("Viewport Information")]
        [SerializeField] private RectTransform mainMenuTransform = null;
        [SerializeField] private RectTransform viewportTransform = null;
        [SerializeField] private Vector2 minMaxViewportXValue = new();
        [SerializeField, Range(0, 1)] private float changeScreenSpeed = 0.2f;
        
        [SerializeField] private TMP_InputField inputField;
        
        public override void Init()
        {
            var saver = GetComponentInChildren<SaveTester>();
            if (saver){ saver.m_Database = m_Data; }
        }
        
        public void ShowLevels()
        {
            
        }


        private Vector2 startPosition = new();
        private Vector3 viewportStartPosition = new();
        private Vector3 targetPosition = new();
        private bool hasMove = false;
        
        private void Update() {
            MoveCurrentPageWithInput();
        }

        /// <summary>
        /// Move the current page when the player swipe the screen
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void MoveCurrentPageWithInput() {
            if (Input.touchCount == 0) {
                viewportTransform.localPosition = Vector3.Lerp(viewportTransform.localPosition, targetPosition, changeScreenSpeed);
                return;
            }
            
            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    startPosition = Input.GetTouch(0).position;
                    viewportStartPosition = viewportTransform.localPosition;
                    break;
                
                case TouchPhase.Moved:
                    viewportTransform.position += new Vector3(Input.GetTouch(0).deltaPosition.x, 0, 0);
                    hasMove = true;
                    break;
                
                case TouchPhase.Ended:
                    if (!hasMove) return;
                    hasMove = false;
                    
                    if (Mathf.Abs(viewportTransform.localPosition.x - viewportStartPosition.x) < mainMenuTransform.sizeDelta.x / 2f) {
                        targetPosition = viewportStartPosition;
                        return;
                    }

                    Vector3 addPosition = new Vector3(mainMenuTransform.sizeDelta.x, 0, 0);
                    if (Input.GetTouch(0).position.x - startPosition.x < 0 && viewportTransform.localPosition.x > minMaxViewportXValue.x) {
                        targetPosition = viewportStartPosition - addPosition;
                    }
                    else if(Input.GetTouch(0).position.x - startPosition.x > 0 && viewportTransform.localPosition.x < minMaxViewportXValue.y) {
                        targetPosition = viewportStartPosition + addPosition;
                    }
                    else {
                        targetPosition = viewportStartPosition;
                    }

                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        /// <summary>
        /// Switch page when button is pressed
        /// </summary>
        /// <param name="id"></param>
        public void MoveToPage(int id) {
            targetPosition = new Vector3(mainMenuTransform.sizeDelta.x * id, 0, 0);
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
