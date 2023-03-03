using Interfaces;
using UnityEngine;

namespace Levels
{
   public class Level : MonoBehaviour
   {
      public LevelData levelData;
      private IBoardable[,,] board;
      private GameObject[] blocksUsed;

      void LoadBlocks()
      {
         throw new System.NotImplementedException();
      }


      public void GenerateLevel(LevelData data)
      {
         blocksUsed = new GameObject[data.blocksUsed.Length];

         for (int i = 0; i < data.blocksUsed.Length; i++)
         {
            //blocksUse[i] = 
         }
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
