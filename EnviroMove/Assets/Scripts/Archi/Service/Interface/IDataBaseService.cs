using Google.MiniJSON;
using Levels;
using UnityEngine;
using LevelData = Levels.LevelData;

namespace Archi.Service.Interface
{
    public interface IDataBaseService : IService
    {
        public string LevelPath();
        
        string[] GetUserLevels(string id);
        string[] GetAllLevels();

        void CreateData(LevelData data, string id, string userName);
        void DeleteData(string id);
        void UpdateData();
        
        void GenerateDataLevel(LevelData data, string key = null);
        void UpdateDataLevel(string jsonData, string dataId);
        void RemovedataLevel(string key);
        
        string GetUniqueIdentifier();
    }
}