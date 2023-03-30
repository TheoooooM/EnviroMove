using Levels;
using UnityEngine;

namespace Interfaces
{
    public interface IBoardable
    {
        void SetOnBoard(Vector3Int boardPos, Level board);
    }
}