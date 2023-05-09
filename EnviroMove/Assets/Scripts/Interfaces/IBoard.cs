using UnityEngine;

namespace Interfaces
{
    public interface IBoard
    {
        public IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side, out bool boardLimit);
        public IBoardable GetNeighbor(Vector3Int boardPos, Enums.Side side, out bool boardLimit, out Vector3Int neighborPos);
        public Vector3Int GetPosition(IBoardable boardable);
        public bool TryMove(Vector3Int boardablePosition, Enums.Side side, out Vector3 position);
        public void Move(IBoardable boardable, Vector3Int position);
        public void SetAt(IBoardable boardable, Vector3Int position);
        public void RemoveBoardable(IBoardable boardable);

        Vector3 GetWorldPos(Vector3Int boardPos);

        Enums.Side GetPlayerDirection(Vector3Int pos);
        void CheckCameraMovement(Vector3Int pos);
        void CheckFinishLevel(Vector3Int pos);

        void FinishLevel();
    }
}