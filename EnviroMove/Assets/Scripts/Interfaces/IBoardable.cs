using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IBoardable
    {
        public List<Enums.BlockTag> GetTags();
        
        public void SetOnBoard(Vector3Int boardPos, Enums.Side boardRotation, IBoard board);
        public void SetPosition(Vector3Int newBoardPos);

        bool TryMoveOn(IBoardable move, Enums.Side commingSide);

        void StopCoroutineAction();
        public void MoveToPoint(Vector3 newPos, float speed = 0);

        public void StartBoard();
    }
}