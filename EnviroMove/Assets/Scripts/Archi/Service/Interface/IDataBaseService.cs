using Google.MiniJSON;
using Levels;
using UnityEngine;
using LevelData = Levels.LevelData;

namespace Archi.Service.Interface
{
    public interface IDataBaseService : IService
    {
        public string LevelPath();
        public string InfoPath();
        
        string[] GetAllLevels();
        LevelInfo[] GetAllLevelInfos();
        string[] GetUserLevels(string id);
        LevelData GetLevel(string path);
        
        void CreateData(string data, string id);
        void DeleteData(string id);
        void UpdateData();
        
        void GenerateDataLevel(LevelData data, string levelName = "unnamed Level");
        void UpdateDataLevel(string jsonData, string dataId);
        void RemovedataLevel(string key);
        
        string GetUniqueIdentifier();
    }
}