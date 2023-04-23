using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public delegate void BlockDelegate();
    public class BlockBehavior : MonoBehaviour, IBoardable
    {
        protected Vector3Int boardPos;
        protected IBoard boardMaster;

        protected BlockDelegate onMoveFinish;

        [SerializeField] protected List<Enums.BlockTag> tags;
        public List<Enums.BlockTag> GetTags() => tags;
        
        [SerializeField] private float moveSpeed = 1f;
    
        public virtual void SetOnBoard(Vector3Int boardPos, IBoard board)
        {
            this.boardPos = boardPos;
            boardMaster = board;
        }

        public void SetPosition(Vector3Int newBoardPos)
        {
            boardPos = newBoardPos;
        }

        public virtual bool TryMoveOn(IBoardable move, Enums.Side commingSide)
        {
            return false;
        }

        public void StartBoard()
        { }

        protected IEnumerator MoveToPosition(Vector3 newPos)
        {
            var magnitude = Vector3.Distance(transform.position, newPos);
            var startMagnitude = magnitude;
            var step = moveSpeed * Time.deltaTime;
            while (magnitude> step)
            {
                //Debug.Log($"Moving by {(newPos - transform.position).normalized * step}");
                transform.position += (newPos - transform.position).normalized * step;
                magnitude -= step;
                yield return new WaitForEndOfFrame();
            }
            transform.position = newPos;
            onMoveFinish?.Invoke();
        }
    }
}
