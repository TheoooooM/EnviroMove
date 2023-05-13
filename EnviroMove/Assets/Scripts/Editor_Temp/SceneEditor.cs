using System.Collections.Generic;
using System.Linq;
using Archi.Service.Interface;
using Attributes;
using Levels;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;
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

    private int sizeOfGridSpace = 1;

    private Camera _camera;

    private GameObject parent;

    private string path;
    private TMP_InputField inputField;

    public bool isMoveCamera = true;

    //LevelData
    public Vector3Int size;
    public Vector3Int defaultSize = new(80, 2, 80);
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
    private int tailleBridge = 4;

    private enum EditorMode
    {
        create,
        delete,
        moveCamera,
        snapMode,
        horizontalRotation,
        verticalRotation,
    }

    private EditorMode Mode = EditorMode.create;

    private LevelData curentLevelData;
    
    #endregion

    public void Start()
    {
        size = defaultSize;
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
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y - 1; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    directionGrid[x, y, z] = Enums.Side.none;
                }
            }
        }
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
        for (int x = posX; x < posX + tileSize.x; x++)
        {
            for (int z = posZ; z < posZ + tileSize.y; z++)
            {
                block = Object.Instantiate(prefabs[(int)Enums.blockType.M1_Block1], new Vector3(x, 0, z),
                    Quaternion.identity);
                block.transform.SetParent(parent.transform);
                blockGrid[x, 0, z] = (int)Enums.blockType.M1_Block1;
                blockHorizontalRotationGrid[x, 0, z] = Enums.Side.none;
                blockVerticalRotationGrid[x, 0, z] = Enums.Side.none;
            }
        }

        firstBlockPosition = new Vector2Int(posX, posZ);
        while (firstBlockPosition.x > 0) firstBlockPosition.x -= tileSize.x;
        while (firstBlockPosition.y > 0) firstBlockPosition.y -= tileSize.y;
        firstBlockPosition.x += tileSize.x;
        firstBlockPosition.y += tileSize.y;
        
        block = Object.Instantiate(prefabs[(int)Enums.blockType.M1_Border], new Vector3(posX + tileSize.x / 2 - .5f, 0, posZ + tileSize.y / 2 - .5f),
            Quaternion.identity);
        block.transform.Rotate(0, 90, 0);
        block.transform.SetParent(parent.transform);
        block.name = (posX + tileSize.x / 2 - .5f) + " " + 0 + " " + (posZ + tileSize.y / 2 - .5f);
    }

    private void DestroyPlatform(int posX, int posZ)
    {
        for (int x = posX; x < posX + tileSize.x; x++)
        {
            for (int z = posZ; z < posZ + tileSize.y; z++)
            {
                Object.Destroy(parent.transform.Find(x + " " + 0 + " " + z).gameObject);
                blockGrid[x, 0, z] = 0;
                blockHorizontalRotationGrid[x, 0, z] = Enums.Side.none;
                blockVerticalRotationGrid[x, 0, z] = Enums.Side.none;
            }
        }
        
        firstBlockPosition = new Vector2Int(posX, posZ);
        while (firstBlockPosition.x > 0) firstBlockPosition.x -= tileSize.x;
        while (firstBlockPosition.y > 0) firstBlockPosition.y -= tileSize.y;
        firstBlockPosition.x += tileSize.x;
        firstBlockPosition.y += tileSize.y;
        
        // destroy border
        Object.Destroy(parent.transform.Find((posX + tileSize.x / 2 - .5f) + " " + 0 + " " + (posZ + tileSize.y / 2 - .5f)).gameObject);
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
        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            _camera.transform.position += new Vector3(-touchDeltaPosition.x, 0, -touchDeltaPosition.y) * Time.deltaTime;
        }
    }

    private void Create()
    {
        if (IsPointerOverUIObject()) return;
        if (Input.GetTouch(0).phase == TouchPhase.Began && isMoveCamera)
        {
            InstantiateNewBlock();
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Moved && !isMoveCamera)
        {
            InstantiateNewBlock();
        }
    }

    private bool IsPointerOverUIObject()
    {
        //if (EventSystem.current)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }

        return false;
    }

    private void InstantiateNewBlock()
    {
        Vector3 position = Input.GetTouch(0).position;
        GameObject blockHit = null;
        RaycastHit hitRay;
        Ray ray = _camera.ScreenPointToRay(position);
        if (RaycastNewBlockPos(ray, blockHit, ref position, out var newGo)) return;
        if (SpecificBlockActions(position, newGo)) return;
        StoreBlockData(position);
    }

    private void StoreBlockData(Vector3 position)
    {
        if (selectedPrefabIndex == 11) return;
        var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
        if (!blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Add(blockPlacedAddress);
        blockGrid[(int)position.x, (int)position.y, (int)position.z] = selectedPrefabIndex;
        blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
        blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
        Debug.Log("blockHorizontalRotationGrid[" + (int)position.x + ", " + (int)position.y + ", " + (int)position.z +
                  "] = " + blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z]);
    }

    private bool SpecificBlockActions(Vector3 position, GameObject newGo)
    {
        switch (selectedPrefabIndex)
        {
            case 6:
                Debug.Log("case 6 block below: " + blockGrid[(int)position.x, (int)position.y - 1, (int)position.z]);
                if (blockGrid[(int)position.x, (int)position.y - 1, (int)position.z] != 1)
                {
                    Object.Destroy(newGo);
                    return true;
                }

                break;
            case 11:
                directionGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
                newGo.name = "directionBlock";
                break;
            case 13:
                int rotation = 0;
                if (blockGrid[(int)position.x, (int)position.y - 1, (int)position.z] == 0 ||
                    position.x + 8 >= size.x && position.x % (tileSize.x + tailleBridge) == 5 ||
                    position.x - 8 < 0 && position.x % (tileSize.x + tailleBridge) == 0 ||
                    position.z + 8 >= size.z && position.x % (tileSize.y + tailleBridge) == 11 ||
                    position.z - 8 < 0 && position.x % (tileSize.y + tailleBridge) == 0)
                {
                    Object.Destroy(newGo);
                    return true;
                }

                GameObject newground = null;
                Vector3 posOfnewPanelStart = new Vector3();
                Enums.Side sideToInstantiateNewGrid = Enums.Side.none;
                for (int i = 1; i <= tailleBridge; i++)
                {
                    newground = SwitchBridgePlacement(position, newground, i, ref posOfnewPanelStart,
                        ref sideToInstantiateNewGrid, ref rotation);
                }

                PlacePanelEnd(newGo, newground, posOfnewPanelStart, rotation);

                //offset for the position of the MakePlatform function
                Vector2 offset = sideToInstantiateNewGrid switch
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
                if (blockGrid[(int)offset.x, 0, (int)offset.y] != 0) return true;
                MakePlatform((int)offset.x, (int)offset.y);
                break;
        }

        return false;
    }

    private void PlacePanelEnd(GameObject newGo, GameObject newground, Vector3 posOfnewPanelStart, int rotation)
    {
        if (newground != null)
        {
            var newPanelStart = Object.Instantiate(prefabs[13], posOfnewPanelStart, Quaternion.identity);
            newPanelStart.transform.parent = parent.transform;
            newPanelStart.transform.Rotate(0, rotation, 0);
            newGo.transform.Rotate(0, rotation, 0);
        }
    }

    private GameObject SwitchBridgePlacement(Vector3 position, GameObject newground, int i,
        ref Vector3 posOfnewPanelStart,
        ref Enums.Side sideToInstantiateNewGrid, ref int rotation)
    {
        switch (position.x % (tileSize.x + tailleBridge))
        {
            //left x = 0, right = 5, back = 0, forward = 15
            case 0:
                newground = Object.Instantiate(prefabs[1], new Vector3(position.x - i, 0, position.z),
                    Quaternion.identity);
                posOfnewPanelStart = new Vector3(position.x - i - 1, 1, position.z);
                sideToInstantiateNewGrid = Enums.Side.left;
                blockGrid[(int)position.x - i, (int)position.y, (int)position.z] = 1;
                rotation = 270;
                directionGrid[(int)position.x - i, (int)position.y, (int)position.z] = Enums.Side.left;
                break;
            case 5:
                newground = Object.Instantiate(prefabs[1], new Vector3(position.x + i, 0, position.z),
                    Quaternion.identity);
                posOfnewPanelStart = new Vector3(position.x + i + 1, 1, position.z);
                sideToInstantiateNewGrid = Enums.Side.right;
                blockGrid[(int)position.x + i, (int)position.y, (int)position.z] = 1;
                rotation = 90;
                directionGrid[(int)position.x + i, (int)position.y, (int)position.z] = Enums.Side.right;
                break;
            default:
                switch (position.z % (tileSize.y + tailleBridge))
                {
                    case 0:
                        newground = Object.Instantiate(prefabs[1], new Vector3(position.x, 0, position.z - i),
                            Quaternion.identity);
                        posOfnewPanelStart = new Vector3(position.x, 1, position.z - i - 1);
                        sideToInstantiateNewGrid = Enums.Side.back;
                        blockGrid[(int)position.x, (int)position.y, (int)position.z - i] = 1;
                        rotation = 180;
                        directionGrid[(int)position.x, (int)position.y, (int)position.z - i] = Enums.Side.back;
                        break;
                    case 11:
                        newground = Object.Instantiate(prefabs[1], new Vector3(position.x, 0, position.z + i),
                            Quaternion.identity);
                        posOfnewPanelStart = new Vector3(position.x, 1, position.z + i + 1);
                        sideToInstantiateNewGrid = Enums.Side.forward;
                        blockGrid[(int)position.x, (int)position.y, (int)position.z + i] = 1;
                        rotation = 0;
                        directionGrid[(int)position.x, (int)position.y, (int)position.z + i] =
                            Enums.Side.forward;
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
        Ray ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            if (hitRay.transform.gameObject.transform.name == "Plane") return;
            if (blockGrid[(int)hitRay.transform.position.x, (int)hitRay.transform.position.y,
                    (int)hitRay.transform.position.z] == 0)
            {
                var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
                if (blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Remove(blockPlacedAddress);
            }

            var position1 = hitRay.transform.position;
            blockGrid[(int)position1.x, (int)position1.y, (int)position1.z] = 0;
            blockHorizontalRotationGrid[(int)position1.x, (int)position1.y, (int)position1.z] = 0;
            blockVerticalRotationGrid[(int)position1.x, (int)position1.y, (int)position1.z] = 0;
            Object.Destroy(hitRay.transform.gameObject);
        }
    }

    public void SaveData(string name)
    {
        var blockHorizontalRotationGridIntArray = Enums.SideToIntArray(blockHorizontalRotationGrid);
        var blockVerticalRotationGridIntArray = Enums.SideToIntArray(blockVerticalRotationGrid);
        var playerDirGridVector3Array = Enums.SideToVector3Array(directionGrid);
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGridIntArray,
            blockVerticalRotationGridIntArray, playerDirGridVector3Array);
        curentLevelData = data;
    }

    public LevelData GetData()
    {
        var blockHorizontalRotationGridIntArray = Enums.SideToIntArray(blockHorizontalRotationGrid);
        var blockVerticalRotationGridIntArray = Enums.SideToIntArray(blockVerticalRotationGrid);
        var playerDirGridVector3Array = Enums.SideToVector3Array(directionGrid);
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGridIntArray,
            blockVerticalRotationGridIntArray, playerDirGridVector3Array);
        curentLevelData = data;
        return data;
    }

    public LevelData TestLevel()
    {
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), Enums.SideToIntArray(blockHorizontalRotationGrid),
            Enums.SideToIntArray(blockVerticalRotationGrid), Enums.SideToVector3Array(directionGrid));
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
        Start();
        CleanScene();
        blocksUsed = new List<string>(dataToLoad.blocksUsed);
        blockGrid = dataToLoad.blockGrid;
        directionGrid = Enums.IntToSideArray(dataToLoad.playerDir);
        blockHorizontalRotationGrid = Enums.IntToSideArray(dataToLoad.blockHorizontalRotationGrid);
        blockVerticalRotationGrid = Enums.IntToSideArray(dataToLoad.blockVerticalRotationGrid);
        for (int z = 0; z < blockGrid.GetLength(2); z++)
        {
            for (int y = 0; y < blockGrid.GetLength(1); y++)
            {
                for (int x = 0; x < blockGrid.GetLength(0); x++)
                {
                    if (blockGrid[x, y, z] == 0) continue;
                    InstantiateBlock(x, y, z);
                }
            }
        }
    }

    private void InstantiateBlock(int x, int y, int z)
    {
        int prefabIndex = blockGrid[x, y, z];
        var block = Object.Instantiate(prefabs[prefabIndex], new Vector3(x, y, z),
            Quaternion.identity);
        block.transform.Rotate(Enums.SideVector3(blockHorizontalRotationGrid[x, y, z]) * 90f);
        block.transform.Rotate(Enums.SideVector3(blockVerticalRotationGrid[x, y, z]) * 90f);
        block.transform.parent = parent.transform;
        if (directionGrid[x, y, z] != Enums.Side.none)
        {
            var directionBlock = Object.Instantiate(prefabs[11], new Vector3(x, y, z), Quaternion.identity);
            directionBlock.transform.Rotate(Enums.SideVector3(directionGrid[x, y, z]));
            directionBlock.transform.parent = parent.transform;
        }
    }

    public void CleanScene()
    {
        foreach (Transform child in parent.transform)
        {
            Object.Destroy(child.gameObject);
        }
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
        Ray ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            if (hitRay.transform.gameObject.transform.name == "Plane") return;
            hitRay.transform.Rotate(90, 0, 0);
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
                Enums.Side.forward => Enums.Side.up,
                Enums.Side.up => Enums.Side.back,
                Enums.Side.back => Enums.Side.down,
                Enums.Side.down => Enums.Side.forward,
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
                Enums.Side.forward => Enums.Side.up,
                Enums.Side.up => Enums.Side.back,
                Enums.Side.back => Enums.Side.down,
                Enums.Side.down => Enums.Side.forward,
                _ => directionGrid[(int)position2.x, (int)position2.y, (int)position2.z]
            };
        hitRay.transform.Rotate(0, 0, 90);
        return true;

    }

    private void HorizontalRotation()
    {
        if (IsPointerOverUIObject()) return;
        if (Input.GetTouch(0).phase != TouchPhase.Began) return;
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        Ray ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            if (hitRay.transform.gameObject.transform.name == "Plane") return;
            hitRay.transform.Rotate(0, 90, 0);
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
                Enums.Side.forward => Enums.Side.right,
                Enums.Side.right => Enums.Side.back,
                Enums.Side.back => Enums.Side.left,
                Enums.Side.left => Enums.Side.forward,
                _ => blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z]
            };
    }

    private void RotateInDirectionGridHorizontal(RaycastHit hitRay)
    {
        if (hitRay.transform.gameObject.transform.name == "directionBlock")
        {
            var position1 = hitRay.transform.position;
            directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] =
                directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] switch
                {
                    Enums.Side.forward => Enums.Side.right,
                    Enums.Side.right => Enums.Side.back,
                    Enums.Side.back => Enums.Side.left,
                    Enums.Side.left => Enums.Side.forward,
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
}