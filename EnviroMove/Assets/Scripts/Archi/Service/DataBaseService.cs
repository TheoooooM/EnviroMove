using System;
using System.IO;
using System.Linq;
using Archi.Service.Interface;
using Attributes;
using BDD;
using Firebase.Database;
using Levels;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Windows;
using Directory = UnityEngine.Windows.Directory;
using File = System.IO.File;
using LevelData = Levels.LevelData;
using Random = UnityEngine.Random;

namespace Archi.Service
{
    public class DataBaseService : Service, IDataBaseService
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        
        private string levelPath;
        private string infoPath;

        private DatabaseReference dbReference;
        
        private DataContainer container = new();

        protected override void Initialize()
        {
            if (!File.Exists($"{Application.persistentDataPath}/SaveData")) Directory.CreateDirectory($"{Application.persistentDataPath}/SaveData");
            if (!File.Exists($"{Application.persistentDataPath}/SaveData/Level")) Directory.CreateDirectory($"{Application.persistentDataPath}/SaveData/Level");
            levelPath = $"{Application.persistentDataPath}/SaveData/Level/";
            
            if (!File.Exists($"{Application.persistentDataPath}/SaveData/Infos")) Directory.CreateDirectory($"{Application.persistentDataPath}/SaveData/Infos");
            infoPath = $"{Application.persistentDataPath}/SaveData/Infos/";

            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            
            container.Init(this);
        }

        public string LevelPath() {return levelPath;}
        public string InfoPath() {return infoPath;}

        /// <summary>
        /// Get All Levels on the DataBase
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string[] GetAllLevels()
        {
            throw new NotImplementedException();
        }

        public LevelInfo[] GetAllLevelInfos()
        {
            return container.allInfoDatas.ToArray();
        }

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

        public LevelData GetLevel(string path)
        {
            return (LevelData)File.ReadAllText(path);
        }
        
        public LevelData GetLevelByName(string levelName)
        {
            var info = container.allInfoDatas.FirstOrDefault(i => i.levelName == levelName);
            if (info == default) throw new NullReferenceException($"Level of Name {levelName} doesn't exist on Device");
            return (LevelData)File.ReadAllText(info.levelFilePath);
        }
        

        

        /// <summary>
        /// Create Data On DataBase
        /// </summary>
        /// <param name="data"> Level Data to upload</param>
        /// <param name="id"> Id of this level</param>
        /// <param name="userName">Username of the Creator</param>
        /// <exception cref="NotImplementedException"></exception>
        public void CreateData(string data, string id)
        {
            string username = PlayerPrefs.GetString("Username");
            if (username == string.Empty) username = "unnamed";
            dbReference.Child("Levels").Child(username).Child(id).SetRawJsonValueAsync(data);
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
        public void GenerateDataLevel(LevelData data, string levelName = "unnamed")
        {
            if (data.id == default) data.id = GetUniqueIdentifier();
            string currentLevelPath =  $"{levelPath}{data.id}.json";
            FileStream ft = File.Create(currentLevelPath);
            ft.Close();
            TextWriter tw = new StreamWriter($"{levelPath}{data.id}.json");
            tw.WriteLine((string)data);
            tw.Close();
           
            CreateData((string)data, data.id);
            
            string currentInfoPath =  $"{infoPath}{data.id}.json";
            var currentInfo = new LevelInfo(levelName, PlayerPrefs.GetString("Username"), currentLevelPath);
            FileStream ftInfo = File.Create(currentInfoPath);
            ftInfo.Close();
            TextWriter twInfo = new StreamWriter($"{infoPath}{data.id}.json");
            twInfo.WriteLine(JsonUtility.ToJson(currentInfo));
            //File.WriteAllText(currentInfoPath,JsonUtility.ToJson(currentInfo));
            twInfo.Close();

            /* Set As Adressable (/!\En dehors du projet)
             var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup g = settings.FindGroup("Levels");
            var guid = AssetDatabase.(path);
            Debug.Log("settings" + settings);
            Debug.Log("guid" + guid);
            Debug.Log("g" + g);
            var entry = settings.CreateOrMoveEntry(guid, g);
            entry.labels.Add("LevelData");*/
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
            string id = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}-";
            for (int i = 0; i < 6; i++)
            {
                char nChar = chars.ToCharArray()[Random.Range(0, chars.Length)];
                id += nChar;
            }
            id += $"-{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}";
            return id;
        }
    }
}