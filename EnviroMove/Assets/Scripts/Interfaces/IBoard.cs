using UnityEngine;

namespace Interfaces
{
    public interface IBoard
    {
        public IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side);
        public bool TryMove(Vector3Int boardablePosition, Enums.Side side, out Vector3 position);
        public void RemoveBoardable(IBoardable boardable);

        Enums.Side GetPlayerDirection(Vector3Int pos);
    }
}