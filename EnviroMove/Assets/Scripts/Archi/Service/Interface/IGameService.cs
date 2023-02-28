namespace Archi.Service.Interface
{
    public interface IGameService : IService
    {
        void ChangeScene(Enums.SceneType type);
        
        void CreateLoading();
        void UpdateLoading();
        void FinishLoading();
    }
}