using System.Collections;
using Archi.Service.Interface;
using Attributes;
using DG.Tweening;
using Levels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Canvas
{
    public class GameOverCanvasUtilities : CanvasUtilities {
        [ServiceDependency] private IToolService m_tool;
        [ServiceDependency] private ILevelService m_level;
        [ServiceDependency] private IInterfaceService m_thisInterface;

        [SerializeField] private Button nextLevelButton = null;
        [SerializeField] private Button editLevelButton = null;
        [SerializeField] private MenuType menuType = MenuType.GameOver;
        private LevelSO nextLevel = null;
        
        public override void Init() {
            nextLevel = m_thisInterface.GetNextLevelSO();
            nextLevelButton.gameObject.SetActive(nextLevel != null);
            editLevelButton.gameObject.SetActive(m_thisInterface.GetTargetPage() == PageDirection.Create);
            // editLevelButton.gameObject.SetActive(true);
        }

        public void EditLevel() {
            StartCoroutine(WaitForAction(() => {
                m_tool.OpenLevel(m_level.GetCurrentLevelData());
            }));
        }

        private LevelData dataTemp;
        public void Replay(bool fromPause) {
            StartCoroutine(WaitForAction(() => {
                Time.timeScale = 1;
                dataTemp = m_level.GetCurrentLevelData();
                m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                    SceneManager.sceneLoaded += AsyncTestLevel;
                    ChangeScene("InGame");
                });
            }));
        }

        private void AsyncTestLevel(Scene arg0, LoadSceneMode arg1) {
            m_level.LoadLevel(dataTemp);
            dataTemp = null;
            SceneManager.sceneLoaded -= AsyncTestLevel;
        }

        /// <summary>
        /// Load the next level
        /// </summary>
        public void LoadNextLevel() {
            Time.timeScale = 1;
            m_thisInterface.SetNextLevelSO(nextLevel.Nextlevel);
            dataTemp = (LevelData)nextLevel.LevelData;
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeScene("InGame");
            });
        }

        public void SetMLevel(ILevelService m_levelUp , IInterfaceService interfaceService) {
            m_level = m_levelUp;
            m_thisInterface = interfaceService;
        }

        /// <summary>
        /// Change Scene
        /// </summary>
        /// <param name="sceneName"></param>
        public void GoToMainMenu(string sceneName) {
            Time.timeScale = 1;
            
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                //SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeSceneWithClouds(sceneName, m_thisInterface);
            });
        }
        
        /// <summary>
        /// Animate the button
        /// </summary>
        /// <param name="button"></param>
        public void ButtonAnimation(RectTransform button) {
            button.DORewind();
            button.DOPunchScale(new Vector3(-.075f, -.075f, -.075f), .8f / 2f, 1);
        }
        
        /// <summary>
        /// Wait before doing anything
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private IEnumerator WaitForAction(UnityAction action) {
            yield return new WaitForSeconds(0.075f);
            action.Invoke();
        }
    }
}

public enum MenuType {
    GameOver,
    Win,
    Pause
}