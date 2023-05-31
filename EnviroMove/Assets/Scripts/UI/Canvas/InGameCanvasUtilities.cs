using UnityEngine;

namespace UI.Canvas
{
    public class InGameCanvasUtilities : CanvasUtilities
    {
        [SerializeField] private GameObject playingCanvas;
        [SerializeField] private GameObject pauseCanvas;
        
        
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