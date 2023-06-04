using System.Collections;
using System.Text;
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
        [ServiceDependency] private IAudioService m_thisAudio;

        [SerializeField] private Button nextLevelButton = null;
        [SerializeField] private Button editLevelButton = null;
        [SerializeField] private MenuType menuType = MenuType.GameOver;
        private LevelSO nextLevel = null;
        private int currentLevelID = 0;
        
        public override void Init() {
            nextLevel = m_thisInterface.GetNextLevelSO();
            currentLevelID = m_thisInterface.GetCurrentLevelID();
            StringBuilder levelFinished = new StringBuilder(PlayerPrefs.GetString("LevelFinish", "000000000000000"));
            if (menuType == MenuType.Win && m_thisInterface.GetTargetPage() != PageDirection.Create) {
                if (levelFinished[currentLevelID] == '0') PlayerPrefs.SetInt("GoldReward", PlayerPrefs.GetInt("GoldReward", 0) + 300);
                else PlayerPrefs.SetInt("GoldReward", PlayerPrefs.GetInt("GoldReward", 0) + 100);
                levelFinished[currentLevelID] = '1';
                PlayerPrefs.SetString("LevelFinish", levelFinished.ToString());
            }
            
            if(nextLevelButton != null) nextLevelButton.gameObject.SetActive(nextLevel != null && menuType == MenuType.Win);
            if(editLevelButton != null) editLevelButton.gameObject.SetActive(m_thisInterface.GetTargetPage() == PageDirection.Create);
            // editLevelButton.gameObject.SetActive(true);
        }

        public void EditLevel() {
            Time.timeScale = 1;
            m_thisAudio.StopSounds();
            StartCoroutine(WaitForAction(() => {
                m_tool.OpenLevel(m_level.GetCurrentLevelData());
            }));
        }

        private LevelData dataTemp;
        public void Replay(bool fromPause) {
            StartCoroutine(WaitForAction(() => {
                m_thisAudio.StopSounds();
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
            m_thisInterface.SetNextLevelSO(nextLevel.Nextlevel, nextLevel.Id);
            dataTemp = (LevelData)nextLevel.LevelData;
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                SceneManager.sceneLoaded += AsyncTestLevel;
                ChangeScene("InGame");
            });
        }

        public void SetMLevel(ILevelService m_levelUp , IInterfaceService interfaceService, IToolService tool, IAudioService audio) {
            m_level = m_levelUp;
            m_thisInterface = interfaceService;
            m_tool = tool;
            m_thisAudio = audio;
            if(editLevelButton != null) editLevelButton.gameObject.SetActive(m_thisInterface.GetTargetPage() == PageDirection.Create);
        }

        /// <summary>
        /// Change Scene
        /// </summary>
        /// <param name="sceneName"></param>
        public void GoToMainMenu(string sceneName) {
            Time.timeScale = 1;
            m_thisAudio.StopSounds();
            string sceneDirection = m_thisInterface.GetTargetPage() switch {
                PageDirection.LevelSelection_Spring => "RoadMap",
                PageDirection.LevelSelection_Autumn => "RoadMap",
                PageDirection.LevelSelection_Winter => "RoadMap",
                _ => "MainMenu"
            };
            
            m_thisInterface.GenerateLoadingScreen("Load Level", 1, () => {
                if(sceneDirection == "RoadMap") SceneManager.sceneLoaded += DrawRoadMap;
                ChangeSceneWithClouds(sceneDirection, m_thisInterface);
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