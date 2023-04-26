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

namespace Archi.Service
{
    public class ToolService : Service, IToolService
    {
        [DependeOnService] private IInterfaceService m_Interface;
        [DependeOnService] private IDataBaseService m_Data;
        [DependeOnService] private ITickService m_Tick; //TODO
        [DependeOnService] private ILevelService m_Level;

        private SceneEditor sceneEditor;
        protected override void Initialize()
        { }

        public void ShowLevels()
        {
            if (sceneEditor != null)
            {
                m_Tick.OnUpdate -= sceneEditor.Update;
                sceneEditor = null;
            }
            LoadScene("LevelSelector");
            sceneLoaded += OnLevelSelectorLoad;
        }

        void OnLevelSelectorLoad(Scene scene, LoadSceneMode mode)
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.toolLevels);
            sceneLoaded -= OnLevelSelectorLoad;
        }
        

        public void ShowTool()
        {
            LoadScene("Tool");
            sceneLoaded += OnLoadSceneCompleted;
        }

        private void OnLoadSceneCompleted(Scene scene, LoadSceneMode mode)
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.tool);
            sceneEditor ??= new SceneEditor();
            SetObjectDependencies(sceneEditor);
            m_Tick.OnUpdate += sceneEditor.Update;
            if (dataLoaded != null)
            {
                sceneEditor.LoadData(dataLoaded);
                dataLoaded = null;
            }
            else
            {
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
            return sceneEditor.GetData();
        }

        private LevelData dataLoaded;
        public void OpenLevel(LevelData data)
        {
            dataLoaded = data;
            LoadScene("Tool");
            sceneLoaded += OnLoadSceneCompleted;
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
        #endregion
    }
}