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
    public Vector3Int defaultSize = new(500, 4, 500);
    public Vector2Int tileSize = new(6,12);
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
        _camera.transform.position = new Vector3((tileSize.x + 3) * (size.x / (tileSize.x + 3) / 2 ), _camera.transform.position.y, (tileSize.y + 3) * (size.z / (tileSize.y + 3) / 2));
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
        var posX = (tileSize.x + 3) * (size.x / (tileSize.x + 3) / 2);
        var posZ = (tileSize.y + 3) * (size.z / (tileSize.y + 3) / 2);
        MakePlatform(posX, posZ);

        blocksUsed.Add("groundBlock");
    }

    private void MakePlatform(int posX, int posZ)
    {
        GameObject block = null;
        for (int x = posX; x < posX + tileSize.x; x++)
        {
            for (int z = posZ; z < posZ + tileSize.y; z++)
            {
                if (x == posX && z == posZ)
                {
                    block = Object.Instantiate(prefabs[5],new Vector3(x,0,z), Quaternion.identity);
                }
                else
                {
                    block = Object.Instantiate(prefabs[(int)Enums.blockType.ground], new Vector3(x, 0, z), Quaternion.identity);
                }
                block.transform.SetParent(parent.transform);
                blockGrid[x, 0, z] = (int)Enums.blockType.ground;
                blockHorizontalRotationGrid[x, 0, z] = Enums.Side.none;
                blockVerticalRotationGrid[x, 0, z] = Enums.Side.none;
            }
        }
        
        firstBlockPosition = new Vector2Int(posX, posZ);
        while (firstBlockPosition.x > 0) firstBlockPosition.x -= tileSize.x;
        while (firstBlockPosition.y > 0) firstBlockPosition.y -= tileSize.y;
        firstBlockPosition.x += tileSize.x;
        firstBlockPosition.y += tileSize.y;
    }

    public void Update()
    {
        if (_camera == null) _camera = Camera.main;
        if (parent == null) parent = new GameObject();
        // if (directionGrid != null)
        // {
        //     for (int x = 0; x < size.x; x++)
        //     {
        //         for (int y = 0; y < size.y - 1; y++)
        //         {
        //             for (int z = 0; z < size.z; z++)
        //             {
        //                 Debug.DrawRay(new Vector3(x, y, z), directionGrid[x, y, z], Color.red);
        //             }
        //         }
        //     }
        // }
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
        if (blockHit == null || position.x < blockHit.transform.position.x || position.x >= blockHit.transform.position.x + size.x || 
            position.y < blockHit.transform.position.y || position.y >= blockHit.transform.position.y + size.y || 
            position.z < blockHit.transform.position.z || position.z >= blockHit.transform.position.z + size.z) return;
        var newGo = Object.Instantiate(selectedPrefab, position, Quaternion.identity);
        newGo.transform.parent = parent.transform;


        switch (selectedPrefabIndex)
        {
            case 6:
                Debug.Log("case 6 block below: " + blockGrid[(int)position.x, (int)position.y - 1, (int)position.z]);
                if (blockGrid[(int)position.x, (int)position.y - 1, (int)position.z] != 1)
                {
                    Object.Destroy(newGo);
                    return;
                }

                break;
            case 11:
                directionGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
                newGo.name = "directionBlock";
                break;
            case 13:
                GameObject newground = null;
                Vector3 posOfnewPanelStart = new Vector3();
                Enums.Side sideToInstantiateNewGrid = Enums.Side.none;
                for (int i = 1; i <= 3; i++)
                {
                    Debug.Log($"position.x {position.x} % tileSize.x {tileSize.x + 3} : " + position.x % (tileSize.x  + 3) + $" position.z {position.z + 3} % tileSize.y {tileSize.y + 3}: " + position.z % (tileSize.y + 3));
                    switch (position.x % (tileSize.x + 3))
                    {
                        //left x = 0, right = 5, back = 0, forward = 15
                        case 0:
                            newground = Object.Instantiate(prefabs[1], new Vector3(position.x - i ,0, position.z), Quaternion.identity);
                            posOfnewPanelStart = new Vector3(position.x - i - 1, 1, position.z);
                            sideToInstantiateNewGrid = Enums.Side.left;
                            break;
                        case 5:
                            newground = Object.Instantiate(prefabs[1], new Vector3(position.x + i, 0, position.z), Quaternion.identity);
                            posOfnewPanelStart = new Vector3(position.x + i + 1, 1, position.z);
                            sideToInstantiateNewGrid = Enums.Side.right;
                            break;
                        default:
                            switch (position.z % (tileSize.y + 3))
                            {
                                case 0:
                                    newground = Object.Instantiate(prefabs[1], new Vector3(position.x, 0, position.z - i), Quaternion.identity);
                                    posOfnewPanelStart = new Vector3(position.x, 1, position.z - i - 1);
                                    sideToInstantiateNewGrid = Enums.Side.back;
                                    break;
                                case 11:
                                    newground = Object.Instantiate(prefabs[1], new Vector3(position.x, 0, position.z + i), Quaternion.identity);
                                    posOfnewPanelStart = new Vector3(position.x, 1, position.z + i + 1);
                                    sideToInstantiateNewGrid = Enums.Side.forward;
                                    break;
                            }

                            break;
                    }
                }
                Debug.Log("position.x - position.x % (tileSize.x - 3)" + (position.x - position.x % (tileSize.x - 3)));
                Debug.Log("sideToInstantiateNewGrid: " + sideToInstantiateNewGrid);
                if (newground != null)
                {
                    var newPanelStart = Object.Instantiate(prefabs[13], posOfnewPanelStart, Quaternion.identity);
                }
                //offset for the position of the MakePlatform function
                Vector2 offset = sideToInstantiateNewGrid switch
                {
                    Enums.Side.left => new Vector2(-tileSize.x + 1 + posOfnewPanelStart.x, position.z - position.z % (tileSize.y + 3)),
                    Enums.Side.right => new Vector2(posOfnewPanelStart.x, position.z - position.z % (tileSize.y + 3)),
                    Enums.Side.back => new Vector2(position.x - position.x % (tileSize.x + 3) , -tileSize.y + posOfnewPanelStart.z),
                    Enums.Side.forward => new Vector2(position.x - position.x % (tileSize.x + 3) , posOfnewPanelStart.z),
                    _ => new Vector2()
                };
                if (blockGrid[(int)offset.x, 0, (int)offset.y] == 1) { break; }
                MakePlatform((int)offset.x, (int)offset.y);
                break;
        }

        var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
        if (!blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Add(blockPlacedAddress);
        blockGrid[(int)position.x, (int)position.y, (int)position.z] = selectedPrefabIndex;
        blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
        blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = Enums.Side.forward;
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
        // if (!hasStartAndEnd())
        // {
        //     Debug.LogError("You need to have one player start and one player end");
        //     return;
        // }
        // var blockGridIntArray = TripleListToIntArray(blockGrid);
        var blockHorizontalRotationGridIntArray = Enums.SideToIntArray(blockHorizontalRotationGrid);
        var blockVerticalRotationGridIntArray = Enums.SideToIntArray(blockVerticalRotationGrid);
        var playerDirGridVector3Array = Enums.SideToVector3Array(directionGrid);
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGridIntArray,
            blockVerticalRotationGridIntArray, playerDirGridVector3Array);
        curentLevelData = data;
        // Debug.Log("data: " + (string)data);
    }

    public LevelData GetData()
    {
        Debug.Log("Starting conversions");
        var blockHorizontalRotationGridIntArray = Enums.SideToIntArray(blockHorizontalRotationGrid);
        Debug.Log("blockHorizontalRotationGridIntArray done");
        var blockVerticalRotationGridIntArray = Enums.SideToIntArray(blockVerticalRotationGrid);
        Debug.Log("blockVerticalRotationGridIntArray done");
        var playerDirGridVector3Array = Enums.SideToVector3Array(directionGrid);
        Debug.Log("Conversions done");
        Debug.Log("Starting new LevelData creation");
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGridIntArray,
            blockVerticalRotationGridIntArray, playerDirGridVector3Array);
        Debug.Log("LevelData created");
        curentLevelData = data;
        return data;
    }

    public LevelData TestLevel()
    {
        // if (!hasStartAndEnd()) return null;
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

    private int[,,] TripleListToIntArray(List<List<List<int>>> list)
    {
        var size = new int[list.Count, list[0].Count, list[0][0].Count];
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                for (int k = 0; k < list[i][j].Count; k++)
                {
                    size[i, j, k] = list[i][j][k];
                }
            }
        }

        return size;
    }

    public void LoadData(LevelData dataToLoad)
    {
        Start();
        CleanScene();
        Debug.Log((string)dataToLoad);
        blocksUsed = new List<string>(dataToLoad.blocksUsed);
        blocksUsed.ForEach(x => Debug.Log(x));
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
                    int prefabIndex = blockGrid[x, y, z];
                    var block = Object.Instantiate(prefabs[prefabIndex /*blockGrid[x, y, z]*/], new Vector3(x, y, z),
                        Quaternion.identity);
                    if (blockHorizontalRotationGrid[x, y, z] != Enums.Side.none)
                        Debug.Log("Rotate" + blockHorizontalRotationGrid[x, y, z]);
                    if (blockVerticalRotationGrid[x, y, z] != Enums.Side.none)
                        Debug.Log("Rotate" + blockHorizontalRotationGrid[x, y, z]);
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
            }
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
            if (child.gameObject.name == "Ground(Clone)") continue;
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
            if (hitRay.transform.gameObject.transform.name == "directionBlock")
            {
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
                return;
            }

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
            Debug.Log("blockVerticalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] = " +
                      blockVerticalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z]);
        }
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
        Debug.Log("blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] = " +
                  blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z]);
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