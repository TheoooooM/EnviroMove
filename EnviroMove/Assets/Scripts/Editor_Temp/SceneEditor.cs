using System.Collections.Generic;
using System.Linq;
using Archi.Service.Interface;
using Attributes;
using Levels;
using TMPro;
using UnityEditor;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    //SelectBox
    private GameObject selectionBox;
    private Image selectionBoxImage;
    private Vector2 startPosition = Vector2.zero;
    private Vector2 endPosition = Vector2.zero;
    
    //Path
    public Vector3[,,] directionGrid;

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
        parent.name = "Level";
        selectedPrefab = prefabs[1];
        InitializeDirectionGrid();
        PlaceDefaultGround();
    }

    private void InitializeDirectionGrid()
    {
        directionGrid = new Vector3[size.x, size.y, size.z];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y - 1; y++)
            {
                for (int z = 0; z < size.z; z++)
                {
                    directionGrid[x, y, z] = Vector3.zero;
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
    }

    public void Update()
    {
        if (_camera == null) _camera = Camera.main;
        if (parent == null) parent = new GameObject();
        if (directionGrid != null)
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y - 1; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Debug.DrawRay(new Vector3(x, y, z), directionGrid[x, y, z], Color.red);
                    }
                }
            }
        }
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
        if (selectedPrefab == prefabs[11])
        {
            directionGrid[(int)position.x, (int)position.y, (int)position.z] = new Vector3(0, 0, 1);
            newGo.name = "directionBlock";
            return;
        }
        newGo.transform.parent = parent.transform;
        blockGrid[(int)position.x, (int)position.y, (int)position.z] = selectedPrefabIndex;
        Debug.Log("Block Placed");
        Debug.Log("blockGrid[" + (int)position.x + "," + (int)position.y + "," + (int)position.z + "] = " + selectedPrefabIndex);
        blockHorizontalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = 0;
        blockVerticalRotationGrid[(int)position.x, (int)position.y, (int)position.z] = 0;
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
            if (blockGrid[(int)hitRay.transform.position.x, (int)hitRay.transform.position.y, (int)hitRay.transform.position.z] == 0)
            {
                var blockPlacedAddress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
                if(blocksUsed.Contains(blockPlacedAddress)) blocksUsed.Remove(blockPlacedAddress);
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
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGrid, blockVerticalRotationGrid, directionGrid);
        curentLevelData = data;
        Debug.Log("data: " + (string)data);
        m_Data.GenerateDataLevel(data, name);
    }

    public LevelData TestLevel()
    {
        // if (!hasStartAndEnd()) return null;
        data = new LevelData(size, blockGrid, blocksUsed.ToArray(), blockHorizontalRotationGrid, blockVerticalRotationGrid, directionGrid);
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
        for (int z = 0; z < blockGrid.GetLength(2); z++)
        {
            for (int y = 0; y < blockGrid.GetLength(1); y++)
            {
                for (int x = 0; x < blockGrid.GetLength(0); x++)
                {
                    Debug.Log("x= " + x + " y= " + y + " z= " + z + " blockGrid[x, y, z]= " + blockGrid[x, y, z]);
                    if (blockGrid[x, y, z] == 0) continue;
                    Debug.Log("prefabs[" + blockGrid[x, y, z] + "]");
                    int prefabIndex = blockGrid[x, y, z];
                    var block = UnityEngine.Object.Instantiate(prefabs[prefabIndex/*blockGrid[x, y, z]*/], new Vector3(x, y, z), Quaternion.identity);
                    block.transform.Rotate(0, blockHorizontalRotationGrid[x, y, z] * 90, 0);
                    block.transform.Rotate(blockVerticalRotationGrid[x, y, z] * 90, 0, 0);
                    block.transform.parent = parent.transform;
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

    private void VerticalRotation()
    {
        if (IsPointerOverUIObject()) return;
        if (Input.GetTouch(0).phase != TouchPhase.Began) return;
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        Ray ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            if (hitRay.transform.gameObject.transform.name ==  "directionBlock") return;
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
            if (hitRay.transform.gameObject.transform.name ==  "directionBlock")
            {
                var position1 = hitRay.transform.position;
                if (directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] ==  Vector3.forward)
                {
                    directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] = Vector3.right;
                }
                else if (directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] ==  Vector3.right)
                {
                    directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] = Vector3.back;
                }
                else if (directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] ==  Vector3.back)
                {
                    directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] = Vector3.left;
                }
                else if (directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] ==  Vector3.left)
                {
                    directionGrid[(int)position1.x, (int)position1.y, (int)position1.z] = Vector3.forward;
                }
            }
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