using System;
using System.Collections.Generic;
using System.Linq;
using Archi.Service.Interface;
using Attributes;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
#if UNITY_STANDALONE && !UNITY_EDITOR
using JsonUtility = UnityEngine.JsonUtility;
#endif

public class SceneEditor
{
    #region Variables

    [ServiceDependency] public IDataBaseService m_Data;

    private GameObject[] prefabs;

    private GameObject selectedPrefab;
    public int selectedPrefabIndex = 1;

    private readonly int sizeOfGridSpace = 1;

    private Camera _camera;

    private GameObject parent;

    private string path;
    private TMP_InputField inputField;

    public bool isMoveCamera = true;

    //LevelData
    public Vector3Int size;
    public Vector3Int defaultSize = new(160, 2, 160);
    public Vector2Int tileSize = new(6, 12);
    public int[,,] blockGrid;
    public Enums.Side[,,] blockHorizontalRotationGrid;
    public Enums.Side[,,] blockVerticalRotationGrid;
    public List<string> blocksUsed;
    public LevelData data;

    //LevelDataList
    public int[,] levelGridPosition;
    public List<LevelData> levelDataList;

    //Path
    public Enums.Side[,,] directionGrid;

    //alaid
    private Vector2Int firstBlockPosition;
    private Vector2Int[,] gridPositions;
    private readonly int tailleBridge = 4;

    private int season = -1;

    private enum EditorMode
    {
        create,
        delete,
        moveCamera,
        snapMode,
        horizontalRotation,
        verticalRotation
    }

    private EditorMode Mode = EditorMode.create;

    private LevelData curentLevelData;

    public float cameraSpeed = 0.5f;

    private List<int> twoByOnePrefabIndex = new() { 24, 26, 27, 28, 43, 46, 47, 81, 82, 84, 85, 86 };
    private List<int> threeByOnePrefabIndex = new() { 25, 95 };
    private List<int> twoPlusOnePrefabIndex = new() { 29, 30, 48, 49, 87, 88 };
    private List<int> twoByTwoPrefabIndex = new() { 51, 89 };

    private List<VolumeProfile> volumeProfiles;
    private Volume volume;

    #endregion

    public void Start()
    {
        size = defaultSize;
        if (season == -1) season = 2;
        new Blocks();
        // blockGrid = new List<List<List<int>>>();
        blocksUsed = new List<string>();
        blocksUsed.Add(null);
        blockGrid = new int[size.x, size.y, size.z];
        blockHorizontalRotationGrid = new Enums.Side[size.x, size.y, size.z];
        blockVerticalRotationGrid = new Enums.Side[size.x, size.y, size.z];
        prefabs = new GameObject[Blocks.BlockType.Count];
        foreach (var blockAddress in Blocks.BlockType)
        {
            if (blockAddress.Key == Enums.blockType.empty) continue;
            var block = Addressables.LoadAssetAsync<GameObject>(blockAddress.Value).WaitForCompletion();
            prefabs[(int)blockAddress.Key] = block;
        }

        volume = Camera.main.transform.GetChild(0).GetComponent<Volume>();
        volumeProfiles = new List<VolumeProfile>();
        volumeProfiles.Add(Addressables.LoadAssetAsync<VolumeProfile>("PP_Automne").WaitForCompletion());
        volumeProfiles.Add(Addressables.LoadAssetAsync<VolumeProfile>("PP_Hiver").WaitForCompletion());
        volumeProfiles.Add(Addressables.LoadAssetAsync<VolumeProfile>("PP_Printemps").WaitForCompletion());
        volume.profile = volumeProfiles[season];
        _camera = Camera.main;
        parent = new GameObject();
        parent.transform.position = Vector3.zero;
        parent.name = "Grid";
        selectedPrefab = prefabs[1];
        InitializeDirectionGrid();
        PlaceDefaultGround();
        PlaceCamera();
    }

    private void PlaceCamera()
    {
        _camera.transform.position = new Vector3(
            (tileSize.x + tailleBridge) * (size.x / (tileSize.x + tailleBridge) / 2),
            _camera.transform.position.y, (tileSize.y + tailleBridge) * (size.z / (tileSize.y + tailleBridge) / 2));
        _camera.transform.rotation = Quaternion.Euler(80, 0, 0);
    }

    private void InitializeDirectionGrid()
    {
        directionGrid = new Enums.Side[size.x, size.y, size.z];
        for (var x = 0; x < size.x; x++)
        for (var y = 0; y < size.y - 1; y++)
        for (var z = 0; z < size.z; z++)
            directionGrid[x, y, z] = Enums.Side.none;
    }

    private void PlaceDefaultGround()
    {
        var posX = (tileSize.x + tailleBridge) * (size.x / (tileSize.x + tailleBridge) / 2);
        var posZ = (tileSize.y + tailleBridge) * (size.z / (tileSize.y + tailleBridge) / 2);
        MakePlatform(posX, posZ);

        blocksUsed.Add("M1_Block1");
    }

    private void MakePlatform(int posX, int posZ)
    {
        GameObject block = null;
        for (var x = posX; x < posX + tileSize.x; x++)
        {
            for (var z = posZ; z < posZ + tileSize.y; z++)
            {
                switch (season)
                {
                    case 0:
                        block = Object.Instantiate(prefabs[(int)Enums.blockType.M3_Block1], new Vector3(x, 0, z),
                            Quaternion.identity);
                        break;
                    case 1:
                        block = Object.Instantiate(prefabs[(int)Enums.blockType.M2_Block1], new Vector3(x, 0, z),
                            Quaternion.identity);
                        break;
                    case 2:
                        block = Object.Instantiate(prefabs[(int)Enums.blockType.M1_Block1], new Vector3(x, 0, z),
                            Quaternion.identity);
                        break;
                }

                block.transform.SetParent(parent.transform);
                blockGrid[x, 0, z] = (int)Enums.blockType.M1_Block1;
                //random rotation
                var randomRotation = Random.Range(0, 4);
                block.transform.Rotate(0, 0, randomRotation * 90);
                blockHorizontalRotationGrid[posX, 0, posZ] = randomRotation switch
                {
                    0 => Enums.Side.forward,
                    1 => Enums.Side.right,
                    2 => Enums.Side.back,
                    3 => Enums.Side.left,
                    _ => blockHorizontalRotationGrid[posX, 0, posZ]
                };
                randomRotation = Random.Range(0, 4);
                block.transform.Rotate(randomRotation * 90, 0, 0);
                blockVerticalRotationGrid[posX, 0, posZ] = randomRotation switch
                {
                    0 => Enums.Side.forward,
                    1 => Enums.Side.up,
                    2 => Enums.Side.back,
                    3 => Enums.Side.down,
                    _ => blockVerticalRotationGrid[posX, 0, posZ]
                };
                block.transform.name = "Block" + x + "" + 0 + "" + z;
            }
        }

        firstBlockPosition = new Vector2Int(posX, posZ);
        while (firstBlockPosition.x > 0) firstBlockPosition.x -= tileSize.x;
        while (firstBlockPosition.y > 0) firstBlockPosition.y -= tileSize.y;
        firstBlockPosition.x += tileSize.x;
        firstBlockPosition.y += tileSize.y;

        InstantiateNewBorder(posX, posZ);
    }

