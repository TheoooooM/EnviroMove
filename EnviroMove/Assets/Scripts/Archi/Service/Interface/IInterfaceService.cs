using UnityEngine;

namespace Archi.Service.Interface
{
    public interface IInterfaceService : IService
    {
        void DrawCanvas(Enums.MajorCanvas canvas);

        public void GeneratePopUp(string title, string message, Sprite icon = null);

        void GenerateLoadingScreen(string loadingName, float loadingMaxValue);
        void UpdateLoadingScreen(float progressValue);
    }
}