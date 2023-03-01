using System.Collections.Generic;
using Google.MiniJSON;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Levels
{
    public class LevelData
    {
        public string id;

        public Vector3Int size;
        public int[,,] blockGrid; //Grid by index of blocksUse
        public string[] blocksUse;

        public LevelData( int[,,] levelBlockGrid, string[] levelBlocksUse)
        {
            blocksUse = levelBlocksUse;
            blockGrid = levelBlockGrid;
            size = new Vector3Int(blockGrid.GetLength(0), blockGrid.GetLength(1), blockGrid.GetLength(2));

        }

        public LevelData(bool random)
        {
            if (random)
            {
                blocksUse = new[] {Blocks.strings[(Enums.blockType)Random.Range(0,Blocks.strings.Count-1)],Blocks.strings[(Enums.blockType)Random.Range(0,Blocks.strings.Count-1)],Blocks.strings[(Enums.blockType)Random.Range(0,Blocks.strings.Count-1)] };
                blockGrid = new int[Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)];
                for (int x = 0; x < blockGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < blockGrid.GetLength(1); y++)
                    {
                        for (int z = 0; z < blockGrid.GetLength(0); z++)
                        {
                            blockGrid[x, y, z] = Random.Range(0, blocksUse.Length);
                        }
                    }
                }
            }
        }

        GameObject[] GetBlocksUse()
        {
            List<GameObject> go = new();
            Addressables.LoadAssetsAsync<GameObject>(blocksUse, o => { go.Add(o); });
            return null;
        }

        public static explicit operator string(LevelData levelData)
        {
            return JsonUtility.ToJson(levelData);
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
