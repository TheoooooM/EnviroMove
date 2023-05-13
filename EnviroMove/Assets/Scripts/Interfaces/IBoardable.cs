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

        bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos);

        void AddOnFinishMove(Action<IBoardable> action);
        void RemoveOnFinishMove(Action<IBoardable> action);

        void StopCoroutineAction();
        public void Grab(Vector3 newPos, float speed = 0);
        public void MoveToPoint(Vector3 newPos, float speed = 0, bool InstanteMove = false);

        public void StartBoard();
    }
}