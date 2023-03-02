using Levels;

namespace Archi.Service.Interface
{
    public interface IGameService : IService
    {

        public void OpenLevel(LevelData levelToOpen);
        
        void CreateLoading();
        void UpdateLoading();
        void FinishLoading();
    }
}