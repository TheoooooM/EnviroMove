using System;
using UnityEngine;

namespace UI.Canvas
{
    public class InGameCanvasUtilities : CanvasUtilities
    {
        [SerializeField] private GameObject playingCanvas;
        [SerializeField] private GameObject pauseCanvas;

        [SerializeField] private GameObject pressToStartText;

        private void Start()
        {
            Inputs.Inputs.Instance.OnTouch += HideStartText;
        }

        private void HideStartText(Vector2 side)
        {
            pressToStartText.SetActive(false);
            Inputs.Inputs.Instance.OnTouch -= HideStartText;
        }

        public override void Init()
        {
            playingCanvas.SetActive(true);
            pauseCanvas.SetActive(false);
        }

        public void Pause()
        {
            Time.timeScale = 0;
        }

        public void UnPause()
        {
            Time.timeScale = 1;
        }
    }
}