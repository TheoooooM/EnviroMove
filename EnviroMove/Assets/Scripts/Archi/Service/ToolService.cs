using Archi.Service.Interface;
using Attributes;
using Levels;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using static AdresseHelper;

namespace Archi.Service
{
    public class ToolService : Service, IToolService
    {
        [DependeOnService] private IInterfaceService m_Interface;
        [DependeOnService] private IDataBaseService m_Data;
        [DependeOnService] private ITickService m_Tick; //TODO
        private SceneEditor sceneEditor;
        protected override void Initialize()
        { }

        public void ShowLevels()
        {
            SceneManager.LoadScene("LevelSelector");
            SceneManager.sceneLoaded += OnLevelSelectorLoad;
        }

        void OnLevelSelectorLoad(Scene scene, LoadSceneMode mode)
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.toolLevels);
            SceneManager.sceneLoaded -= OnLevelSelectorLoad;
        }
        

        public void ShowTool()
        {
            SceneManager.LoadScene("Tool");
            SceneManager.sceneLoaded += OnLoadSceneCompleted;
        }

        private void OnLoadSceneCompleted(Scene scene, LoadSceneMode mode)
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.tool);
            sceneEditor = new SceneEditor();
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
            SceneManager.sceneLoaded -= OnLoadSceneCompleted;
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
            throw new System.NotImplementedException();
        }

        private LevelData dataLoaded;
        public void OpenLevel(LevelData data)
        {
            dataLoaded = data;
            SceneManager.LoadScene("Tool");
            SceneManager.sceneLoaded += OnLoadSceneCompleted;
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
            // Mode = (EditorMode) index;
        }
    
        public void ChangeMoveCamera()
        {
            // isMoveCamera = !isMoveCamera;
        }

        public void PlaceBlock(int indexBlock)
        {
        }

        public void SaveData()
        {
            Debug.Log("Tool service clicked");
            sceneEditor.SaveData();
        }

        #endregion
    }
}