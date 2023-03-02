using System;
using Archi.Service.Interface;
using Attributes;
using UI;
using UI.Canvas;
using UnityEngine;
using static AdresseHelper;
using Object = UnityEngine.Object;

namespace Archi.Service
{
    public class InterfaceService : Service, IInterfaceService
    {
        [DependeOnService] private IGameService m_Game;
        [DependeOnService] private IToolService m_Tool;
        [DependeOnService] private IDataBaseService m_data;
        
        protected override void Initialize()
        { }

        public void DrawCanvas(Enums.MajorCanvas canvas)
        {
            string address = "";
            switch (canvas)
            {
                case Enums.MajorCanvas.menu:
                    address = "MainMenuCanvas";
                    break;
                case Enums.MajorCanvas.inGame:
                    break;
                case Enums.MajorCanvas.tool:
                    address = "ToolCanvas";
                    break;
                case Enums.MajorCanvas.levels:
                    break;
                case Enums.MajorCanvas.toolLevels:
                    address = "LevelSelectorCanvas";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(canvas), canvas, null);
            }
            LoadAssetWithCallback<GameObject>(address, DrawCanvasAsync);
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
            throw new System.NotImplementedException();
        }

        public void UpdateLoadingScreen(float progressValue)
        {
            throw new System.NotImplementedException();
        }

        public void HideLoadingScreen()
        {
            throw new System.NotImplementedException();
        }
    }
}