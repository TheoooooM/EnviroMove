using Levels;
using UnityEngine;
using LevelData = Levels.LevelData;

namespace Archi.Service.Interface
{
    public interface ILevelService : IService
    {
        Level LoadLevel(Levels.LevelData data, GameObject levelContainer = null); //Instantiate Level with Level script
        
        LevelData GetData(Level data); //Get Level From Data

        LevelData GetCurrentLevelData();
    }
}