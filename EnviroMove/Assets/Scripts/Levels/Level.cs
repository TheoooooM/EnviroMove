using Interfaces;
using UnityEngine;

namespace Levels
{
   public class Level : MonoBehaviour
   {
      public LevelData levelData;
      private IBoardable[,,] board;
      private GameObject[] blocksUse;

      void LoadBlocks()
      {
         throw new System.NotImplementedException();
      }

      public void GenerateLevel()
      {
         throw new System.NotImplementedException();
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
