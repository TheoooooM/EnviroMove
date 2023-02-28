using System.Collections.Generic;
using Google.MiniJSON;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Levels
{
    public class LevelData : MonoBehaviour
    {
        public string id;

        public Vector3Int size;
        public int[,,] blockGrid; //Grid by index of blocksUse
        public string[] blocksUse;

        public LevelData(Vector3Int levelSize, int[,,] levelBlockGrid, GameObject[] levelBlocksUse)
        {

        }

        GameObject[] GetBlocksUse()
        {
            List<GameObject> go = new();
            Addressables.LoadAssetsAsync<GameObject>(blocksUse, o => { go.Add(o); });
            return null;
        }

        public static implicit operator string(LevelData levelData)
        {
            var dict = new Dictionary<string, object>
            {
                {"id", levelData.id},
                {"size", levelData.size},
                {"blockGrid", levelData.blockGrid},
                {"blocksUse", levelData.blocksUse}
            };
            return Json.Serialize(dict);
        }
    
        public static explicit operator LevelData(string levelData)
        {
            var level = JsonUtility.FromJson<LevelData>(levelData);
            // var dict = Json.Deserialize(levelData) as Dictionary<string, object>;
            // var level = new LevelData(
            //     (Vector3Int) dict["size"],
            //     (int[,,]) dict["blockGrid"],
            //     (GameObject[]) dict["blocksUse"]
            // );
            return level;
        }
    }
}
