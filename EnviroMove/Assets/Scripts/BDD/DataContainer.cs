using System;
using System.Collections.Generic;
using Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BDD
{
    public class DataContainer
    {
        public List<LevelInfo> allDatas;

        public DataContainer() // Récupère tout les addressable avec le label Level
        {
            // Addressables.LoadAssetsAsync<string>("info", (string json) =>
            // {
            //     allDatas.Add(JsonUtility.FromJson<LevelInfo>(json));
            //     Debug.Log($"Found {allDatas.Count} item(s) ");
            // });
        }
    }
}