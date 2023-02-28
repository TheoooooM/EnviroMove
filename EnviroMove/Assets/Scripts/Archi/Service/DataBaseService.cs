using System;
using System.Linq;
using Archi.Service.Interface;
using BDD;
using Levels;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Windows;
using File = System.IO.File;
using LevelData = Levels.LevelData;

namespace Archi.Service
{
    public class DataBaseService : Service, IDataBaseService
    {
        private DataContainer container = new();
        private string levelPath;
        
        protected override void Initialize()
        {
            levelPath = $"{Application.persistentDataPath}/SaveData/Level/";
        }

        public string LevelPath() {return levelPath;}

        /// <summary>
        ///  //Get level of a User 
        /// </summary>
        /// <param name="id">ID of the user</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string[] GetUserLevels(string id) 
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get All Levels on the DataBase
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string[] GetAllLevels()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Create Data On DataBase
        /// </summary>
        /// <param name="data"> Level Data to upload</param>
        /// <param name="id"> Id of this level</param>
        /// <param name="userName">Username of the Creator</param>
        /// <exception cref="NotImplementedException"></exception>
        public void CreateData(LevelData data, string id, string userName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Delete Data On DataBase
        /// </summary>
        /// <param name="id">Id of the level</param>
        /// <exception cref="NotImplementedException"></exception>
        public void DeleteData(string userName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///  Update Data On Device from DataBase
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateData()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Create Data on Device
        /// </summary>
        /// <param name="data">Level Data to Save</param>
        /// <param name="key"> j'ai oublié</param>
        public void GenerateDataLevel(LevelData data, string key = null)
        {
            var json = JsonUtility.ToJson(data);

            if (data.id == default) data.id = GetUniqueIdentifier();

            string path = $"{levelPath}{data.id}.json";
            File.WriteAllText(path,json);

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup g = settings.FindGroup("Levels");
            var guid = AssetDatabase.AssetPathToGUID(path);
            var entry = settings.CreateOrMoveEntry(guid, g);
            entry.labels.Add("LevelData");
        }

        /// <summary>
        /// Update Data on Device
        /// </summary>
        /// <param name="jsonData">New Json</param>
        /// <param name="dataId"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateDataLevel(string jsonData, string dataId)
        {
            File.WriteAllText($"{levelPath}{dataId}.json",jsonData);
        }

        /// <summary>
        ///  Remove Data on Device
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemovedataLevel(string key)
        {
            File.Delete($"{levelPath}{key}.json");
        }

        public string GetUniqueIdentifier()
        {
            throw new System.NotImplementedException();
        }
    }
}