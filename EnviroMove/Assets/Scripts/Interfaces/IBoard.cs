using UnityEngine;

namespace Interfaces
{
    public interface IBoard
    {
        public bool TryMove(Vector3Int boardablePosition, Enums.Side side, out Vector3 position);
    }
}