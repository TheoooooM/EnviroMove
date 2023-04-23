using System.Collections.Generic;
using Levels;
using UnityEngine;

namespace Interfaces
{
    public interface IBoardable
    {
        public List<Enums.BlockTag> GetTags();
        
        public void SetOnBoard(Vector3Int boardPos, IBoard board);
        public void SetPosition(Vector3Int newBoardPos);

        bool TryMoveOn(IBoardable move, Enums.Side commingSide);

        public void StartBoard();
    }
}