    private void InstantiateNewBorder(int posX, int posZ)
    {
        // instantiating the selling
        // var block = Object.Instantiate(prefabs[(int)Enums.blockType.SM_BorderVfinal_Roof],
        //     new Vector3(posX + tileSize.x / 2 - .5f, -0.5f, posZ + tileSize.y / 2 - .5f),
        //     Quaternion.identity);
        // block.transform.Rotate(0, 90, 0);
        // block.transform.SetParent(parent.transform);
        // block.name = posX + tileSize.x / 2 - .5f + " " + 2 + " " + (posZ + tileSize.y / 2 - .5f);
        //
        // InsideBottomLeftCorner 102, InsideBottomRightCorner 103, InsideTopRightCorner 104, InsideTopLeftCorner 105
        // InsideTop start at 150 to InsideTop ends at 155
        // InsideLeft start at 156 to InsideLeft ends at 167
        // InsideRight start at 168 to InsideRight ends at 180
        // InsideBottom start at 181 to InsideBottom ends at 186
        //left bottom corner
        var block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideBottomLeftCorner],
            new Vector3(posX - 0.5f, 0.5f, posZ - 0.5f), Quaternion.identity);
        block.transform.SetParent(parent.transform);
        block.transform.Rotate(-90, -90, 0);
        blockGrid[posX - 1, 1, posZ - 1] = (int)Enums.blockType.InsideBottomLeftCorner;
        blockHorizontalRotationGrid[posX - 1, 1, posZ - 1] = Enums.Side.left;
        blockVerticalRotationGrid[posX - 1, 1, posZ - 1] = Enums.Side.down;
        block.name = "Block (" + (posX - 1) + ", " + 1 + ", " + (posZ - 1) + ")";

        //right bottom corner
        block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideBottomRightCorner],
            new Vector3(posX + tileSize.x - .5f, 0.5f, posZ - .5f), Quaternion.identity);
        block.transform.SetParent(parent.transform);
        block.transform.Rotate(-90, 0, -90);
        blockGrid[posX + tileSize.x, 1, posZ - 1] = (int)Enums.blockType.InsideBottomRightCorner;
        blockHorizontalRotationGrid[posX + tileSize.x, 1, posZ - 1] = Enums.Side.right;
        blockVerticalRotationGrid[posX + tileSize.x, 1, posZ - 1] = Enums.Side.forward;
        block.name = "Block (" + (posX + tileSize.x) + ", " + 1 + ", " + (posZ - 1) + ")";

        //right top corner
        block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideTopRightCorner],
            new Vector3(posX + tileSize.x - .5f, 0.5f, posZ + tileSize.y - .5f), Quaternion.identity);
        block.transform.SetParent(parent.transform);
        block.transform.Rotate(-90, 0, -90);
        blockGrid[posX + tileSize.x, 1, posZ + tileSize.y] = (int)Enums.blockType.InsideTopRightCorner;
        blockHorizontalRotationGrid[posX + tileSize.x, 1, posZ + tileSize.y] = Enums.Side.right;
        blockVerticalRotationGrid[posX + tileSize.x, 1, posZ + tileSize.y] = Enums.Side.back;
        block.name = "Block (" + (posX + tileSize.x) + ", " + 1 + ", " + (posZ + tileSize.y) + ")";

        //left top corner
        block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideTopLeftCorner],
            new Vector3(posX - .5f, 0.5f, posZ + tileSize.y - .5f), Quaternion.identity);
        block.transform.SetParent(parent.transform);
        block.transform.Rotate(-90, 0, -90);
        blockGrid[posX - 1, 1, posZ + tileSize.y] = (int)Enums.blockType.InsideTopLeftCorner;
        blockHorizontalRotationGrid[posX - 1, 1, posZ + tileSize.y] = Enums.Side.left;
        blockVerticalRotationGrid[posX - 1, 1, posZ + tileSize.y] = Enums.Side.back;
        block.name = $"Block ({posX - 1}, {1}, {(posZ + tileSize.y)})";
        //left side
        var i = 0;
        for (i = 0; i < tileSize.y; i++)
        {
            block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideLeft1 + i],
                new Vector3(posX - 0.5f, 0.5f, posZ + i - 0.5f), Quaternion.identity);
            block.transform.SetParent(parent.transform);
            block.transform.Rotate(-90, 180, 0);
            blockGrid[posX - 1, 1, posZ + i] = (int)Enums.blockType.InsideLeft1 + i;
            blockHorizontalRotationGrid[posX - 1, 1, posZ + i] = Enums.Side.left;
            blockVerticalRotationGrid[posX - 1, 1, posZ + i] = Enums.Side.forward;
            block.name = "Block (" + (posX - 1) + ", " + 1 + ", " + (posZ + i) + ")";
        }

        //right side
        for (i = 0; i < tileSize.y; i++)
        {
            block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideRight1 + i],
                new Vector3(posX + tileSize.x - .5f, 0.5f, posZ + i + .5f), Quaternion.identity);
            block.transform.SetParent(parent.transform);
            block.transform.Rotate(-90, 0, 0);
            blockGrid[posX + tileSize.x, 1, posZ + i] = (int)Enums.blockType.InsideRight1 + i;
            blockHorizontalRotationGrid[posX + tileSize.x, 1, posZ + i] = Enums.Side.right;
            blockVerticalRotationGrid[posX + tileSize.x, 1, posZ + i] = Enums.Side.forward;
            block.name = "Block (" + (posX + tileSize.x) + ", " + 1 + ", " + (posZ + i) + ")";
        }

        //top side
        for (i = 0; i < tileSize.x; i++)
        {
            block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideTop1 + i],
                new Vector3(posX + i - .5f, 0.5f, posZ + tileSize.y - .5f), Quaternion.identity);
            block.transform.SetParent(parent.transform);
            block.transform.Rotate(-90, -90, 0);
            blockGrid[posX + i, 1, posZ + tileSize.y] = (int)Enums.blockType.InsideTop1 + i;
            blockHorizontalRotationGrid[posX + i, 1, posZ + tileSize.y] = Enums.Side.forward;
            blockVerticalRotationGrid[posX + i, 1, posZ + tileSize.y] = Enums.Side.back;
            block.name = "Block (" + (posX + i) + ", " + 1 + ", " + (posZ + tileSize.y) + ")";
        }

        //bottom side
        for (i = 0; i < tileSize.x; i++)
        {
            block = Object.Instantiate(prefabs[(int)Enums.blockType.InsideBottom1 + i],
                new Vector3(posX + i + .5f, 0.5f, posZ - .5f), Quaternion.identity);
            block.transform.SetParent(parent.transform);
            block.transform.Rotate(-90, 0, 90);
            blockGrid[posX + i, 1, posZ - 1] = (int)Enums.blockType.InsideBottom1 + i;
            blockHorizontalRotationGrid[posX + i, 1, posZ - 1] = Enums.Side.forward;
            blockVerticalRotationGrid[posX + i, 1, posZ - 1] = Enums.Side.forward;
            block.name = "Block (" + (posX + i) + ", " + 1 + ", " + (posZ - 1) + ")";
        }
    }

    private void DestroyPlatform(int posX, int posZ)
    {
        for (var x = posX; x < posX + tileSize.x; x++)
        for (var z = posZ; z < posZ + tileSize.y; z++)
        {
            Object.Destroy(parent.transform.Find(x + " " + 0 + " " + z).gameObject);
            blockGrid[x, 0, z] = 0;
            blockHorizontalRotationGrid[x, 0, z] = Enums.Side.none;
            blockVerticalRotationGrid[x, 0, z] = Enums.Side.none;
        }

        firstBlockPosition = new Vector2Int(posX, posZ);
        while (firstBlockPosition.x > 0) firstBlockPosition.x -= tileSize.x;
        while (firstBlockPosition.y > 0) firstBlockPosition.y -= tileSize.y;
        firstBlockPosition.x += tileSize.x;
        firstBlockPosition.y += tileSize.y;

        // destroy border
        Object.Destroy(parent.transform
            .Find(posX + tileSize.x / 2 - .5f + " " + 0 + " " + (posZ + tileSize.y / 2 - .5f)).gameObject);
    }

    public void Update()
    {
        if (_camera == null) _camera = Camera.main;
        if (parent == null) parent = new GameObject();
        selectedPrefab = prefabs[selectedPrefabIndex];
        if (Input.touchCount <= 0) return;
        switch (Mode)
        {
            case EditorMode.create:
                Create();
                break;

            case EditorMode.delete:
                Delete();
                break;
            case EditorMode.moveCamera:
                MoveCamera();
                break;
            case EditorMode.horizontalRotation:
                HorizontalRotation();
                break;
            case EditorMode.verticalRotation:
                VerticalRotation();
                break;
        }
    }

    private void MoveCamera()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            var touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            _camera.transform.position += new Vector3(-touchDeltaPosition.x * cameraSpeed * Time.deltaTime, 0,
                -touchDeltaPosition.y * cameraSpeed * Time.deltaTime);
        }
    }

    public void SliderCamera(float value)
    {
        cameraSpeed = value;
    }

    private void Create()
    {
        if (IsPointerOverUIObject()) return;
        if (Input.GetTouch(0).phase == TouchPhase.Began && isMoveCamera)
            InstantiateNewBlock();
        else if (Input.GetTouch(0).phase == TouchPhase.Moved && !isMoveCamera) InstantiateNewBlock();
    }

    private bool IsPointerOverUIObject()
    {
        //if (EventSystem.current)
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }

    private void InstantiateNewBlock()
    {
        Vector3 position = Input.GetTouch(0).position;
        GameObject blockHit = null;
        var ray = _camera.ScreenPointToRay(position);
        if (RaycastNewBlockPos(ray, blockHit, ref position, out var newGo)) return;
        if (SpecificBlockActions(position, newGo)) return;
        StoreBlockData(position);
    }

    private void StoreBlockData(Vector3 position)
    {
        if (selectedPrefabIndex == (int)Enums.blockType.directionBlock) return;
        //if the prefab is in the twoByTwo list, add one block to the right and one block to the bottom and one block to the bottom right
        if (twoByTwoPrefabIndex.Contains(selectedPrefabIndex))
        {
            for (int i = 0; i < 3; i++)
            {
                if (blockGrid[(int)position.x + i % 2, (int)position.y, (int)position.z + i / 2] != 0) continue;
                blockGrid[(int)position.x + i % 2, (int)position.y, (int)position.z + i / 2] = 186;
                if (blockHorizontalRotationGrid[(int)position.x + i % 2, (int)position.y, (int)position.z + i / 2] ==
                    Enums.Side.none)
                    blockHorizontalRotationGrid[(int)position.x + i % 2, (int)position.y, (int)position.z + i / 2] =
                        Enums.Side.forward;
                if (blockVerticalRotationGrid[(int)position.x + i % 2, (int)position.y, (int)position.z + i / 2] ==
                    Enums.Side.none)
                    blockVerticalRotationGrid[(int)position.x + i % 2, (int)position.y, (int)position.z + i / 2] =
                        Enums.Side.forward;
            }
        }

        if (twoByOnePrefabIndex.Contains(selectedPrefabIndex))
        {
            if (blockGrid[(int)position.x + 1, (int)position.y, (int)position.z] == 0)
            {
                blockGrid[(int)position.x + 1, (int)position.y, (int)position.z] = 186;
                if (blockHorizontalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] ==
                    Enums.Side.none)
                    blockHorizontalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] =
                        Enums.Side.forward;
                if (blockVerticalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] == Enums.Side.none)
                    blockVerticalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] =
                        Enums.Side.forward;
            }
        }

        if (threeByOnePrefabIndex.Contains(selectedPrefabIndex))
        {
            for (int i = 1; i < 3; i++)
            {
                if (blockGrid[(int)position.x + i, (int)position.y, (int)position.z] != 0) continue;
                blockGrid[(int)position.x + i, (int)position.y, (int)position.z] = 186;
                if (blockHorizontalRotationGrid[(int)position.x + i, (int)position.y, (int)position.z] ==
                    Enums.Side.none)
                    blockHorizontalRotationGrid[(int)position.x + i, (int)position.y, (int)position.z] =
                        Enums.Side.forward;
                if (blockVerticalRotationGrid[(int)position.x + i, (int)position.y, (int)position.z] ==
                    Enums.Side.none)
                    blockVerticalRotationGrid[(int)position.x + i, (int)position.y, (int)position.z] =
                        Enums.Side.forward;
            }
        }

        if (twoPlusOnePrefabIndex.Contains(selectedPrefabIndex))
        {
            if (blockGrid[(int)position.x + 1, (int)position.y, (int)position.z] == 0)
            {
                blockGrid[(int)position.x + 1, (int)position.y, (int)position.z] = 186;
                if (blockHorizontalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] ==
                    Enums.Side.none)
                    blockHorizontalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] =
                        Enums.Side.forward;
                if (blockVerticalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] == Enums.Side.none)
                    blockVerticalRotationGrid[(int)position.x + 1, (int)position.y, (int)position.z] =
                        Enums.Side.forward;
            }

            if (blockGrid[(int)position.x, (int)position.y, (int)position.z - 1] == 0)
            {
                blockGrid[(int)position.x, (int)position.y, (int)position.z - 1] = 186;
                if (blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z - 1] ==
                    Enums.Side.none)
                    blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z - 1] =
                        Enums.Side.forward;
                if (blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z - 1] == Enums.Side.none)
                    blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z - 1] =
                        Enums.Side.forward;
            }
        }

        var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
        if (!blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Add(blockPlacedAddress);
        blockGrid[(int)position.x, (int)position.y, (int)position.z] = selectedPrefabIndex;
        if (blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] == Enums.Side.none)
            blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
        if (blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] == Enums.Side.none)
            blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
    }

    public void PlaceGrass()
    {
        selectedPrefabIndex = season switch
        {
            0 => (int)Enums.blockType.M3_Block1,
            1 => (int)Enums.blockType.M2_Block1,
            2 => (int)Enums.blockType.M1_Block1,
            _ => selectedPrefabIndex
        };
    }

    public void PlaceCaillou()
    {
        selectedPrefabIndex = season switch
        {
            0 => (int)Enums.blockType.M3_Caillou,
            1 => (int)Enums.blockType.M2_Caillou,
            2 => (int)Enums.blockType.M1_Caillou,
            _ => selectedPrefabIndex
        };
    }

    private bool SpecificBlockActions(Vector3 position, GameObject newGo)
    {
        int randomRotation;
        switch (selectedPrefabIndex)
        {
            case (int)Enums.blockType.ground or (int)Enums.blockType.M1_Block1 or (int)Enums.blockType.M2_Block1
                or (int)Enums.blockType.M3_Block1 or (int)Enums.blockType.M1_Caillou or (int)Enums.blockType.M2_Caillou
                or (int)Enums.blockType.M3_Caillou:
                // random rotation
                randomRotation = Random.Range(0, 4);
                newGo.transform.Rotate(0, randomRotation * 90, 0);
                blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = randomRotation switch
                {
                    0 => Enums.Side.forward,
                    1 => Enums.Side.right,
                    2 => Enums.Side.back,
                    3 => Enums.Side.left,
                    _ => blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z]
                };
                randomRotation = Random.Range(0, 4);
                newGo.transform.Rotate(0, 0, randomRotation * 90);
                blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = randomRotation switch
                {
                    0 => Enums.Side.forward,
                    1 => Enums.Side.right,
                    2 => Enums.Side.back,
                    3 => Enums.Side.left,
                    _ => blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z]
                };
                newGo.name = "Block" + position.x + position.y + position.z;
                break;
            case (int)Enums.blockType.directionBlock:
                directionGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
                newGo.name = "directionBlock";
                break;
            case 13:
                var rotation = 0;
                if (blockGrid[(int)position.x, (int)position.y - 1, (int)position.z] == 0 ||
                    position.x % (tileSize.x + tailleBridge) != 0 && position.x % (tileSize.x + tailleBridge) != 5 &&
                    position.z % (tileSize.y + tailleBridge) != 0 && position.z % (tileSize.y + tailleBridge) != 11)
                {
                    Debug.Log("Bridge can't be placed here, position.x % (tileSize.x + tailleBridge)" +
                              position.x % (tileSize.x + tailleBridge) + " position.z % (tileSize.y + tailleBridge)" +
                              position.z % (tileSize.y + tailleBridge));
                    Object.Destroy(newGo);
                    return true;
                }

                GameObject newground = null;
                var posOfnewPanelStart = new Vector3();
                var sideToInstantiateNewGrid = Enums.Side.none;
                for (var i = 1; i <= tailleBridge; i++)
                    newground = SwitchBridgePlacement(position, newground, i, ref posOfnewPanelStart,
                        ref sideToInstantiateNewGrid, ref rotation);

                var posOfNewPanelEnd = PlacePanelEnd(newGo, newground, posOfnewPanelStart, rotation);

                //offset for the position of the MakePlatform function
                var offset = sideToInstantiateNewGrid switch
                {
                    Enums.Side.left => new Vector2(-tileSize.x + 1 + posOfnewPanelStart.x,
                        position.z - position.z % (tileSize.y + tailleBridge)),
                    Enums.Side.right => new Vector2(posOfnewPanelStart.x,
                        position.z - position.z % (tileSize.y + tailleBridge)),
                    Enums.Side.back => new Vector2(position.x - position.x % (tileSize.x + tailleBridge),
                        -tileSize.y + posOfnewPanelStart.z + 1),
                    Enums.Side.forward => new Vector2(position.x - position.x % (tileSize.x + tailleBridge),
                        posOfnewPanelStart.z),
                    _ => new Vector2()
                };
                if (offset.x < 0 || offset.y < 0 || offset.x >= blockGrid.GetLength(0) ||
                    offset.y >= blockGrid.GetLength(2)) return true;
                if (blockGrid[(int)offset.x, 0, (int)offset.y] == 1) break;
                //check if there is already a platform
                if (blockGrid[(int)offset.x, 0, (int)offset.y] != 0)
                {
                    DeleteWalls(newGo, posOfNewPanelEnd);
                    return true;
                }

                MakePlatform((int)offset.x, (int)offset.y);
                DeleteWalls(newGo, posOfNewPanelEnd);
                break;
        }

        return false;
    }

    private void DeleteWalls(GameObject newGo, GameObject posOfnewPanel)
    {
        var posOfnewPanelStart = posOfnewPanel.transform.position;
        var rotation = (int)posOfnewPanel.transform.rotation.eulerAngles.y;
        switch (rotation)
        {
            case 0:
                blockGrid[(int)posOfnewPanelStart.x, (int)posOfnewPanelStart.y, (int)posOfnewPanelStart.z - 1] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewPanelStart.x, posOfnewPanelStart.y,
                    posOfnewPanelStart.z - 1)));
                break;
            case 90:
                blockGrid[(int)posOfnewPanelStart.x - 1, (int)posOfnewPanelStart.y, (int)posOfnewPanelStart.z] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewPanelStart.x - 1, posOfnewPanelStart.y,
                    posOfnewPanelStart.z)));
                break;
            case 180:
                blockGrid[(int)posOfnewPanelStart.x, (int)posOfnewPanelStart.y, (int)posOfnewPanelStart.z + 1] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewPanelStart.x, posOfnewPanelStart.y,
                    posOfnewPanelStart.z + 1)));
                break;
            case 270:
                blockGrid[(int)posOfnewPanelStart.x + 1, (int)posOfnewPanelStart.y, (int)posOfnewPanelStart.z] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewPanelStart.x + 1, posOfnewPanelStart.y,
                    posOfnewPanelStart.z)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var posOfnewGo = newGo.transform.position;
        var rotationGo = (int)newGo.transform.rotation.eulerAngles.y;
        switch (rotationGo)
        {
            case 0:
                blockGrid[(int)posOfnewGo.x, (int)posOfnewGo.y, (int)posOfnewGo.z + 1] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewGo.x, posOfnewGo.y, posOfnewGo.z + 1)));
                break;
            case 90:
                blockGrid[(int)posOfnewGo.x + 1, (int)posOfnewGo.y, (int)posOfnewGo.z] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewGo.x + 1, posOfnewGo.y, posOfnewGo.z)));
                break;
            case 180:
                blockGrid[(int)posOfnewGo.x, (int)posOfnewGo.y, (int)posOfnewGo.z - 1] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewGo.x, posOfnewGo.y, posOfnewGo.z - 1)));
                break;
            case 270:
                blockGrid[(int)posOfnewGo.x - 1, (int)posOfnewGo.y, (int)posOfnewGo.z] = 0;
                Object.Destroy(FindBlockAtPosition(new Vector3(posOfnewGo.x - 1, posOfnewGo.y, posOfnewGo.z)));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private GameObject PlacePanelEnd(GameObject newGo, GameObject newground, Vector3 posOfnewPanelStart, int rotation)
    {
        if (newground == null) return null;
        var newPanelStart = Object.Instantiate(prefabs[13], posOfnewPanelStart, Quaternion.identity);
        // blockGrid[(int)posOfnewPanelStart.x, (int)posOfnewPanelStart.y, (int)posOfnewPanelStart.z] = 13;
        newPanelStart.transform.parent = parent.transform;
        newPanelStart.transform.Rotate(0, rotation, 0);
        var newGoGridPos = newGo.transform.position;
        blockHorizontalRotationGrid[(int)newGoGridPos.x, (int)newGoGridPos.y, (int)newGoGridPos.z] = rotation switch
        {
            0 => Enums.Side.forward,
            90 => Enums.Side.right,
            180 => Enums.Side.back,
            270 => Enums.Side.left,
            _ => blockHorizontalRotationGrid[(int)newGoGridPos.x, (int)newGoGridPos.y, (int)newGoGridPos.z]
        };
        newGo.transform.Rotate(0, rotation, 0);
        directionGrid[(int)newGoGridPos.x, (int)newGoGridPos.y, (int)newGoGridPos.z] = blockHorizontalRotationGrid
            [(int)newGoGridPos.x, (int)newGoGridPos.y, (int)newGoGridPos.z];
        blockHorizontalRotationGrid
            [(int)posOfnewPanelStart.x, (int)posOfnewPanelStart.y, (int)posOfnewPanelStart.z] = rotation switch
        {
            0 => Enums.Side.forward,
            90 => Enums.Side.right,
            180 => Enums.Side.back,
            270 => Enums.Side.left,
            _ => blockHorizontalRotationGrid[(int)posOfnewPanelStart.x, (int)posOfnewPanelStart.y,
                (int)posOfnewPanelStart.z]
        };
        return newPanelStart;
    }

    private Object FindBlockAtPosition(Vector3 vector3)
    {
        //find in parent the block at position
        var transform = parent.transform.Find("Block (" + vector3.x + ", " + 1 + ", " + vector3.z + ")");
        if (transform != null)
        {
            return transform.gameObject;
        }

        return null;
    }

    private GameObject SwitchBridgePlacement(Vector3 position, GameObject newground, int i,
        ref Vector3 posOfnewPanelStart,
        ref Enums.Side sideToInstantiateNewGrid, ref int rotation)
    {
        switch (position.x % (tileSize.x + tailleBridge))
        {
            //left x = 0, right = 5, back = 0, forward = 11
            case 0:
                newground = Object.Instantiate(prefabs[season switch
                    {
                        0 => (int)Enums.blockType.M3_Block1,
                        1 => (int)Enums.blockType.M2_Block1,
                        2 => (int)Enums.blockType.M1_Block1,
                    }], new Vector3(position.x - i, 0, position.z),
                    Quaternion.identity);
                posOfnewPanelStart = new Vector3(position.x - i - 1, 1, position.z);
                sideToInstantiateNewGrid = Enums.Side.left;
                blockGrid[(int)position.x - i, (int)position.y - 1, (int)position.z] = 1;
                rotation = 270;
                directionGrid[(int)position.x - i, (int)position.y - 1, (int)position.z] = Enums.Side.left;
                newground.transform.name = "Block" + (position.x - i) + 0 + position.z;
                break;
            case 5:
                newground = Object.Instantiate(prefabs[season switch
                    {
                        0 => (int)Enums.blockType.M3_Block1,
                        1 => (int)Enums.blockType.M2_Block1,
                        2 => (int)Enums.blockType.M1_Block1,
                    }], new Vector3(position.x + i, 0, position.z),
                    Quaternion.identity);
                posOfnewPanelStart = new Vector3(position.x + i + 1, 1, position.z);
                sideToInstantiateNewGrid = Enums.Side.right;
                blockGrid[(int)position.x + i, (int)position.y - 1, (int)position.z] = 1;
                rotation = 90;
                directionGrid[(int)position.x + i, (int)position.y - 1, (int)position.z] = Enums.Side.right;
                newground.transform.name = "Block" + (position.x + i) + 0 + position.z;
                break;
            default:
                switch (position.z % (tileSize.y + tailleBridge))
                {
                    case 0:
                        newground = Object.Instantiate(prefabs[season switch
                            {
                                0 => (int)Enums.blockType.M3_Block1,
                                1 => (int)Enums.blockType.M2_Block1,
                                2 => (int)Enums.blockType.M1_Block1,
                            }], new Vector3(position.x, 0, position.z - i),
                            Quaternion.identity);
                        posOfnewPanelStart = new Vector3(position.x, 1, position.z - i - 1);
                        sideToInstantiateNewGrid = Enums.Side.back;
                        blockGrid[(int)position.x, (int)position.y - 1, (int)position.z - i] = 1;
                        rotation = 180;
                        directionGrid[(int)position.x, (int)position.y - 1, (int)position.z - i] = Enums.Side.back;
                        newground.transform.name = "Block" + position.x + 0 + (position.z - i);
                        break;
                    case 11:
                        newground = Object.Instantiate(prefabs[season switch
                            {
                                0 => (int)Enums.blockType.M3_Block1,
                                1 => (int)Enums.blockType.M2_Block1,
                                2 => (int)Enums.blockType.M1_Block1,
                            }], new Vector3(position.x, 0, position.z + i),
                            Quaternion.identity);
                        posOfnewPanelStart = new Vector3(position.x, 1, position.z + i + 1);
                        sideToInstantiateNewGrid = Enums.Side.forward;
                        blockGrid[(int)position.x, (int)position.y - 1, (int)position.z + i] = 1;
                        rotation = 0;
                        directionGrid[(int)position.x, (int)position.y - 1, (int)position.z + i] =
                            Enums.Side.forward;
                        newground.transform.name = "Block" + position.x + 0 + (position.z + i);
                        break;
                }

                break;
        }

        return newground;
    }

    private bool RaycastNewBlockPos(Ray ray, GameObject blockHit, ref Vector3 position, out GameObject newGo)
    {
        RaycastHit hitRay;
        if (Physics.Raycast(ray, out hitRay))
        {
            position = hitRay.normal switch
            {
                var up when up == Vector3.up => hitRay.point + new Vector3(0, 0.5f, 0),
                var down when down == Vector3.down => hitRay.point + new Vector3(0, -0.5f, 0),
                var left when left == Vector3.left => hitRay.point + new Vector3(-0.5f, 0, 0),
                var right when right == Vector3.right => hitRay.point + new Vector3(0.5f, 0, 0),
                var forward when forward == Vector3.forward => hitRay.point + new Vector3(0, 0, 0.5f),
                var back when back == Vector3.back => hitRay.point + new Vector3(0, 0, -0.5f),
                _ => position
            };
            blockHit = hitRay.collider.gameObject;
        }
        else
        {
            position = _camera.ScreenToWorldPoint(new Vector3(position.x, position.y, 10));
        }

        position.x = Mathf.Round(position.x / sizeOfGridSpace) * sizeOfGridSpace;
        position.y = Mathf.Round(position.y / sizeOfGridSpace) * sizeOfGridSpace;
        position.z = Mathf.Round(position.z / sizeOfGridSpace) * sizeOfGridSpace;

        if (blockHit == null || position.x < blockHit.transform.position.x ||
            position.x >= blockHit.transform.position.x + size.x ||
            position.y < blockHit.transform.position.y || position.y >= blockHit.transform.position.y + size.y ||
            position.z < blockHit.transform.position.z || position.z >= blockHit.transform.position.z + size.z)
        {
            newGo = null;
            return true;
        }

        if (position.y >= size.y || position.y < 0)
        {
            newGo = null;
            return true;
        }

        newGo = Object.Instantiate(selectedPrefab, position, Quaternion.identity);
        newGo.transform.parent = parent.transform;
        return false;
    }

    private void DragToChangeDirectionGrid()
    {
        ChangeCameraAngle();
        ToggleLevelElements();
        if (_camera.orthographic == false) return;
        // TODO CONNECT DOTS
    }

    private void Delete()
    {
        if (IsPointerOverUIObject() ||
            Input.GetTouch(0).phase != TouchPhase.Began ||
            EventSystem.current.currentSelectedGameObject) return;
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        var ray = _camera.ScreenPointToRay(position);
        if (!Physics.Raycast(ray, out hitRay)) return;
        switch (hitRay.transform.gameObject.transform.name)
        {
            case "Plane":
                return;
            case "directionBlock":
            {
                var position2 = hitRay.transform.position;
                Debug.Log("Direction before:" + directionGrid[(int)position2.x, (int)position2.y,
                    (int)position2.z]);
                directionGrid[(int)position2.x, (int)position2.y,
                    (int)position2.z] = Enums.Side.none;
                Debug.Log("Deleted direction at:" + position2); 
                Object.Destroy(hitRay.transform.gameObject);
                return;
            }
        }

        var hitRayPosition = hitRay.transform.position;

        if (twoByOnePrefabIndex.Contains(blockGrid[(int)hitRayPosition.x,
                (int)hitRayPosition.y,
                (int)hitRayPosition.z]))
        {
            switch (blockHorizontalRotationGrid[(int)hitRayPosition.x,
                        (int)hitRayPosition.y,
                        (int)hitRayPosition.z])
            {
                case Enums.Side.forward:
                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    break;
                case Enums.Side.right:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    break;
                case Enums.Side.back:
                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    break;
                case Enums.Side.left:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    break;
                case Enums.Side.none:
                    break;
                case Enums.Side.up:
                    break;
                case Enums.Side.down:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (twoByTwoPrefabIndex.Contains(blockGrid[(int)hitRayPosition.x,
                (int)hitRayPosition.y,
                (int)hitRayPosition.z]))
        {
            switch (blockHorizontalRotationGrid[(int)hitRayPosition.x,
                        (int)hitRayPosition.y,
                        (int)hitRayPosition.z])
            {
                case Enums.Side.forward:
                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    break;
                case Enums.Side.right:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    break;
                case Enums.Side.back:
                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    break;
                case Enums.Side.left:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    break;
                case Enums.Side.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (threeByOnePrefabIndex.Contains(blockGrid[(int)hitRayPosition.x,
                (int)hitRayPosition.y,
                (int)hitRayPosition.z]))
        {
            switch (blockHorizontalRotationGrid[(int)hitRayPosition.x,
                        (int)hitRayPosition.y,
                        (int)hitRayPosition.z])
            {
                case Enums.Side.forward:
                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 2, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 2, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 2, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    break;
                case Enums.Side.right:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 2] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 2] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 2] = Enums.Side.none;

                    break;
                case Enums.Side.back:
                    blockGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x - 2, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x - 2, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x - 2, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    break;
                case Enums.Side.left:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y, (int)hitRayPosition.z + 2] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 2] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 2] = Enums.Side.none;
                    break;
                case Enums.Side.none:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (twoPlusOnePrefabIndex.Contains(blockGrid[(int)hitRayPosition.x,
                (int)hitRayPosition.y,
                (int)hitRayPosition.z]))
        {
            switch (blockHorizontalRotationGrid[(int)hitRayPosition.x,
                        (int)hitRayPosition.y,
                        (int)hitRayPosition.z])
            {
                case Enums.Side.forward:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;

                    break;
                case Enums.Side.right:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z - 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y, (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    break;
                case Enums.Side.back:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x - 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    break;
                case Enums.Side.left:
                    blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                        (int)hitRayPosition.z + 1] = Enums.Side.none;

                    blockGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = 0;
                    blockHorizontalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    blockVerticalRotationGrid[(int)hitRayPosition.x + 1, (int)hitRayPosition.y,
                        (int)hitRayPosition.z] = Enums.Side.none;
                    break;
            }
        }

        if (blockGrid[(int)hitRayPosition.x, (int)hitRayPosition.y,
                (int)hitRayPosition.z] == 0)
        {
            var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
            if (blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Remove(blockPlacedAddress);
        }

        var position1 = hitRay.transform.position;
        blockGrid[(int)position1.x, (int)position1.y, (int)position1.z] = 0;
        blockHorizontalRotationGrid[(int)position1.x, (int)position1.y, (int)position1.z] = Enums.Side.none;
        blockVerticalRotationGrid[(int)position1.x, (int)position1.y, (int)position1.z] = Enums.Side.none;
        Object.Destroy(hitRay.transform.gameObject);
    }

    public void SaveData(string name)
    {
        var blockHorizontalRotationGridIntArray = Enums.SideToIntArray(blockHorizontalRotationGrid);
        var blockVerticalRotationGridIntArray = Enums.SideToIntArray(blockVerticalRotationGrid);
        var playerDirGridVector3Array = Enums.SideToVector3Array(directionGrid);
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGridIntArray,
            blockVerticalRotationGridIntArray, playerDirGridVector3Array, season);
        curentLevelData = data;
    }

    public LevelData GetData()
    {
        var blockHorizontalRotationGridIntArray = Enums.SideToIntArray(blockHorizontalRotationGrid);
        var blockVerticalRotationGridIntArray = Enums.SideToIntArray(blockVerticalRotationGrid);
        var playerDirGridVector3Array = Enums.SideToVector3Array(directionGrid);
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGridIntArray,
            blockVerticalRotationGridIntArray, playerDirGridVector3Array, season);
        curentLevelData = data;
        return data;
    }

    public LevelData TestLevel()
    {
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), Enums.SideToIntArray(blockHorizontalRotationGrid),
            Enums.SideToIntArray(blockVerticalRotationGrid), Enums.SideToVector3Array(directionGrid), season);
        curentLevelData = data;
        return data;
    }

    private bool hasStartAndEnd()
    {
        return blocksUsed.Count(x => x == Blocks.BlockType[Enums.blockType.playerStart]) == 1 &&
               blocksUsed.Count(x => x == Blocks.BlockType[Enums.blockType.playerEnd]) == 1;
    }

    public void LoadData(LevelData dataToLoad)
    {
        season = dataToLoad.season;
        Start();
        CleanScene();
        blocksUsed = new List<string>(dataToLoad.blocksUsed);
        blockGrid = dataToLoad.blockGrid;
        directionGrid = Enums.IntToSideArray(dataToLoad.playerDir);
        blockHorizontalRotationGrid = Enums.IntToSideArray(dataToLoad.blockHorizontalRotationGrid);
        blockVerticalRotationGrid = Enums.IntToSideArray(dataToLoad.blockVerticalRotationGrid);
        for (var z = 0; z < blockGrid.GetLength(2); z++)
        for (var y = 0; y < blockGrid.GetLength(1); y++)
        for (var x = 0; x < blockGrid.GetLength(0); x++)
        {
            if (directionGrid[x, y, z] != Enums.Side.none)
            {
                Debug.Log("Direction block instantiated" + x + y + z);
                InstantiateDirectionPrefab(x, y, z);
                Debug.Log("Instantiated direction block at:" + x + y + z);
            }

            switch (blockGrid[x, y, z])
            {
                case 0:
                    continue;
                default:
                    InstantiateBlock(x, y, z);
                    break;
            }
        }
    }

    private void InstantiateDirectionPrefab(int i, int i1, int i2)
    {
        var block = Object.Instantiate(prefabs[11], new Vector3(i, i1, i2), Quaternion.identity);
        switch (directionGrid[i, i1, i2])
        {
            case Enums.Side.right:
                block.transform.Rotate(0, 90, 0);
                break;
            case Enums.Side.left:
                block.transform.Rotate(0, -90, 0);
                break;
            case Enums.Side.forward:
                break;
            case Enums.Side.back:
                block.transform.Rotate(0, 180, 0);
                break;
            case Enums.Side.none:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        block.name = "directionBlock";
    }

    private void InstantiateBlock(int x, int y, int z)
    {
        var prefabIndex = blockGrid[x, y, z];
        var block = Object.Instantiate(prefabs[prefabIndex], new Vector3(x, y, z),
            Quaternion.identity);
        block.transform.parent = parent.transform;
        switch (prefabIndex)
        {
            case (int)Enums.blockType.ground or (int)Enums.blockType.M1_Block1 or (int)Enums.blockType.M2_Block1
                or (int)Enums.blockType.M3_Block1 or (int)Enums.blockType.M1_Caillou or (int)Enums.blockType.M2_Block1
                or (int)Enums.blockType.M3_Block1:
            {
                var randomRotation = Random.Range(0, 4);
                blockHorizontalRotationGrid[x, y, z] = randomRotation switch
                {
                    0 => Enums.Side.forward,
                    1 => Enums.Side.right,
                    2 => Enums.Side.back,
                    3 => Enums.Side.left,
                    _ => blockHorizontalRotationGrid[x, y, z]
                };
                randomRotation = Random.Range(0, 4);
                block.transform.Rotate(0, 0, randomRotation * 90);
                blockVerticalRotationGrid[x, y, z] = randomRotation switch
                {
                    0 => Enums.Side.forward,
                    1 => Enums.Side.up,
                    2 => Enums.Side.back,
                    3 => Enums.Side.down,
                    _ => blockVerticalRotationGrid[x, y, z]
                };
                break;
            }
            case > 97 and <= 186:
                switch (prefabIndex)
                {
                    //left bottom corner
                    case (int)Enums.blockType.InsideBottomLeftCorner:
                        block.transform.Rotate(-90, -90, 0);
                        block.transform.position += new Vector3(.5f, -0.5f, .5f);
                        break;
                    //right bottom corner
                    case (int)Enums.blockType.InsideBottomRightCorner:
                        block.transform.Rotate(-90, 0, -90);
                        block.transform.position += new Vector3(-.5f, -0.5f, .5f);
                        break;
                    //left top corner
                    case (int)Enums.blockType.InsideTopLeftCorner:
                        block.transform.Rotate(-90, 0, -90);
                        block.transform.position += new Vector3(.5f, -0.5f, -.5f);
                        break;
                    //right top corner
                    case (int)Enums.blockType.InsideTopRightCorner:
                        block.transform.Rotate(-90, 0, -90);
                        block.transform.position += new Vector3(-.5f, -0.5f, -.5f);
                        break;
                    //left side
                    case >= (int)Enums.blockType.InsideLeft1 and <= (int)Enums.blockType.InsideLeft12:
                        block.transform.Rotate(-90, 180, 0);
                        block.transform.position += new Vector3(.5f, -0.5f, -.5f);
                        break;
                    //right side
                    case >= (int)Enums.blockType.InsideRight1 and <= (int)Enums.blockType.InsideRight12:
                        block.transform.Rotate(-90, 0, 0);
                        block.transform.position += new Vector3(-.5f, -0.5f, .5f);
                        break;
                    //top side
                    case >= (int)Enums.blockType.InsideTop1 and <= (int)Enums.blockType.InsideTop6:
                        block.transform.Rotate(-90, -90, 0);
                        block.transform.position += new Vector3(-.5f, -0.5f, -.5f);
                        break;
                    //bottom side
                    case >= (int)Enums.blockType.InsideBottom1 and <= (int)Enums.blockType.InsideBottom6:
                        block.transform.Rotate(-90, 0, 90);
                        block.transform.position += new Vector3(.5f, -0.5f, .5f);
                        break;
                }

                break;
            default:
                switch (blockHorizontalRotationGrid[x, y, z])
                {
                    case Enums.Side.right:
                        block.transform.Rotate(0, 90, 0);
                        break;
                    case Enums.Side.left:
                        block.transform.Rotate(0, -90, 0);
                        break;
                    case Enums.Side.back:
                        block.transform.Rotate(0, 180, 0);
                        break;
                    default:
                        break;
                }

                switch (blockVerticalRotationGrid[x, y, z])
                {
                    case Enums.Side.back:
                        block.transform.Rotate(180, 0, 0);
                        break;
                    case Enums.Side.up:
                        block.transform.Rotate(90, 0, 0);
                        break;
                    case Enums.Side.down:
                        block.transform.Rotate(-90, 0, 0);
                        break;
                    default:
                        break;
                }

                break;
        }

        block.name = "Block" + x + y + z;
        if (directionGrid[x, y, z] != Enums.Side.none)
        {
            var directionBlock = Object.Instantiate(prefabs[11], new Vector3(x, y, z), Quaternion.identity);
            directionBlock.transform.Rotate(Enums.SideVector3(directionGrid[x, y, z] - 1));
            directionBlock.transform.parent = parent.transform;
        }
    }

    public void CleanScene()
    {
        foreach (Transform child in parent.transform) Object.Destroy(child.gameObject);
    }

    public void ToggleLevelElements()
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.name == "M1_Block1(Clone)") continue;
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }

    public void ChangeCameraAngle()
    {
        if (_camera.orthographic)
        {
            _camera.orthographic = false;
            _camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y - 10f,
                _camera.transform.position.z - 10);
            _camera.transform.rotation = Quaternion.Euler(80, 0, 0);
        }
        else
        {
            _camera.orthographic = true;
            _camera.orthographicSize = 8;
            _camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y + 10f,
                _camera.transform.position.z + 10);
            _camera.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void VerticalRotation()
    {
        if (IsPointerOverUIObject()) return;
        if (Input.GetTouch(0).phase != TouchPhase.Began) return;
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        var ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            if (hitRay.transform.gameObject.transform.name == "Plane") return;
            hitRay.transform.Rotate(-90, 0, 0);
            if (RotateInDirectionGridVertical(hitRay)) return;

            RotateBlockVertical(hitRay);
        }
    }

    private void RotateBlockVertical(RaycastHit hitRay)
    {
        var blockPosition = hitRay.transform.position;
        blockVerticalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] =
            blockVerticalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] switch
            {
                Enums.Side.forward => Enums.Side.down,
                Enums.Side.up => Enums.Side.forward,
                Enums.Side.back => Enums.Side.up,
                Enums.Side.down => Enums.Side.back,
                _ => blockVerticalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z]
            };
    }

    private bool RotateInDirectionGridVertical(RaycastHit hitRay)
    {
        if (hitRay.transform.gameObject.transform.name != "directionBlock") return false;
        var position2 = hitRay.transform.position;
        directionGrid[(int)position2.x, (int)position2.y, (int)position2.z] =
            directionGrid[(int)position2.x, (int)position2.y, (int)position2.z] switch
            {
                Enums.Side.forward => Enums.Side.down,
                Enums.Side.up => Enums.Side.forward,
                Enums.Side.back => Enums.Side.up,
                Enums.Side.down => Enums.Side.back,
                _ => directionGrid[(int)position2.x, (int)position2.y, (int)position2.z]
            };
        hitRay.transform.Rotate(90, 0, 0);
        return true;
    }

    private void HorizontalRotation()
    {
        if (IsPointerOverUIObject()) return;
        if (Input.GetTouch(0).phase != TouchPhase.Began) return;
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        var ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            var hitPosition = hitRay.transform.position;
            if (hitRay.transform.gameObject.transform.name == "Plane") return;
            if (twoByOnePrefabIndex.Contains(blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z]))
            {
                switch (blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z])
                {
                    case Enums.Side.forward:
                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.right:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.back:
                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.left:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.none:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (twoByTwoPrefabIndex.Contains(blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z]))
            {
                switch (blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z])
                {
                    case Enums.Side.forward:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.right:
                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.back:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.left:
                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.none:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (threeByOnePrefabIndex.Contains(blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z]))
            {
                switch (blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z])
                {
                    case Enums.Side.forward:
                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockGrid[(int)hitPosition.x + 2, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x + 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.forward;
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 2] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 2] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 2] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.right:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 2] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 2] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 2] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockGrid[(int)hitPosition.x + 2, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x + 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.back:
                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockGrid[(int)hitPosition.x - 2, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x - 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.forward;
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 2] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 2] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 2] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.left:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 2] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 2] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 2] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockGrid[(int)hitPosition.x - 2, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        blockVerticalRotationGrid[(int)hitPosition.x - 2, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.forward;
                        break;
                    case Enums.Side.none:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (twoPlusOnePrefabIndex.Contains(blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z]))
            {
                switch (blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z])
                {
                    case Enums.Side.forward:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        break;
                    case Enums.Side.right:
                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        break;
                    case Enums.Side.back:
                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z + 1] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x, (int)hitPosition.y, (int)hitPosition.z - 1] =
                            Enums.Side.none;
                        break;
                    case Enums.Side.left:
                        blockGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] = 0;
                        blockHorizontalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x + 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;

                        blockGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] = 186;
                        blockHorizontalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        blockVerticalRotationGrid[(int)hitPosition.x - 1, (int)hitPosition.y, (int)hitPosition.z] =
                            Enums.Side.none;
                        break;
                    case Enums.Side.none:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            hitRay.transform.Rotate(0, -90, 0);
            RotateInDirectionGridHorizontal(hitRay);
            RotateBlockHorizontal(hitRay);
        }
    }

    private void RotateBlockHorizontal(RaycastHit hitRay)
    {
        var blockPosition = hitRay.transform.position;
        blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] =
            blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] switch
            {
                Enums.Side.forward => Enums.Side.left,
                Enums.Side.right => Enums.Side.forward,
                Enums.Side.back => Enums.Side.right,
                Enums.Side.left => Enums.Side.back,
                _ => blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z]
            };
        Debug.Log("Horizontal Rotation at " + blockPosition + " is " +
                  blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] +
                  " and direction is " +
                  directionGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z]);
    }

    private void RotateInDirectionGridHorizontal(RaycastHit hitRay)
    {
        if (hitRay.transform.gameObject.transform.name == "directionBlock")
        {
            var position1 = hitRay.transform.position;
            directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] =
                directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] switch
                {
                    Enums.Side.forward => Enums.Side.left,
                    Enums.Side.right => Enums.Side.forward,
                    Enums.Side.back => Enums.Side.right,
                    Enums.Side.left => Enums.Side.back,
                    _ => directionGrid[(int)position1.x, (int)position1.y, (int)position1.z]
                };
        }
    }

    public void ChangePrefab(int index)
    {
        selectedPrefabIndex = index;
    }

    public void SwitchMode(int index)
    {
        Mode = (EditorMode)index;
    }

    public void ChangeMoveCamera()
    {
        isMoveCamera = !isMoveCamera;
    }

    public void SwapSeason()
    {
        if (season == 2)
        {
            season = 0;
        }
        else
        {
            season++;
        }

        switch (season)
        {
            case 0:
                Autumn();
                break;
            case 1:
                Winter();
                break;
            case 2:
                Spring();
                break;
        }
    }

    private void Autumn()
    {
        volume.profile = volumeProfiles[0];

        for (int i = 0; i < blockGrid.GetLength(0); i++)
        {
            for (int j = 0; j < blockGrid.GetLength(1); j++)
            {
                for (int k = 0; k < blockGrid.GetLength(2); k++)
                {
                    if (blockGrid[i, j, k] == (int)Enums.blockType.M1_Block1 ||
                        blockGrid[i, j, k] == (int)Enums.blockType.ground)
                    {
                        blockGrid[i, j, k] = (int)Enums.blockType.M3_Block1;
                        ReplaceBlock(i, j, k);
                    }
                    else if (blockGrid[i, j, k] == (int)Enums.blockType.M1_Caillou)
                    {
                        blockGrid[i, j, k] = (int)Enums.blockType.M3_Caillou;
                        ReplaceBlock(i, j, k);
                    }
                }
            }
        }
    }

    private void Winter()
    {
        volume.profile = volumeProfiles[1];

        for (int i = 0; i < blockGrid.GetLength(0); i++)
        {
            for (int j = 0; j < blockGrid.GetLength(1); j++)
            {
                for (int k = 0; k < blockGrid.GetLength(2); k++)
                {
                    if (blockGrid[i, j, k] == (int)Enums.blockType.M3_Block1)
                    {
                        blockGrid[i, j, k] = (int)Enums.blockType.M2_Block1;
                        ReplaceBlock(i, j, k);
                    }
                    else if (blockGrid[i, j, k] == (int)Enums.blockType.M3_Caillou)
                    {
                        blockGrid[i, j, k] = (int)Enums.blockType.M2_Caillou;
                        ReplaceBlock(i, j, k);
                    }
                }
            }
        }
    }

    private void Spring()
    {
        volume.profile = volumeProfiles[2];

        for (int i = 0; i < blockGrid.GetLength(0); i++)
        {
            for (int j = 0; j < blockGrid.GetLength(1); j++)
            {
                for (int k = 0; k < blockGrid.GetLength(2); k++)
                {
                    if (blockGrid[i, j, k] == (int)Enums.blockType.M2_Block1)
                    {
                        blockGrid[i, j, k] = (int)Enums.blockType.M1_Block1;
                        ReplaceBlock(i, j, k);
                    }
                    else if (blockGrid[i, j, k] == (int)Enums.blockType.M2_Caillou)
                    {
                        blockGrid[i, j, k] = (int)Enums.blockType.M1_Caillou;
                        ReplaceBlock(i, j, k);
                    }
                }
            }
        }
    }

    private void ReplaceBlock(int i, int j, int k)
    {
        var block = GameObject.Find("Block" + i + j + k);
        Object.Destroy(block);
        var blockPrefab = Object.Instantiate(prefabs[blockGrid[i, j, k]], new Vector3(i, j, k),
            Quaternion.identity);
        blockPrefab.transform.Rotate(90 * Random.Range(0, 4),
            90 * Random.Range(0, 4), 90 * Random.Range(0, 4));
        blockPrefab.name = "Block" + i + j + k;
        blockPrefab.transform.parent = parent.transform;
    }
}