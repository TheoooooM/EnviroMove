using UnityEngine;

namespace Archi.Service.Interface
{
    public interface IInterfaceService : IService {
        void DrawCanvas(Enums.MajorCanvas canvas);

        public void GeneratePopUp(string title, string message, Sprite icon = null);

        void GenerateLoadingScreen(string loadingName, float loadingMaxValue);
        void UpdateLoadingScreen(float progressValue);
        void HideLoadingScreen();

        public void SetTargetPage(PageDirection page, float value);
        public PageDirection GetTargetPage();
        public void SetNextLevelSO(LevelSO nextLevel);
        public LevelSO GetNextLevelSO();
    }
}