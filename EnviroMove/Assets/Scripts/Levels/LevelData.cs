using System;
using System.Collections.Generic;
using System.Linq;
using Google.MiniJSON;
using Levels;
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
        public int[] _blockHorizontalRotationEnumerable;
        public int[,,] blockVerticalRotationGrid;
        public int[] _blockVerticalRotationEnumerable;
        public int[,,] playerDir;
        public int[] playerDirEnumerable;
        public int[,] levelGridPosition;
        

        public LevelData(Vector3Int size, int[] blockEnumerable, string[] levelBlocksUsed, int[,,] blockHorizontalRotationGrid, int[,,] blockVerticalRotationGrid)
        {
            blocksUsed = levelBlocksUsed;
            this.blockEnumerable = blockEnumerable;
            this.size = size;
            this.blockHorizontalRotationGrid = blockHorizontalRotationGrid;
            this.blockVerticalRotationGrid = blockVerticalRotationGrid;
        }

        public LevelData(Vector3Int size, int[,,] blockGrid, string[] levelBlocksUsed, int[,,] blockHorizontalRotationGrid, int[,,] blockVerticalRotationGrid, Vector3[,,] playerDirGrid)
        {
            blocksUsed = levelBlocksUsed;
            this.blockGrid = blockGrid;
            blockEnumerable = From3DTo1DArray(blockGrid);
            this.size = size;
            this.blockHorizontalRotationGrid = blockHorizontalRotationGrid;
            _blockHorizontalRotationEnumerable = From3DTo1DArray(this.blockHorizontalRotationGrid);
            this.blockVerticalRotationGrid = blockVerticalRotationGrid;
            _blockVerticalRotationEnumerable = From3DTo1DArray(this.blockVerticalRotationGrid);
            playerDir = Vector3ToSideArray(playerDirGrid);
            playerDirEnumerable = From3DTo1DArray(playerDir);
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
            if(sequenceToRead == null) {Debug.LogError("Missing Sequence To Read");
                return new int[arraySize.x, arraySize.y, arraySize.z];
            }
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

        int[,,] Vector3ToSideArray(Vector3[,,] vectorArray)
        {
            var size = new Vector3Int(vectorArray.GetLength(0), vectorArray.GetLength(1), vectorArray.GetLength(2));
            int[,,] sideArray = new int[size.x,size.y,size.z];
            for (int z = 0; z < size.z ; z++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int x = 0; x < size.x; x++)
                    {
                        if (vectorArray[x, y, z] == Vector3.forward) sideArray[x, y, z] = (int)Enums.Side.forward;
                        else if(vectorArray[x, y, z] == Vector3.back)sideArray[x, y, z] = (int)Enums.Side.back;
                        else if(vectorArray[x, y, z] == Vector3.left)sideArray[x, y, z] = (int)Enums.Side.left;
                        else if(vectorArray[x, y, z] == Vector3.right)sideArray[x, y, z] = (int)Enums.Side.right;
                        else sideArray[x, y, z] = (int)Enums.Side.none;
                    }
                }
            }
            return sideArray;
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
            level.blockHorizontalRotationGrid = BlockEnumerable(level._blockHorizontalRotationEnumerable, level.size);
            level.blockVerticalRotationGrid = BlockEnumerable(level._blockVerticalRotationEnumerable, level.size);
            level.playerDir = BlockEnumerable(level.playerDirEnumerable, level.size);
            return level;
        }
    }
}

public class ListOfLevelData
{
    public List<LevelData> levels = new List<LevelData>();


    public void AddLevelDataToList(LevelData levelData)
    {
        levels.Add(levelData);
    }

    public static explicit operator string(ListOfLevelData listOfLevelData)
    {
        return JsonUtility.ToJson(listOfLevelData);
    }

    public static explicit operator ListOfLevelData(string listOfLevelData)
    {
        return JsonUtility.FromJson(listOfLevelData, typeof(ListOfLevelData)) as ListOfLevelData;
    }
}