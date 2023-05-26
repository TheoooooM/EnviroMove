using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archi.Service.Interface;
using BDD;
using Firebase.Database;
using Levels;
using UnityEngine;
using Directory = System.IO.Directory;
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
            //UpdateData();
        }
        // Remove DB : dbReference.Child("Levels").RemoveValueAsync();

        public string LevelPath()
        {
            return levelPath;
        }

        public string InfoPath()
        {
            return infoPath;
        }

        public void SetUsername(string name) => PlayerPrefs.SetString("Username", name);

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

        public LevelInfo GetInfoByID(string id)
        {
            return container.allInfoDatas.FirstOrDefault(i => i.id == id);
        }
        
        public void RefreshLevelData() => container.Init(this);

        /// <summary>
        /// Create Data On DataBase
        /// </summary>
        /// <param name="data"> Level Data to upload</param>
        /// <param name="id"> Id of this level</param>
        /// <param name="userName">Username of the Creator</param>
        /// <exception cref="NotImplementedException"></exception>
        public void CreateData(string data, string id)
        {
            var info = GetInfoByID(id);
            if (info == default) throw new NullReferenceException($"Missing Info from level : {id}");
            string username = PlayerPrefs.GetString("Username");
            if (username == string.Empty) username = "unnamed";
            Debug.Log($"username:{username}, id:{id}");
            dbReference.Child("Levels").Child(username).Child(id).SetRawJsonValueAsync(data);
            dbReference.Child("Infos").Child(username).Child(id).SetRawJsonValueAsync(JsonUtility.ToJson(info));
        }

        void CreateInfoOnCloud(string username, string id, LevelInfo info)
        {
            dbReference.Child("Infos").Child(username).Child(id).SetRawJsonValueAsync(JsonUtility.ToJson(info));
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
        public async void UpdateData()
        {
            var startTime = Time.time;
            Debug.Log("Start Load Levels");
            DataSnapshot dataFiles = await dbReference.Child("Infos").GetValueAsync();
            Debug.Log($"Load Levels in {Time.time - startTime} sec");
            foreach (var user in dataFiles.Children)
            {
                foreach (var info in user.Children)
                {
                    var currentInfo = ConvertObjectToInfo(info.Value);

                    if (!container.allInfoDatas.Any(i => i.id == currentInfo.id))
                    {
                        GenerateInfoLevel(currentInfo);
                    }
                }
            }

            throw new System.NotImplementedException();
        }

        public void GenerateDataLevel(LevelData data, string levelName = "unnamed Level")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create Data on Device
        /// </summary>
        /// <param name="data">Level Data to Save</param>
        /// <param name="key"> j'ai oublié</param>
        public void GenerateDataLevel(LevelData data, string levelName = "unnamed", bool createInfo = true)
        {
            if (data.id == default) data.id = GetUniqueIdentifier();
            data.levelName = levelName;
            if (data.creator == default) data.creator = PlayerPrefs.GetString("Username");
            string currentLevelPath = $"{levelPath}{data.id}.json";
            FileStream ft = File.Create(currentLevelPath);
            ft.Close();
            TextWriter tw = new StreamWriter($"{levelPath}{data.id}.json");
            tw.WriteLine((string)data);
            tw.Close();

            GenerateInfoLevel(new LevelInfo(levelName, data.id, data.creator, currentLevelPath));
        }

        void GenerateInfoLevel(LevelInfo info)
        {
            string currentInfoPath = $"{infoPath}{info.id}.json";
            Debug.Log($"Save At {currentInfoPath}");
            FileStream ftInfo = File.Create(currentInfoPath);
            ftInfo.Close();
            TextWriter twInfo = new StreamWriter($"{infoPath}{info.id}.json");
            twInfo.WriteLine(JsonUtility.ToJson(info));
            //File.WriteAllText(currentInfoPath,JsonUtility.ToJson(currentInfo));
            twInfo.Close();
        }

        /// <summary>
        /// Update Data on Device
        /// </summary>
        /// <param name="jsonData">New Json</param>
        /// <param name="dataId"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void UpdateDataLevel(string jsonData, string dataId)
        {
            File.WriteAllText($"{levelPath}{dataId}.json", jsonData);
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

        #region Convertor

        int[] ConvertObjectsToInt(IEnumerable<object> objects)
        {
            IEnumerator<object> enumerator = objects.GetEnumerator();
            List<int> values = new();
            while (enumerator.MoveNext())
            {
                values.Add(Convert.ToInt32((long)enumerator.Current));
            }

            return values.ToArray();
        }

        LevelInfo ConvertObjectToInfo(object info)
        {
            var values = (Dictionary<string, object>)info;

            string levelName = (string)values["levelName"];
            string creator = (string)values["creator"];
            string id = (string)values["id"];
            int like = Convert.ToInt32((long)values["like"]);
            int weekLike = Convert.ToInt32((long)values["weekLike"]);
            int carrotAmount = Convert.ToInt32((long)values["carrotAmount"]);
            int goldValue = Convert.ToInt32((long)values["goldValue"]);
            int difficulty = Convert.ToInt32((long)values["difficulty"]);
            int season = Convert.ToInt32((long)values["season"]);
            bool wasTrending = (bool)values["wasTrending"];
            bool wasDaysMap = (bool)values["wasDaysMap"];
            int timesPlay = Convert.ToInt32((long)values["season"]);

            return new LevelInfo(levelName, creator, id, like, weekLike, carrotAmount, goldValue, difficulty, season,
                wasTrending, wasDaysMap, timesPlay);

        }

        LevelData ConvertObjectToLevelData(object level)
        {
            var values = (Dictionary<string, object>)level;
            var items = values["blockEnumerable"];
            int[] blockEnumerable = ConvertObjectsToInt((List<object>)values["blockEnumerable"]);
            Vector3Int size = ConvertObjectToVector3Int(values["size"]);
            string[] blockUsed = ConvertObjectsToString((List<object>)values["blocksUsed"]);
            int[] blockHorizontalRotationEnumerable =
                ConvertObjectsToInt((List<object>)values["_blockHorizontalRotationEnumerable"]);
            int[] blockVerticalRotationEnumerable =
                ConvertObjectsToInt((List<object>)values["_blockVerticalRotationEnumerable"]);
            int[] playerDirEnumerable = ConvertObjectsToInt((List<object>)values["playerDirEnumerable"]);

            return new LevelData(size, blockEnumerable, blockUsed, blockHorizontalRotationEnumerable,
                blockVerticalRotationEnumerable, playerDirEnumerable);
        }

        Vector3Int ConvertObjectToVector3Int(object value)
        {
            var values = (Dictionary<string, object>)value;
            var x = Convert.ToInt32((long)values["x"]);
            var y = Convert.ToInt32((long)values["y"]);
            var z = Convert.ToInt32((long)values["z"]);
            return new Vector3Int(x, y, z);
        }

        string[] ConvertObjectsToString(IEnumerable<object> objects)
        {
            IEnumerator<object> enumerator = objects.GetEnumerator();
            List<string> values = new();
            while (enumerator.MoveNext())
            {
                values.Add(Convert.ToString(enumerator.Current));
            }

            return values.ToArray();

            /*
            var list = new List<bool>();
            do
            {
                if(enumerator.Current == null) ints.Add(default);
                else ints.Add((int)enumerator.Current);
            } while (enumerator.MoveNext());

            return ints.ToArray();
            */
        }

        #endregion

    }
} 