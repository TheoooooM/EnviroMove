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


        private MovementDirection moveDir = MovementDirection.None;
        private Vector2 startPosition = new();
        private Vector3 viewportStartPosition = new();
        private bool hasMove = false;
        private int pageID = 0;
        
        private void Update() {
            MoveCurrentPageWithInput();
        }

        /// <summary>
        /// Move the current page when the player swipe the screen
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void MoveCurrentPageWithInput() {
            if (Input.touchCount == 0) {
                moveDir = MovementDirection.None;
                viewportTransform.localPosition = Vector3.Lerp(viewportTransform.localPosition, GetTargetPosition(), changeScreenSpeed);
                return;
            }

            Vector2 delatPos = Input.GetTouch(0).deltaPosition;
            Vector2 position = Input.GetTouch(0).position;
            if (moveDir == MovementDirection.Y) {
                if (Input.GetTouch(0).phase == TouchPhase.Ended) moveDir = MovementDirection.None;
                return;
            }
            
            switch (Input.GetTouch(0).phase) {
                case TouchPhase.Began:
                    startPosition = position;
                    viewportStartPosition = viewportTransform.localPosition;
                    break;
                
                case TouchPhase.Moved:
                    if (viewportTransform.localPosition.x >= minMaxViewportXValue.y && delatPos.x > 0 || viewportTransform.localPosition.x <= minMaxViewportXValue.x && delatPos.x < 0) return;
                    if (Mathf.Abs(delatPos.y) > Mathf.Abs(delatPos.x) && moveDir == MovementDirection.None) moveDir = MovementDirection.Y;
                    else if(Mathf.Abs(delatPos.y) <= Mathf.Abs(delatPos.x) && moveDir == MovementDirection.None) moveDir = MovementDirection.X;
                    
                    viewportTransform.localPosition += new Vector3(delatPos.x, 0, 0);
                    hasMove = true;
                    break;
                
                case TouchPhase.Ended:
                    if (!hasMove) return;
                    hasMove = false;
                    
                    if (Mathf.Abs(viewportTransform.localPosition.x - viewportStartPosition.x) < mainMenuTransform.sizeDelta.x / 3f) return;
                    pageID = (position.x - startPosition.x) switch {
                        < 0 when viewportTransform.localPosition.x > minMaxViewportXValue.x => Mathf.Clamp(pageID - 1, -2, 1),
                        > 0 when viewportTransform.localPosition.x < minMaxViewportXValue.y => Mathf.Clamp(pageID + 1, -2, 1),
                        _ => pageID
                    };
                    break;
                
                case TouchPhase.Stationary:
                    if (!hasMove) {
                        viewportTransform.localPosition = Vector3.Lerp(viewportTransform.localPosition, GetTargetPosition(), changeScreenSpeed);
                    }
                    break;
                case TouchPhase.Canceled: break;
            }
        }

        /// <summary>
        /// Get the target position of the viewport
        /// </summary>
        private Vector3 GetTargetPosition() => new (mainMenuTransform.sizeDelta.x * pageID, 0, 0);

        /// <summary>
        /// Switch page when button is pressed
        /// </summary>
        /// <param name="id"></param>
        public void MoveToPage(int id) => pageID = id;

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

public enum MovementDirection {
    X, Y, None
}
