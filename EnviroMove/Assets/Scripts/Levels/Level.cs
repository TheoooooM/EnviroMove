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
   public class Level : MonoBehaviour
   {
      public LevelDelegate onFinishGenerate;
      
      
      public LevelData levelData;
      private IBoardable[,,] _board;
      private GameObject[] _blocksUsed;

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
               if(data.blocksUsed[i] == null) continue;  
               LoadAssetWithCallbackIndexed<GameObject>(data.blocksUsed[i], (obj, index) => { _blocksUsed[index] = obj; waitCount--; }, i);
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
                  Debug.Log($"currentPos: {currentPos}");
                  Debug.Log($"blockGrid: {data.blockGrid[currentPos.x, currentPos.y, currentPos.z]}");
                  Debug.Log($"Block use: {_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]]}");
                  if(_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]] ==null ) continue;
                  GameObject currentGo = Instantiate(_blocksUsed[data.blockGrid[currentPos.x, currentPos.y, currentPos.z]],
                        transform.position + currentPos, Quaternion.identity, transform);
                  IBoardable currentBoardable = currentGo.GetComponent<IBoardable>();
                  if (currentBoardable == null) throw new MissingMemberException($"{currentGo.name} isn't Boardable");
                  currentBoardable.SetOnBoard(currentPos, this);
               }
            }
         }
         
         onFinishGenerate?.Invoke();
      }

      IBoardable GetNeighbor(Vector2Int position, Enums.Side side)
      {
         throw new System.NotImplementedException();
      }

      bool TryMove(IBoardable sender, Enums.Side side)
      {
         throw new System.NotImplementedException();
      }
   }
}
