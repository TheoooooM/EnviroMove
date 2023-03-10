using Levels;

namespace Archi.Service.Interface
{
    public interface IToolService : IService
    {
        public void ShowLevels();
        
        public void ShowTool();
        
        LevelData GetDataCreation();

        public void OpenLevel(LevelData data);

        public void CleanScene();
        
        public void ChangePrefab(int index);
        
        public void SwitchMode(int index);
        
        public void ChangeMoveCamera();

        public void PlaceBlock(int indexBlock);

        public void SaveData();
    }
}