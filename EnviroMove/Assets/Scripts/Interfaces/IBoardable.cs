using Levels;
using UnityEngine;

namespace Interfaces
{
    public interface IBoardable
    {
        public void SetOnBoard(Vector3Int boardPos, IBoard board);
        public void SetPosition(Vector3Int newBoardPos);

        public void StartBoard();
    }
}