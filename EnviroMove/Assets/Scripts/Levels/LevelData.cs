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
        public Dictionary<Vector3, int> blockDict = new (); //Grid by index of blocksUse
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
                blocksUse = new[] {Blocks.BlockType[(Enums.blockType)Random.Range(0,Blocks.BlockType.Count-1)],Blocks.BlockType[(Enums.blockType)Random.Range(0,Blocks.BlockType.Count-1)],Blocks.BlockType[(Enums.blockType)Random.Range(0,Blocks.BlockType.Count-1)] };
                var x = Random.Range(0, 10);
                var y = Random.Range(0, 10);
                var z = Random.Range(0, 10);
                blockGrid = new int[x,y,z];
                size = new Vector3Int(x, y, z);
                for (int _x= 0; _x <x; _x++)
                {
                    for (int _y = 0; _y < y; _y++)
                    {
                        for (int _z = 0; _z < z; _z++)
                        {
                            blockGrid[_x, _y, _z] = Random.Range(0, blocksUse.Length);
                            //Debug.Log(new Vector3Int(_x,_y,_z) + "=" + blockGrid[_x, _y, _z]);
                            blockDict.Add(new Vector3Int(_x,_y,_z), Random.Range(0, blocksUse.Length));
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
