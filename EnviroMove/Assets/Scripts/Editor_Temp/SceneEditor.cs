using System.Collections.Generic;
using System.Linq;
using Archi.Service.Interface;
using Attributes;
using Levels;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    // public List<List<List<int>>> blockGrid;
    public int[,,] blockGrid;
    public int[,,] blockHorizontalRotationGrid;
    public int[,,] blockVerticalRotationGrid;
    public List<string> blocksUsed;
    public LevelData data;
    private Blocks blocks;
    
    //path
    public GameObject startBlock;
    public GameObject endBlock;
    public Node[,,] pathGrid;
    public List<GameObject> wallsAndFloors;
    private PathFinding pathFinding;
    public List<Node> pathForPlayer;

    //SelectBox
    private GameObject selectionBox;
    private Image selectionBoxImage;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 endPosition = Vector2.zero;


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
        selectionBox = Addressables.LoadAssetAsync<GameObject>("SelectionBox").WaitForCompletion();
        selectionBoxImage = selectionBox.GetComponent<Image>();

        size = defaultSize;
        blocks = new Blocks();
        // blockGrid = new List<List<List<int>>>();
        blocksUsed = new List<string>();
        blocksUsed.Add(null);
        blockGrid = new int[size.x, size.y, size.z];
        blockHorizontalRotationGrid = new int[size.x, size.y, size.z];
        blockVerticalRotationGrid = new int[size.x, size.y, size.z];
        wallsAndFloors = new List<GameObject>();
        pathGrid = new Node[size.x, size.y, size.z];
        pathForPlayer = new List<Node>();
        pathFinding = new PathFinding(this);
        prefabs = new GameObject[Blocks.BlockType.Count];
        foreach (var blockAddress in Blocks.BlockType)
        {
            if (blockAddress.Key == Enums.blockType.empty) continue;
            var block = Addressables.LoadAssetAsync<GameObject>(blockAddress.Value).WaitForCompletion();
            prefabs[(int)blockAddress.Key] = block;
        }

        _camera = Camera.main;
        parent = new GameObject();
        parent.name = "Level";
        selectedPrefab = prefabs[1];
        PlaceDefaultGround();
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
                wallsAndFloors.Add(block);
            }
        }
        blocksUsed.Add("groundBlock");
    }

    public void Update()
    {
        selectedPrefab = prefabs[selectedPrefabIndex];
        if (startBlock != null && endBlock != null && pathForPlayer.Count == 0)
        {
            pathForPlayer = pathFinding.FindPath(startBlock.transform.position, endBlock.transform.position);
        }
        if (startBlock != null)
        {
            Debug.Log("start block is placed");
        }
        
        if (endBlock != null)
        {
            Debug.Log("end block is placed");
        }
        // if (pathForPlayer != null)
        // {
        //     foreach (var node in pathForPlayer)
        //     {
        //         Debug.DrawRay(node.position, Vector3.up, Color.blue);
        //         Debug.Log(node.position);
        //     }
        // }
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
            case EditorMode.snapMode:
                SelectionBox();
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
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void InstantiateNewBlock()
    {
        Vector3 position = Input.GetTouch(0).position;
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
        }
        else
        {
            position = _camera.ScreenToWorldPoint(new Vector3(position.x, position.y, 10));
        }

        position.x = Mathf.Round(position.x / sizeOfGridSpace) * sizeOfGridSpace;
        position.y = Mathf.Round(position.y / sizeOfGridSpace) * sizeOfGridSpace;
        position.z = Mathf.Round(position.z / sizeOfGridSpace) * sizeOfGridSpace;
        if (position.x < 0 || position.x >= size.x || position.y < 0 || position.y >= size.y || position.z < 0 || position.z >= size.z) return;
        var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
        if(!blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Add(blockPlacedAddress);
        var newGo = Object.Instantiate(selectedPrefab, position, Quaternion.identity);
        newGo.transform.parent = parent.transform;
        blockGrid[(int)position.x, (int)position.y, (int)position.z] = selectedPrefabIndex;
        Debug.Log("Block Placed");
        Debug.Log("blockGrid[" + (int)position.x + "," + (int)position.y + "," + (int)position.z + "] = " + selectedPrefabIndex);
        blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = 0;
        blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = 0;
        if (selectedPrefabIndex == 4)
        {
            startBlock = newGo;
        }
        else if (selectedPrefabIndex == 5)
        {
            endBlock = newGo;
        }
        if (selectedPrefabIndex is 1 or 2)
        {
            wallsAndFloors.Add(newGo);
        }
    }

    private void Delete()
    {
        if (IsPointerOverUIObject()) return;
        if (EventSystem.current.currentSelectedGameObject) return;
        if (Input.GetTouch(0).phase != TouchPhase.Began) return;
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        Ray ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
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
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGrid, blockVerticalRotationGrid);
        curentLevelData = data;
        Debug.Log("data: " + (string)data);
        m_Data.GenerateDataLevel(data, name);
    }

    public LevelData TestLevel()
    {
        // if (!hasStartAndEnd()) return null;
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGrid, blockVerticalRotationGrid);
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
        Debug.Log("size of blockHorizontalRotationGrid: " + blockHorizontalRotationGrid.GetLength(0) + " " + blockHorizontalRotationGrid.GetLength(1) + " " + blockHorizontalRotationGrid.GetLength(2));
        Debug.Log("size of blockVerticalRotationGrid: " + blockVerticalRotationGrid.GetLength(0) + " " + blockVerticalRotationGrid.GetLength(1) + " " + blockVerticalRotationGrid.GetLength(2));
        Debug.Log("size of blockGrid: " + blockGrid.GetLength(0) + " " + blockGrid.GetLength(1) + " " + blockGrid.GetLength(2));
        blockHorizontalRotationGrid = dataToLoad.blockHorizontalRotationGrid;
        blockVerticalRotationGrid = dataToLoad.blockVerticalRotationGrid;
        for (int z = 0; z < blockGrid.GetLength(0); z++)
        {
            for (int y = 0; y < blockGrid.GetLength(1); y++)
            {
                for (int x = 0; x < blockGrid.GetLength(2); x++)
                {
                    Debug.Log("x= " + x + " y= " + y + " z= " + z + " blockGrid[x, y, z]= " + blockGrid[x, y, z]);
                    if (blockGrid[x, y, z] == 0) continue;
                    Debug.Log("prefabs[" + blockGrid[x, y, z] + "]");
                    // Debug.Log("blockUsed[" + blockGrid[x, y, z] + "] = " + blocksUsed[blockGrid[x, y, z]]);
                    // int prefabIndex = (int)Blocks.BlockAdressType[blocksUsed[blockGrid[x, y, z]]];
                    int prefabIndex = blockGrid[x, y, z];
                    var block = UnityEngine.Object.Instantiate(prefabs[prefabIndex/*blockGrid[x, y, z]*/], new Vector3(x, y, z), Quaternion.identity);
                    block.transform.Rotate(0, blockHorizontalRotationGrid[x, y, z] * 90, 0);
                    block.transform.Rotate(blockVerticalRotationGrid[x, y, z] * 90, 0, 0);
                    block.transform.parent = parent.transform;
                    switch (prefabIndex)
                    {
                        case 4:
                            startBlock = block;
                            break;
                        case 5:
                            endBlock = block;
                            break;
                    }
                }
            }
        }
    }
