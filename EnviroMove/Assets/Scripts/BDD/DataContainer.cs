using System;
using System.Collections.Generic;
using System.IO;
using Archi.Service.Interface;
using Levels;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace BDD
{
    public class DataContainer
    {
        public List<LevelInfo> allInfoDatas = new ();

        public void Init(IDataBaseService dataBase) // Récupère tout les LevelInfo
        {
            Debug.Log($"init");
            var infoPaths = Directory.GetFiles(dataBase.InfoPath());

            foreach (var infoPath in infoPaths)
            {
                LevelInfo info = JsonUtility.FromJson<LevelInfo>(File.ReadAllText(infoPath));
                allInfoDatas.Add(info);
            }
            Debug.Log($"Load {allInfoDatas.Count} infos");
        }
    }
}