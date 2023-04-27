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
    public Vector3Int defaultSize = new(6, 10, 12);
    public int[,,] blockGrid;
    public List<int[,,]> blockGridList;
    public int[,][,,] blockGridGrid;
    public Enums.Side[,,] blockHorizontalRotationGrid;
    public Enums.Side[,,] blockVerticalRotationGrid;
    public List<string> blocksUsed;
    public LevelData data;

    //LevelDataList
    public int[,] levelGridPosition;
    public List<LevelData> levelDataList;

    //Path
    public Enums.Side[,,] directionGrid;

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
        parent.name = "Grid1";
        selectedPrefab = prefabs[1];
        InitializeDirectionGrid();
        PlaceDefaultGround();
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
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.z; z++)
            {
                var block = Object.Instantiate(prefabs[1], new Vector3(x, 0, z), Quaternion.identity);
                block.transform.parent = parent.transform;
                blockGrid[x, 0, z] = 1;
                blockVerticalRotationGrid[x, 0, z] = 0;
                blockHorizontalRotationGrid[x, 0, z] = 0;
            }
        }

        blocksUsed.Add("groundBlock");
        blockGridList.Add(blockGrid);
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
        if (position.x < blockHit.transform.parent.transform.position.x || position.x >= blockHit.transform.parent.transform.position.x + size.x || 
            position.y < blockHit.transform.parent.transform.position.y || position.y >= blockHit.transform.parent.transform.position.y + size.y || 
            position.z < blockHit.transform.parent.transform.position.z || position.z >= blockHit.transform.parent.transform.position.z + size.z) return;
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
                Vector3 posOfNewGrid = new Vector3();
                Vector3 posOfnewPanelStart = new Vector3();
                var position1 = newGo.gameObject.transform.parent.gameObject.transform.position;
                Vector3 parentPos = new Vector3(position1.x, 0, position1.z);
                Enums.Side sideToInstantiateNewGrid = Enums.Side.none;
                for (int i = 0; i < 3; i++)
                {
                    if (position.x == parentPos.x)
                    {
                        newground = Object.Instantiate(prefabs[1], new Vector3(parentPos.x - 1 - i, 0, position.z), Quaternion.identity);
                        posOfNewGrid = new Vector3(parentPos.x - 1 - i, 0, parentPos.z);
                        posOfnewPanelStart = new Vector3(parentPos.x - 2 - i, position.y, position.z);
                        sideToInstantiateNewGrid = Enums.Side.left;
                    }
                    else if (position.x == parentPos.x + size.x - 1)
                    {
                        newground = Object.Instantiate(prefabs[1], new Vector3(size.x + i, 0, position.z), Quaternion.identity);
                        posOfNewGrid = new Vector3(parentPos.x + size.x + i, 0, parentPos.z);
                        posOfnewPanelStart = new Vector3(parentPos.x + size.x + 1 + i, position.y, position.z);
                        sideToInstantiateNewGrid = Enums.Side.right;
                    }
                    else if (position.z == parentPos.z)
                    {
                        newground = Object.Instantiate(prefabs[1], new Vector3(position.x, 0, parentPos.z - 1 - i), Quaternion.identity);
                        posOfNewGrid = new Vector3(parentPos.x, 0, parentPos.z - 1 - i);
                        posOfnewPanelStart = new Vector3(position.x, position.y, parentPos.z - 2 - i);
                        sideToInstantiateNewGrid = Enums.Side.back;
                    }
                    else if (position.z == parentPos.x + size.z - 1)
                    {
                        newground = Object.Instantiate(prefabs[1], new Vector3(position.x, 0, size.z + i), Quaternion.identity);
                        posOfNewGrid = new Vector3(parentPos.x, 0, parentPos.z + size.z + i);
                        posOfnewPanelStart = new Vector3(position.x, position.y, parentPos.z + size.z + 1 + i);
                        sideToInstantiateNewGrid = Enums.Side.forward;
                    }
                }
                var newGrid = new GameObject();
                if (newground != null)
                {
                    var newPanelStart = Object.Instantiate(prefabs[12], posOfnewPanelStart, Quaternion.identity);
                }
                

                newGrid.name = "Grid(" + blockGridList.Count + ")";
                newGrid.transform.position = new Vector3(posOfNewGrid.x, position.y, posOfNewGrid.z);
                var transformPosition = newGrid.transform.position;
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        for (int z = 0; z < size.z; z++)
                        {
                            if (blockGrid[x, y, z] != 0)
                            {
                                switch (sideToInstantiateNewGrid)
                                {
                                    case (Enums.Side.forward):
                                        var newBlock = Object.Instantiate(prefabs[1],
                                            new Vector3(x + transformPosition.x, transformPosition.y - 1 , z + transformPosition.z + 1), Quaternion.identity);
                                        newBlock.transform.parent = newGrid.transform;
                                        break;
                                    case (Enums.Side.right):
                                        var newBlock1 = Object.Instantiate(prefabs[1],
                                            new Vector3(size.x - x + transformPosition.x, transformPosition.y - 1, z + transformPosition.z), Quaternion.identity);
                                        newBlock1.transform.parent = newGrid.transform;
                                        break;
                                    case (Enums.Side.left):
                                        var newBlock2 = Object.Instantiate(prefabs[1],
                                            new Vector3(- x + transformPosition.x - 1, transformPosition.y - 1, transformPosition.z + z), Quaternion.identity);
                                        newBlock2.transform.parent = newGrid.transform;
                                        break;
                                    case (Enums.Side.back):
                                        var newBlock3 = Object.Instantiate(prefabs[1],
                                            new Vector3(transformPosition.x + x , transformPosition.y - 1, -z + transformPosition.z - 1), Quaternion.identity);
                                        newBlock3.transform.parent = newGrid.transform;
                                        break;
                                }
                            }
                        }
                    }
                }
                break;
        }

        var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
        if (!blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Add(blockPlacedAddress);
        // var blockGridToModify = blockGridList[blockPlacedAddress];
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
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), Enums.SideToIntArray(blockHorizontalRotationGrid),
            Enums.SideToIntArray(blockVerticalRotationGrid), Enums.SideToVector3Array(directionGrid));
        curentLevelData = data;
        Debug.Log("data: " + (string)data);
    }

    public LevelData GetData()
    {
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), Enums.SideToIntArray(blockHorizontalRotationGrid),
            Enums.SideToIntArray(blockVerticalRotationGrid), Enums.SideToVector3Array(directionGrid));
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