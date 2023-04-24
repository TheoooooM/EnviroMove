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
        protected Enums.Side boardRotation;
        protected IBoard boardMaster;

        protected BlockDelegate onMoveFinish;

        [SerializeField] protected List<Enums.BlockTag> tags;
        public List<Enums.BlockTag> GetTags() => tags;
        
        [SerializeField] protected float moveSpeed = 1f;
        private Coroutine _actionCoroutine;
    
        public virtual void SetOnBoard(Vector3Int boardPos, Enums.Side boardRotation, IBoard board)
        {
            this.boardPos = boardPos;
            this.boardRotation = boardRotation;
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

        public void MoveToPoint(Vector3 newPos, float speed)
        {
            _actionCoroutine = StartCoroutine(MoveToPosition(newPos, moveSpeed));
        }
        
        public void StopCoroutineAction()
        {
            StopCoroutine(_actionCoroutine);
        }
        
        public IEnumerator MoveToPosition(Vector3 newPos, float speed)
        {
            var magnitude = Vector3.Distance(transform.position, newPos);
            var startMagnitude = magnitude;
            var step = speed * Time.deltaTime;
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
