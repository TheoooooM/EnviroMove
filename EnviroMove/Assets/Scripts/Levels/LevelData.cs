using System.Collections.Generic;
using System.Linq;
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
        public int[] blockEnumerable;
        public string[] blocksUse; //Block Address from Addressable
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
                var x = Random.Range(1, 10);
                var y = Random.Range(1, 10);
                var z = Random.Range(1, 10);
                blockGrid = new int[x,y,z];
                blockEnumerable = new int[x * y * z];
                Debug.Log(blockEnumerable.Length);
                size = new Vector3Int(x, y, z);
                int i = 0;
                for (int _z= 0; _z <z; _z++)
                {
                    for (int _y = 0; _y < y; _y++)
                    {
                        for (int _x = 0; _x < x; _x++)
                        {
                            int index =Random.Range(0, 3);
                            blockGrid[_x, _y, _z] = index;
                            blockEnumerable[i] = index; // _x + _y * x + _z * y * x
                            i++;
                            //Debug.Log(new Vector3Int(_x,_y,_z) + "=" + blockGrid[_x, _y, _z]);
                            //blockDict.Add(new Vector3Int(_x,_y,_z), Random.Range(0, blocksUse.Length));
                        }
                    }
                }
            }
        }

        static int[,,] BlockEnumerable(IEnumerable<int> sequenceToRead,Vector3Int arraySize)
        {
            IEnumerator<int> sequenceEnumerator = sequenceToRead.GetEnumerator();
            Vector3Int position = Vector3Int.zero;
            int[,,] array3 = new int[arraySize.x, arraySize.y, arraySize.z];
            while (sequenceEnumerator.MoveNext())
            {
                position.x++;
                if (position.x == arraySize.x)
                {
                    position.x = 0;
                    position.y++;
                    if (position.y == arraySize.y)
                    {
                        position.y = 0;
                        position.z++;
                    }
                    
                }
                array3[position.x, position.y, position.z] = sequenceEnumerator.Current;

            }

            return array3;
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
            level.blockGrid = BlockEnumerable(level.blockEnumerable, level.size);
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