//(int)Blocks.BlockAdressType[blocksUsed.FirstOrDefault(jpp => jpp == blocksUsed[blockGrid[x, y, z]]) ?? string.Empty];
    public void CleanScene()
    {
        foreach (Transform child in parent.transform)
        {
            Object.Destroy(child.gameObject);
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
            hitRay.transform.Rotate(90, 0, 0);
        }
        var blockPosition = hitRay.transform.position;
        blockVerticalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] = (int)hitRay.transform.rotation.eulerAngles.x / 90;
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
            hitRay.transform.Rotate(0, 90, 0);
        }
        
        var blockPosition = hitRay.transform.position;
        blockHorizontalRotationGrid[(int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z] = (int)hitRay.transform.rotation.eulerAngles.y / 90;
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

    #region SelectBox

    

    private void SelectionBox()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Finger down");
            StartSelectionBox();
        }
        else if (Input.GetMouseButton(0))
        {
            Debug.Log("finger held");
            UpdateSelectionBox();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("finger up");
            EndSelectionBox();
        }
    }
    
    private void StartSelectionBox()
    {
        startPosition = Input.GetTouch(0).position;
        endPosition = startPosition;
    }

    private void UpdateSelectionBox()
    {
        endPosition = Input.GetTouch(0).position;
        ChangeSelectionBoxSize();
        if (startPosition != endPosition)
        {
            //TODO Select all in selection box
        }
        else if (startPosition == endPosition)
        {
            //TODO Select What was touched
        }
    }

    private void ChangeSelectionBoxSize()
    {
        if (selectionBoxImage == null) return;
        selectionBoxImage.transform.position = startPosition + (endPosition - startPosition) / 2;
        selectionBoxImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(endPosition.x - startPosition.x), Mathf.Abs(endPosition.y - startPosition.y));
    }

    private void EndSelectionBox()
    {
        
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        ResetSelectionBoxSize();

    }

    private void ResetSelectionBoxSize()
    {
        if (selectionBoxImage == null) return;
        selectionBoxImage.transform.position = Vector2.zero;
        selectionBoxImage.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
    }

    #endregion
}