using Archi.Service.Interface;
using Attributes;
using DG.Tweening;
using UnityEngine;

namespace UI.Canvas
{
    public class InGameCanvasUtilities : CanvasUtilities
    {
        [ServiceDependency] private ILevelService m_level;
        [ServiceDependency] private IInterfaceService m_thisInterface;
        [ServiceDependency] private IToolService m_tool;
        [SerializeField] private GameObject playingCanvas;
        [SerializeField] private GameObject pauseCanvas;

        [SerializeField] private GameObject pressToStartText;
        [SerializeField] private GameOverCanvasUtilities canvasGO = null;

        private void Start() {
            Inputs.Inputs.Instance.OnTouch += HideStartText;
            pressToStartText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f, 1).SetLoops(-1).SetAutoKill(false);
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

        public void Pause() {
            canvasGO.SetMLevel(m_level, m_thisInterface, m_tool);
            Time.timeScale = 0;
            pauseCanvas.SetActive(true);
            playingCanvas.SetActive(false);
        }

        public void UnPause() {
            Time.timeScale = 1;
            pauseCanvas.SetActive(false);
            playingCanvas.SetActive(true);
        }
    }
}