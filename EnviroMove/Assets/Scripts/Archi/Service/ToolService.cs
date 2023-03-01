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

        public void ShowTool()
        {
            SceneManager.LoadScene("Tool");
            SceneManager.sceneLoaded += OnLoadSceneCompleted;
        }

        private void OnLoadSceneCompleted(Scene scene, LoadSceneMode mode)
        {
            m_Interface.DrawCanvas(Enums.MajorCanvas.tool);
            sceneEditor = new SceneEditor();
            m_Tick.OnUpdate += sceneEditor.Update;
            sceneEditor.Start();
            SceneManager.sceneLoaded -= OnLoadSceneCompleted;
        }

        private void GenerateEditorHandler(GameObject editor)
        {
            var newGo = Object.Instantiate(editor);
            var editorScript = newGo.GetComponent<SceneEditor>();
            editorScript.m_Data = m_Data;   
        }

        public LevelData GetDataCreation()
        {
            throw new System.NotImplementedException();
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
            sceneEditor.SaveData();
        }

        #endregion
    }
}