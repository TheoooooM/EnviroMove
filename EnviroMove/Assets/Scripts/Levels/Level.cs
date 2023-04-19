using System;
using Archi.Service.Interface;
using Attributes;
using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using static AdresseHelper;

namespace Levels
{
   public delegate void LevelDelegate();
   public class Level : MonoBehaviour, IBoard
   {
      public LevelDelegate onFinishGenerate;
      
      
      public LevelData levelData;
      private IBoardable[,,] _board;
      private GameObject[] _blocksUsed;

      private Vector3Int _destinationPos;

      void LoadBlocks()
      {
         throw new System.NotImplementedException();
      }


      public async void GenerateLevel(LevelData data)
      {
         _blocksUsed = new GameObject[data.blocksUsed.Length];
         _board = new IBoardable[data.size.x,data.size.y,data.size.z];

         int waitCount = 0;
         for (int i = 0; i < data.blocksUsed.Length; i++)
         {
            if (data.blocksUsed[i] != null)
            {
               if(data.blocksUsed[i] == null || data.blocksUsed[i] == "playerEndBlock") continue;  
               if(data.blocksUsed[i] == "playerStartBlock") LoadAssetWithCallbackIndexed<GameObject>("Player", (obj, index) => { _blocksUsed[index] = obj; waitCount--; }, i);
               else LoadAssetWithCallbackIndexed<GameObject>(data.blocksUsed[i], (obj, index) => { _blocksUsed[index] = obj; waitCount--; }, i);
               waitCount++;
            }
         }

         bool blocksLoad = false;
         while (!blocksLoad)
         {
            await UniTask.DelayFrame(1);
            if(waitCount == 0)blocksLoad = true;
         }

         for (int z = 0; z < _board.GetLength(2); z++)
         {
            for (int y = 0; y < _board.GetLength(1); y++)
            {
               for (int x = 0; x < _board.GetLength(0); x++)
               {
                  Vector3Int currentPos = new(x,y,z);
                  /*
                  Debug.Log($"currentPos: {currentPos}");
                  Debug.Log($"blockGrid: {data.blockGrid[currentPos.x, currentPos.y, currentPos.z]}");
                  Debug.Log($"Block use: {_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]]}");
                  */
                  Debug.Log($"currentPos:{currentPos}, blockIndex:{data.blockGrid[currentPos.x, currentPos.y, currentPos.z]},");
                  if (_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]] == null)
                  {
                     if (data.blockGrid[currentPos.x, currentPos.y, currentPos.z] != 0) _destinationPos = currentPos;
                     continue;
                  }
                  
                  GameObject currentGo = Instantiate(_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]],
                        transform.position + currentPos, Quaternion.identity, transform);
                  IBoardable currentBoardable = currentGo.GetComponent<IBoardable>();
                  if (currentBoardable == null) throw new MissingMemberException($"{currentGo.name} isn't Boardable");
                  currentBoardable.SetOnBoard(currentPos, this);
                  _board[currentPos.x, currentPos.y, currentPos.z] = currentBoardable;
                  onFinishGenerate += currentBoardable.StartBoard;
               }
            }
         }
         
         onFinishGenerate?.Invoke();
      }

      IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side)
      {
         return GetNeighbor(boardPos, side, out _);
      }
      
      IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side, out Vector3Int neighborPos)
      {
         neighborPos = boardPos;
         switch (side)
         {
            case Enums.Side.top:
               if (boardPos.z + 1 == _board.GetLength(2)) return null;
               neighborPos = new Vector3Int(boardPos.x, boardPos.y, boardPos.z + 1);
               return _board[neighborPos.x, neighborPos.y, neighborPos.z];
            case Enums.Side.left:
               if (boardPos.x + 1 == _board.GetLength(0)) return null;
               neighborPos = new Vector3Int(boardPos.x - 1, boardPos.y, boardPos.z);
               return _board[neighborPos.x-1, neighborPos.y, neighborPos.z];
            case Enums.Side.right:
               if (boardPos.x == 0) return null;
               neighborPos = new Vector3Int(boardPos.x + 1, boardPos.y, boardPos.z);
               return _board[neighborPos.x+1, neighborPos.y, neighborPos.z];
               break;
            case Enums.Side.back:
               if (boardPos.z == 0) return null;
               neighborPos = new Vector3Int(boardPos.x, boardPos.y, boardPos.z - 1);
               return _board[neighborPos.x, neighborPos.y, neighborPos.z - 1];
            default:
               throw new ArgumentOutOfRangeException(nameof(side), side, null);
         }
            /*case Enums.Side.up :
               if (boardPos.y+1 == levelSize.y) return null;
               return  blockGrid[(int)boardPos.x,(int)boardPos.y+1];
            case Enums.Side.left :
               if (boardPos.x == 0) return null;
               return  blockGrid[(int)boardPos.x-1,(int)boardPos.y];
            case Enums.Side.right :
               if (boardPos.x+1 == levelSize.x) return null;
               return  blockGrid[(int)boardPos.x+1,(int)boardPos.y];
            case Enums.Side.down :
               if (boardPos.y == 0) return null;
               return  blockGrid[(int)boardPos.x,(int)boardPos.y-1];
            default: return null;*/
      }

      void SetBoardable(IBoardable boardable, Vector3Int boardPos, Enums.Side side = Enums.Side.none)
      {
         
      }

      public bool TryMove(Vector3Int boardablePosition, Enums.Side side, out Vector3 position)
      {
         position = transform.position + boardablePosition;
         
         IBoardable neighboor = GetNeighbor(boardablePosition, side, out Vector3Int neighborPos);
         if (neighboor != null) return false;
         position = transform.position + neighborPos;
         _board[neighborPos.x, neighborPos.y, neighborPos.z] = _board[boardablePosition.x, boardablePosition.y, boardablePosition.z];
         _board[neighborPos.x, neighborPos.y, neighborPos.z].SetPosition(neighborPos);
         _board[boardablePosition.x, boardablePosition.y, boardablePosition.z] = null;
         
         return true;
      }
   }
}
