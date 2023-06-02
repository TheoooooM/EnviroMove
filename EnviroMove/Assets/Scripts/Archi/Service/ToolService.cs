using System;
using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static AdresseHelper;
using static UnityEngine.SceneManagement.SceneManager;
using Object = UnityEngine.Object;

namespace Archi.Service
{
    public class ToolService : Service, IToolService
    {
        [DependeOnService] private IInterfaceService m_Interface;
        [DependeOnService] private IDataBaseService m_Data;
        [DependeOnService] private ITickService m_Tick; //TODO
        [DependeOnService] private ILevelService m_Level;

        private SceneEditor sceneEditor;

        private string currentID;
        
        protected override void Initialize()
        { }

        public void ShowLevels()
        { 
            LoadScene("LevelSelector");
            sceneLoaded += OnLevelSelectorLoad;
        }

        public void DesactivateTool()
        {
            if (sceneEditor == null) return;
            m_Tick.OnUpdate -= sceneEditor.Update;
            sceneEditor = null;
        }

        void OnLevelSelectorLoad(Scene scene, LoadSceneMode mode)
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.toolLevels);
            sceneLoaded -= OnLevelSelectorLoad;
        }
        

        public void ShowTool()
        {
            m_Interface.GenerateLoadingScreen("Lood tool", 1, () => {
                LoadScene("Tool");
                sceneLoaded += OnLoadSceneCompleted;
            });
        }

        private void OnLoadSceneCompleted(Scene scene, LoadSceneMode mode)
        {
            m_Interface.HideLoadingScreen();
            m_Interface.DrawCanvas(Enums.MajorCanvas.tool);
            sceneEditor ??= new SceneEditor();
            SetObjectDependencies(sceneEditor);
            m_Tick.OnUpdate += sceneEditor.Update;
            if (dataLoaded != null)
            {
                sceneEditor.LoadData(dataLoaded);
                currentID = dataLoaded.id;
                dataLoaded = null;
            }
            else
            {
                currentID = m_Data.GetUniqueIdentifier();
                sceneEditor.Start();
            }
            sceneLoaded -= OnLoadSceneCompleted;
        }

        private void GenerateEditorHandler(GameObject editor)
        {
            var newGo = Object.Instantiate(editor);
            var editorScript = newGo.GetComponent<SceneEditor>();
            SetObjectDependencies(editorScript);
            Debug.Log("set m_Data :"+editorScript.m_Data);
        }

        public LevelData GetDataCreation()
        {
            Debug.Log("GetDataCreation");
            var data = sceneEditor.GetData();
            if (currentID == "") throw new ArgumentNullException("ID isn't Set");
            if (data.id == null) data.id = currentID;
            return data;
        }

        private LevelData dataLoaded;
        public void OpenLevel(LevelData data) {
            m_Interface.GenerateLoadingScreen("Lood tool", 1, () => {
                dataLoaded = data;
                LoadScene("Tool");
                sceneLoaded += OnLoadSceneCompleted;
            });
        }
        
        public void TestLevel()
        {
            sceneLoaded += AsyncTestLevel;
            dataLoaded = sceneEditor.TestLevel();
            /*dataLoaded = sceneEditor.TestLevel();
            Debug.Log(dataLoaded);
            m_Level.LoadLevel(dataLoaded); */
        }

        private void AsyncTestLevel(Scene scene, LoadSceneMode mode)
        {
            Debug.Log(dataLoaded);
            m_Level.LoadLevel(dataLoaded);
            
            sceneLoaded -= AsyncTestLevel;
        }


        #region SceneEditor
        
        public void SliderCamera(float value)
        {
            sceneEditor.SliderCamera(value);
        }
        public void CleanScene()
        {
            // parent = GameObject.Find(inputField.text);
            // foreach (Transform child in parent.transform)
            // {
            //     Destroy(child.gameObject);
            // }
        }
        
        public void ChangePrefab(int index)
        {
            sceneEditor.ChangePrefab(index);
        }

        public void SwitchMode(int index)
        {
            sceneEditor.SwitchMode(index);
        }
    
        public void ChangeMoveCamera()
        {
            // isMoveCamera = !isMoveCamera;
        }

        public void PlaceBlock(int indexBlock)
        {
        }

        public void SaveData(string name)
        {
            Debug.Log("Tool service clicked");
            sceneEditor.SaveData(name);
        }

        public void ToggleLevelElements()
        {
            sceneEditor.ToggleLevelElements();
        }

        public void ChangeCameraAngle()
        {
            sceneEditor.ChangeCameraAngle();
        }
        
        public void SwapSeason()
        {
            sceneEditor.SwapSeason();
        }
        
        public void PlaceGrass()
        {
            sceneEditor.PlaceGrass();
        }
        
        public void PlaceCaillou()
        {
            sceneEditor.PlaceCaillou();
        }
        #endregion
    }
}