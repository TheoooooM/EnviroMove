using System;
using System.Collections.Generic;
using System.Linq;
using Google.MiniJSON;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Levels
{
    public class LevelData
    {
        public string id;

        public Vector3Int size;
        public int[,,] blockGrid; //Grid by index of blocksUse
        public int[] blockEnumerable;
        public string[] blocksUsed; //lock Address from Addressable
        public int[,,] blockHorizontalRotationGrid;
        public int[,,] blockVerticalRotationGrid;
        

        public LevelData(Vector3Int size, int[] blockEnumerable, string[] levelBlocksUsed, int[,,] blockHorizontalRotationGrid, int[,,] blockVerticalRotationGrid)
        {
            blocksUsed = levelBlocksUsed;
            this.blockEnumerable = blockEnumerable;
            this.size = size;
            this.blockHorizontalRotationGrid = blockHorizontalRotationGrid;
            this.blockVerticalRotationGrid = blockVerticalRotationGrid;
        }

        public LevelData(Vector3Int size, int[,,] blockGrid, string[] levelBlocksUsed, int[,,] blockHorizontalRotationGrid, int[,,] blockVerticalRotationGrid)
        {
            blocksUsed = levelBlocksUsed;
            this.blockGrid = blockGrid;
            blockEnumerable = From3DTo1DArray(blockGrid);
            this.size = size;
            this.blockHorizontalRotationGrid = blockHorizontalRotationGrid;
            this.blockVerticalRotationGrid = blockVerticalRotationGrid;
        }


        public LevelData(bool random)
        {
            if (random)
            {
                blocksUsed = new[]
                {
                    Blocks.BlockType[(Enums.blockType)Random.Range(0, Blocks.BlockType.Count - 1)],
                    Blocks.BlockType[(Enums.blockType)Random.Range(0, Blocks.BlockType.Count - 1)],
                    Blocks.BlockType[(Enums.blockType)Random.Range(0, Blocks.BlockType.Count - 1)]
                };
                var x = Random.Range(1, 10);
                var y = Random.Range(1, 10);
                var z = Random.Range(1, 10);
                blockGrid = new int[x, y, z];
                blockEnumerable = new int[x * y * z];
                Debug.Log(blockEnumerable.Length);
                size = new Vector3Int(x, y, z);
                int i = 0;
                for (int _z = 0; _z < z; _z++)
                {
                    for (int _y = 0; _y < y; _y++)
                    {
                        for (int _x = 0; _x < x; _x++)
                        {
                            int index = Random.Range(0, 3);
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

        static int[,,] BlockEnumerable(IEnumerable<int> sequenceToRead, Vector3Int arraySize)
        {
            IEnumerator<int> sequenceEnumerator = sequenceToRead.GetEnumerator();
            Vector3Int position = Vector3Int.zero;
            int[,,] array3 = new int[arraySize.x, arraySize.y, arraySize.z];

            while (sequenceEnumerator.MoveNext())
            {
                array3[position.x, position.y, position.z] = sequenceEnumerator.Current;
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
            }

            return array3;
        }

        private static int[] From3DTo1DArray(int[,,] threeDArray)
        {
            var oneDList = new List<int>();

            for (int z = 0; z < threeDArray.GetLength(2); z++)
            {
                for (int y = 0; y < threeDArray.GetLength(1); y++)
                {
                    for (int x = 0; x < threeDArray.GetLength(0); x++)
                    {
                        oneDList.Add(threeDArray[x, y, z]);
                    }
                }
            }

            return oneDList.ToArray();
        }

        public static explicit operator string(LevelData levelData)
        {
            return JsonUtility.ToJson(levelData);
        }

        public static explicit operator LevelData(string levelData)
        {
            // Debug.Log(levelData);
            var level = JsonUtility.FromJson(levelData, typeof(LevelData)) as LevelData;
            // Debug.Log((string)level);
            level.blockGrid = BlockEnumerable(level.blockEnumerable, level.size);
            level.blockHorizontalRotationGrid = BlockEnumerable(level.blockEnumerable, level.size);
            level.blockVerticalRotationGrid = BlockEnumerable(level.blockEnumerable, level.size);
            return level;
        }
    }
}