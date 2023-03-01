using System;
using System.Collections.Generic;
using System.Linq;
using Archi.Service.Interface;
using Interfaces;
using Levels;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

#if UNITY_STANDALONE && !UNITY_EDITOR
using JsonUtility = UnityEngine.JsonUtility;
#endif

public class SceneEditor
{
    public IDataBaseService m_Data;
    
    private GameObject[] prefabs;

    private GameObject selectedPrefab;
    public int selectedPrefabIndex;

    [SerializeField] private int sizeOfGridSpace = 1;

    private Camera _camera;

    private GameObject parent;

    [SerializeField] private string path;
    [SerializeField] private TMP_InputField inputField;

    public bool isMoveCamera = true;
    
    //LevelData
    public Vector3Int size;
    public Vector3Int defaultSize = new Vector3Int(10, 10, 10);
    // public List<List<List<int>>> blockGrid;
    public int[,,] blockGrid;
    public List<string> blocksUsed;
    public LevelData data;
    private Blocks blocks;


    private enum EditorMode
    {
        create,
        delete,
    }

    [SerializeField] private EditorMode Mode = EditorMode.create;

    public void Start()
    {
        size = defaultSize;
        blocks = new Blocks();
        // blockGrid = new List<List<List<int>>>();
        blockGrid = new int[size.x, size.y, size.z];
        prefabs = new GameObject[Blocks.BlockType.Count];
        foreach (var blockAddress in Blocks.BlockType)
        {
            if (blockAddress.Key == Enums.blockType.empty) continue;
            var block = Addressables.LoadAssetAsync<GameObject>(blockAddress.Value).WaitForCompletion();
            prefabs[(int)blockAddress.Key] = block;
        }

        _camera = Camera.main;
        parent = new GameObject();
        path = "Assets/SavedPrefab/" + parent.name + ".prefab";
        PlaceDefaultGround();
    }

    private void PlaceDefaultGround()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.z; z++)
            {
                var block = UnityEngine.Object.Instantiate(prefabs[1], new Vector3(x, 0, z), Quaternion.identity);
                block.transform.parent = parent.transform;
                blockGrid[x, 0, z] = 1;
            }
        }
    }

    public void Update()
    {
        selectedPrefab = prefabs[selectedPrefabIndex];
        if (Input.touchCount <= 0) return;
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
        switch (Mode)
        {
            case EditorMode.create:
                Create();
                break;

            case EditorMode.delete:
                Delete();
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
        if (Input.GetTouch(0).phase == TouchPhase.Began && isMoveCamera)
        {
            InstantiateNewBlock();
        }
        //else if the isMoveCamera is false
        else if (Input.GetTouch(0).phase == TouchPhase.Moved && !isMoveCamera)
        {
            InstantiateNewBlock();
        }
    }

    private void InstantiateNewBlock()
    {
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        Ray ray = _camera.ScreenPointToRay(position);
        Debug.Log("TryCast");
        if (Physics.Raycast(ray, out hitRay))
        {
            Debug.Log(hitRay.point);
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
        var blockPlacedAdress = Blocks.BlockType[(Enums.blockType)selectedPrefabIndex];
        if(!blocksUsed.Contains(blockPlacedAdress)) blocksUsed.Add(blockPlacedAdress);
        var newGo = UnityEngine.Object.Instantiate(selectedPrefab, position, Quaternion.identity);
        newGo.transform.parent = parent.transform;
        blockGrid[(int)position.x, (int)position.y, (int)position.z] = blocksUsed.IndexOf(blockPlacedAdress);
    }

    private void Delete()
    {
        Vector3 position = Input.GetTouch(0).position;
        RaycastHit hitRay;
        Ray ray = _camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hitRay))
        {
            UnityEngine.Object.Destroy(hitRay.transform.gameObject);
        }
    }

    public void SaveData()
    { 
        // var blockGridIntArray = TripleListToIntArray(blockGrid);
        data = new LevelData(blockGrid, blocksUsed.ToArray());
        m_Data.GenerateDataLevel(data);
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
        
    }

    public void CleanScene()
    {
        parent = GameObject.Find(inputField.text);
        foreach (Transform child in parent.transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
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