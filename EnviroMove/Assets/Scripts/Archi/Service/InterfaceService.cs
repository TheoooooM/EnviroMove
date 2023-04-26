using System;
using System.Collections.Generic;
using Archi.Service.Interface;
using Attributes;
using UI;
using UI.Canvas;
using UnityEngine;
using UnityEngine.SceneManagement;
using static AdresseHelper;
using Object = UnityEngine.Object;

namespace Archi.Service
{
    public class InterfaceService : Service, IInterfaceService
    {
        [DependeOnService] private IGameService m_Game;
        [DependeOnService] private ILevelService m_Level;
        [DependeOnService] private IToolService m_Tool;
        [DependeOnService] private IDataBaseService m_data;

        private LoadingScreen loadingScreen;

        private readonly Dictionary<Enums.MajorCanvas, string> canvasAddress = new()
        {
            {Enums.MajorCanvas.menu, "MainMenuCanvas"},
            {Enums.MajorCanvas.inGame, ""},
            {Enums.MajorCanvas.tool, "ToolCanvas"},
            {Enums.MajorCanvas.levels, "LevelsCanvas"},
            {Enums.MajorCanvas.toolLevels, "LevelSelectorCanvas"},
        };

        protected override void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneInit;
            AdresseHelper.LoadAssetWithCallback<GameObject>("LoadingCanvas",SetupLoading);
        }

        void SetupLoading(GameObject obj)
        {
            var go = Object.Instantiate(obj);
            loadingScreen = go.GetComponent<LoadingScreen>();
            if (!loadingScreen) throw new MissingMemberException($"{loadingScreen} havn't LoadingScreen Script");
            Object.DontDestroyOnLoad(go);
            go.SetActive(false);
        }

        private void OnSceneInit(Scene scene, LoadSceneMode sceneMode)
        {
            switch (scene.name)
            {
                case "MainMenu" : DrawCanvas(Enums.MajorCanvas.menu);
                    break;
                case "Levels" : DrawCanvas(Enums.MajorCanvas.levels);
                    break;
            }
        }

        public void DrawCanvas(Enums.MajorCanvas canvas)
        {
            LoadAssetWithCallback<GameObject>(canvasAddress[canvas], DrawCanvasAsync);
        }

        void DrawCanvasAsync(GameObject canvas)
        {
            var go = Object.Instantiate(canvas);
            var canvasUtilities = go.GetComponent<CanvasUtilities>();
            if(!canvasUtilities) canvasUtilities = go.GetComponentInChildren<CanvasUtilities>();
            SetObjectDependencies(canvasUtilities);
            canvasUtilities.Init();
        }

        public void GeneratePopUp(string title, string message, Sprite icon = null)
        {
            throw new System.NotImplementedException();
        }

        public void GenerateLoadingScreen(string loadingName, float loadingMaxValue)
        {
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.SetLoader(loadingName, loadingMaxValue);
        }

        public void UpdateLoadingScreen(float progressValue)
        {
            throw new System.NotImplementedException();
        }

        public void HideLoadingScreen()
        {
            loadingScreen.gameObject.SetActive(false);
        }
    }
}