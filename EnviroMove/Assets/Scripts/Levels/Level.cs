using System;
using System.Collections;
using System.Collections.Generic;
using Archi.Service.Interface;
using Attributes;
using Cysharp.Threading.Tasks;
using Inputs;
using Interfaces;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using static AdresseHelper;

namespace Levels
{
    public delegate void LevelDelegate();

    public class Level : MonoBehaviour, IBoard
    {
        [ServiceDependency] private IInterfaceService m_interface;

        public LevelDelegate onFinishGenerate;


        public LevelData levelData;
        private IBoardable[,,] _board;
        private Enums.Side[,,] _playerDirBoard;
        private Dictionary<int, GameObject> _blocksUsed;
        private Dictionary<Vector3Int, Enums.Side> _cameraMovesSides = new();

        private Vector3Int _destinationPos;

        private GameObject _player;
        
        private List<VolumeProfile> volumeProfiles;
        private Volume volume;


        public async void GenerateLevel(LevelData data)
        {
            volume = Camera.main.transform.GetChild(0).GetComponent<Volume>();
            volumeProfiles = new List<VolumeProfile>
            {
                Addressables.LoadAssetAsync<VolumeProfile>("PP_Automne").WaitForCompletion(),
                Addressables.LoadAssetAsync<VolumeProfile>("PP_Hiver").WaitForCompletion(),
                Addressables.LoadAssetAsync<VolumeProfile>("PP_Printemps").WaitForCompletion()
            };

            switch (data.season)
            {
                case 0:
                    volume.profile = volumeProfiles[0];
                    break;
                case 1:
                    volume.profile = volumeProfiles[1];
                    break;
                case 2:
                    volume.profile = volumeProfiles[2];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _blocksUsed = new();

            _board = new IBoardable[data.size.x, data.size.y, data.size.z];
            _playerDirBoard = new Enums.Side[data.size.x, data.size.y, data.size.z];

            var dirBlock = await Addressables.LoadAssetAsync<GameObject>("DirectionBlock");

            for (int z = 0; z < data.size.z; z++)
            {
                for (int y = 0; y < data.size.y; y++)
                {
                    for (int x = 0; x < data.size.x; x++)
                    {
                        var dir = (Enums.Side)data.playerDir[x, y, z];
                        _playerDirBoard[x, y, z] = dir;
                        if (dir != Enums.Side.none)
                            Instantiate(dirBlock, transform.position + new Vector3(x, y, z),
                                Quaternion.LookRotation(Enums.SideVector3(dir), Vector3.up));
                    }
                }
            }


            int waitCount = 0;
            /*for (int i = 0; i < data.blocksUsed.Length; i++)
            {
               if (data.blocksUsed[i] != null)
               {
                  if(data.blocksUsed[i] == "" || data.blocksUsed[i] == "playerEndBlock") continue;  
                  if(data.blocksUsed[i] == "playerStartBlock") LoadAssetWithCallbackIndexed<GameObject>("Player", (obj, index) => { _blocksUsed[index] = obj; waitCount--; }, i);
                  else LoadAssetWithCallbackIndexed<GameObject>(data.blocksUsed[i], (obj, index) => { _blocksUsed[index] = obj; waitCount--; }, i);
                  waitCount++;
               }
            }*/

            _blocksUsed.Add((int)Enums.blockType.empty, null);
            _blocksUsed.Add((int)Enums.blockType.playerEnd, null);
            _blocksUsed.Add((int)Enums.blockType.panelEnd, null);
            foreach (var index in data.blockGrid)
            {
                Enums.blockType type = (Enums.blockType)index;
                if (type is Enums.blockType.playerEnd or Enums.blockType.empty or Enums.blockType.panelEnd) continue;
                string key = Blocks.BlockType[type];
                if (type == Enums.blockType.playerStart) key = "Player";
                if (!_blocksUsed.ContainsKey(index))
                {
                    LoadAssetWithCallbackIndexed<GameObject>(key, (block, i) =>
                    {
                        _blocksUsed[i] = block;
                        waitCount--;
                    }, index);
                    waitCount++;
                }
            }

            bool blocksLoad = false;
            while (!blocksLoad)
            {
                await UniTask.DelayFrame(1);
                if (waitCount == 0) blocksLoad = true;
            }

            for (int z = 0; z < _board.GetLength(2); z++)
            {
                for (int y = 0; y < _board.GetLength(1); y++)
                {
                    for (int x = 0; x < _board.GetLength(0); x++)
                    {
                        Vector3Int currentPos = new(x, y, z);
                        /*
                        Debug.Log($"currentPos: {currentPos}");
                        Debug.Log($"blockGrid: {data.blockGrid[currentPos.x, currentPos.y, currentPos.z]}");
                        Debug.Log($"Block use: {_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]]}");
                        */
                        if (_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]] == null)
                        {
                            if (data.blockGrid[currentPos.x, currentPos.y, currentPos.z] ==
                                (int)Enums.blockType.playerEnd) _destinationPos = currentPos;
                            if (data.blockGrid[currentPos.x, currentPos.y, currentPos.z] ==
                                (int)Enums.blockType.panelEnd)
                                _cameraMovesSides.Add(currentPos,
                                    (Enums.Side)data.blockHorizontalRotationGrid[x, y, z]);
                            continue;
                        }

                        Quaternion rotation = Quaternion.LookRotation(
                            Enums.SideVector3((Enums.Side)data.blockHorizontalRotationGrid[x, y, z]), Vector3.up);
                        GameObject currentGo;
                        switch (data.blockGrid[currentPos.x, currentPos.y, currentPos.z])
                        {
                            case > 97 and <= 186:
                            {
                                currentGo = Instantiate(_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]], transform.position + currentPos, Quaternion.identity);
                                var prefabIndex = data.blockGrid[currentPos.x, currentPos.y, currentPos.z];
                                switch (prefabIndex)
                                {
                                    //left bottom corner
                                    case (int)Enums.blockType.InsideBottomLeftCorner:
                                        currentGo.transform.Rotate(-90, -90, 0);
                                        currentGo.transform.position += new Vector3(.5f, -0.5f, .5f);
                                        break;
                                    //right bottom corner
                                    case (int)Enums.blockType.InsideBottomRightCorner:
                                        currentGo.transform.Rotate(-90, 0, -90);
                                        currentGo.transform.position += new Vector3(-.5f, -0.5f, .5f);
                                        break;
                                    //left top corner
                                    case (int)Enums.blockType.InsideTopLeftCorner:
                                        currentGo.transform.Rotate(-90, 0, -90);
                                        currentGo.transform.position += new Vector3(.5f, -0.5f, -.5f);
                                        break;
                                    //right top corner
                                    case (int)Enums.blockType.InsideTopRightCorner:
                                        currentGo.transform.Rotate(-90, 0, -90);
                                        currentGo.transform.position += new Vector3(-.5f, -0.5f, -.5f);
                                        break;
                                    //left side
                                    case >= (int)Enums.blockType.InsideLeft1 and <= (int)Enums.blockType.InsideLeft12:
                                        currentGo.transform.Rotate(-90, 180, 0);
                                        currentGo.transform.position += new Vector3(.5f, -0.5f, -.5f);
                                        break;
                                    //right side
                                    case >= (int)Enums.blockType.InsideRight1 and <= (int)Enums.blockType.InsideRight12:
                                        currentGo.transform.Rotate(-90, 0, 0);
                                        currentGo.transform.position += new Vector3(-.5f, -0.5f, .5f);
                                        break;
                                    //top side
                                    case >= (int)Enums.blockType.InsideTop1 and <= (int)Enums.blockType.InsideTop6:
                                        currentGo.transform.Rotate(-90, -90, 0);
                                        currentGo.transform.position += new Vector3(-.5f, -0.5f, -.5f);
                                        break;
                                    //bottom side
                                    case >= (int)Enums.blockType.InsideBottom1 and <= (int)Enums.blockType.InsideBottom6:
                                        currentGo.transform.Rotate(-90, 0, 90);
                                        currentGo.transform.position += new Vector3(.5f, -0.5f, .5f);
                                        break;
                                }

                                break;
                            }
                            case (int)Enums.blockType.M1_Block1 or (int)Enums.blockType.ground or (int)Enums.blockType.M2_Block1 or (int)Enums.blockType.M3_Block1:
                                currentGo = Instantiate(
                                    _blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]],
                                    transform.position + currentPos, quaternion.identity, transform);
                                currentGo.transform.Rotate(90 * UnityEngine.Random.Range(0, 4),
                                    90 * UnityEngine.Random.Range(0, 4), 90 * UnityEngine.Random.Range(0, 4));
                                break;
                            default:
                                currentGo = Instantiate(
                                    _blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]],
                                    transform.position + currentPos, rotation, transform);
                                break;
                        }

                        currentGo.name = currentGo.name + currentPos;
                        if (data.blockGrid[currentPos.x, currentPos.y, currentPos.z] ==
                            (int)Enums.blockType.playerStart)
                        {
                            _player = currentGo;
                            var mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                            mesh.material = _player.GetComponent<Player>().GetMat(PlayerPrefs.GetInt("PlayerSkin"));
                        }

                        IBoardable currentBoardable = currentGo.GetComponent<IBoardable>();
                        if (currentBoardable == null)
                            throw new MissingMemberException($"{currentGo.name} isn't Boardable");
                        currentBoardable.SetOnBoard(currentPos, (Enums.Side)data.blockHorizontalRotationGrid[x, y, z],
                            this);
                        _board[currentPos.x, currentPos.y, currentPos.z] = currentBoardable;
                        onFinishGenerate += currentBoardable.StartBoard;
                    }
                }
            }

            Inputs.Inputs.Instance.OnTouch += StartBoard;
            onFinishGenerate?.Invoke();
        }

        private void StartBoard(Vector2 side)
        {
            Inputs.Inputs.Instance.OnTouch -= StartBoard;
            InteractionDetector.Instance.isActive = true;
            _player.GetComponent<Player>().Move();
        }


        


        public IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side, out bool boardLimit)
        {
            return GetNeighbor(boardPos, side, out boardLimit, out _);
        }

        public IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side, out bool boardLimit,
            out Vector3Int neighborPos)
        {
            neighborPos = boardPos;
            boardLimit = false;
            switch (side)
            {
                case Enums.Side.forward:
                    if (boardPos.z + 1 == _board.GetLength(2))
                    {
                        boardLimit = true;
                        return null;
                    }

                    neighborPos = new Vector3Int(boardPos.x, boardPos.y, boardPos.z + 1);
                    return _board[neighborPos.x, neighborPos.y, neighborPos.z];
                case Enums.Side.left:
                    if (boardPos.x == 0)
                    {
                        boardLimit = true;
                        return null;
                    }

                    neighborPos = new Vector3Int(boardPos.x - 1, boardPos.y, boardPos.z);
                    return _board[neighborPos.x, neighborPos.y, neighborPos.z];
                case Enums.Side.right:
                    if (boardPos.x + 1 == _board.GetLength(0))
                    {
                        boardLimit = true;
                        return null;
                    }

                    neighborPos = new Vector3Int(boardPos.x + 1, boardPos.y, boardPos.z);
                    return _board[neighborPos.x, neighborPos.y, neighborPos.z];
                case Enums.Side.back:
                    if (boardPos.z == 0)
                    {
                        boardLimit = true;
                        return null;
                    }

                    neighborPos = new Vector3Int(boardPos.x, boardPos.y, boardPos.z - 1);
                    return _board[neighborPos.x, neighborPos.y, neighborPos.z];
                case Enums.Side.up:
                    if (boardPos.y == _board.GetLength(1) - 1)
                    {
                        boardLimit = true;
                        return null;
                    }

                    neighborPos = new Vector3Int(boardPos.x, boardPos.y + 1, boardPos.z);
                    return _board[neighborPos.x, neighborPos.y, neighborPos.z];
                case Enums.Side.down:
                    if (boardPos.y == 0)
                    {
                        boardLimit = true;
                        return null;
                    }

                    neighborPos = new Vector3Int(boardPos.x, boardPos.y - 1, boardPos.z);
                    return _board[neighborPos.x, neighborPos.y, neighborPos.z];
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }

      public void SetAt(IBoardable boardable, Vector3Int position)
      {
         if (position.x >= _board.GetLength(0)) return;
         if (position.y >= _board.GetLength(1)) return;
         if (position.z >= _board.GetLength(2)) return;
         _board[position.x, position.y, position.z] = boardable;
      }

        void SetBoardable(IBoardable boardable, Vector3Int boardPos, Enums.Side side = Enums.Side.none)
        {
            throw new NotImplementedException();
        }

        public void Move(IBoardable boardable, Vector3Int position)
        {
            for (int z = 0; z < _board.GetLength(2); z++)
            {
                for (int y = 0; y < _board.GetLength(1); y++)
                {
                    for (int x = 0; x < _board.GetLength(0); x++)
                    {
                        if (_board[x, y, z] == boardable)
                        {
                            _board[position.x, position.y, position.z]?.MoveOn(boardable, position);
                            _board[position.x, position.y, position.z] = _board[x, y, z];
                            _board[position.x, position.y, position.z].SetPosition(position);
                            _board[x, y, z] = null;
                            return;
                        }
                    }
                }
            }

            _board[position.x, position.y, position.z]?.MoveOn(boardable, position);
            _board[position.x, position.y, position.z] = boardable;
            _board[position.x, position.y, position.z].SetPosition(position);
        }

        // public void SetAt(IBoardable boardable, Vector3Int position)
        // {
        //     _board[position.x, position.y, position.z] = boardable;
        // }
        
        public Vector3Int GetPosition(IBoardable boardable)
        {
            for (int z = 0; z < _board.GetLength(2); z++)
            {
                for (int y = 0; y < _board.GetLength(1); y++)
                {
                    for (int x = 0; x < _board.GetLength(0); x++)
                    {
                        if (_board[x, y, z] == boardable) return new Vector3Int(x, y, z);
                    }
                }
            }

            throw new NullReferenceException($"{name} board Doesn't Countain {boardable}");
        }

        public void RemoveBoardable(IBoardable boardable)
        {
            var pos = GetPosition(boardable);
            _board[pos.x, pos.y, pos.z] = null;
            Debug.Log($"Remove Boardable At {pos}");
        }

        public Vector3 GetWorldPos(Vector3Int boardPos) => transform.position + boardPos;

        public Enums.Side GetPlayerDirection(Vector3Int pos) => _playerDirBoard[pos.x, pos.y, pos.z];

        public void CheckCameraMovement(Vector3Int pos)
        {
            if (_cameraMovesSides.TryGetValue(pos, out var side)) StartCoroutine(MoveCamera(side));
        }

        public void CheckFinishLevel(Vector3Int pos)
        {
            if (pos == _destinationPos) FinishLevel();
        }

        public void FinishLevel()
        {
            Destroy(_player);
            m_interface.DrawCanvas(Enums.MajorCanvas.winMenu);
        }

        public void GameOver()
        {
            Destroy(_player);
            m_interface.DrawCanvas(Enums.MajorCanvas.gameOver);
        }

        public bool TryMove(Vector3Int boardablePosition, Enums.Side side, out Vector3 position)
        {
            position = transform.position + boardablePosition;
            var mover = _board[boardablePosition.x, boardablePosition.y, boardablePosition.z];
            IBoardable neighboor =
                GetNeighbor(boardablePosition, side, out bool boardLimit, out Vector3Int neighborPos);
            if (boardLimit) return false;
            if (neighboor != null)
            {
                if (!neighboor.TryMoveOn(mover, Enums.InverseSide(side),
                        boardablePosition + Enums.SideVector3Int(side))) return false;
            }

            position = transform.position + neighborPos;
            _board[neighborPos.x, neighborPos.y, neighborPos.z] =
                _board[boardablePosition.x, boardablePosition.y, boardablePosition.z];
            _board[neighborPos.x, neighborPos.y, neighborPos.z].SetPosition(neighborPos);
            _board[boardablePosition.x, boardablePosition.y, boardablePosition.z] = null;

            return true;
        }

        public bool CanMove(Vector3Int boardablePosition, Enums.Side side, bool isCalledByPlayer, out Vector3 position,
            out Vector3Int boardPos)
        {
            position = transform.position + boardablePosition;
            boardPos = boardablePosition;
            var mover = _board[boardablePosition.x, boardablePosition.y, boardablePosition.z];
            IBoardable neighboor =
                GetNeighbor(boardablePosition, side, out bool boardLimit, out Vector3Int neighborPos);
            if (boardLimit) return false;
            if (neighboor != null && !(isCalledByPlayer && neighboor is Player))
            {
                if (!neighboor.CanMoveOn(mover, Enums.InverseSide(side),
                        boardablePosition + Enums.SideVector3Int(side))) return false;
            }

            boardPos = neighborPos;
            position = transform.position + neighborPos;
            return true;
        }

        IEnumerator MoveCamera(Enums.Side side)
        {
            float camSpeed = 10f * Time.deltaTime;
            Camera cam = Camera.main;
            var camDist = Enums.SideVector3(side);
            switch (side)
            {
                case Enums.Side.forward:
                case Enums.Side.back:
                    camDist *= 16f;
                    break;
                case Enums.Side.left:
                case Enums.Side.right:
                    camDist *= 10f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            var newCamPos = cam.transform.position + camDist;

            while (Vector3.Distance(cam.transform.position, newCamPos) > camSpeed)
            {
                cam.transform.position += Enums.SideVector3(side) * camSpeed;
                yield return new WaitForEndOfFrame();
            }

            cam.transform.position = newCamPos;
        }
    }
}