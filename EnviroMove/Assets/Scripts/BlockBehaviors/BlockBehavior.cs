using System;
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

        public bool CanBlockInteract() => true;

        [SerializeField] protected float moveSpeed = 1f;
        private Coroutine _actionCoroutine;
    
        public virtual void SetOnBoard(Vector3Int boardPos, Enums.Side boardRotation, IBoard board)
        {
            this.boardPos = boardPos;
            if (boardRotation == Enums.Side.none) this.boardRotation = Enums.Side.forward; 
            else this.boardRotation = boardRotation;
            boardMaster = board;
        }

        public void SetPosition(Vector3Int newBoardPos)
        {
            boardPos = newBoardPos;
        }
        
        public virtual bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            return false;
        }

        public virtual bool CanMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            return false;
        }

        public virtual void MoveOn(IBoardable move, Vector3Int pos)
        { }

        public void AddOnFinishMove(Action<IBoardable> action)=>onMoveFinish += () => action?.Invoke(this);
        public void RemoveOnFinishMove(Action<IBoardable> action) => onMoveFinish -= () => action?.Invoke(this);

        public virtual void StartBoard() {
            InitAfterBeingPos();
        }

        public void Grab(Vector3 newPos, float speed = 0)
        {
            MoveToPoint(newPos, speed);
        }

        public void MoveToPoint(Vector3 newPos, float speed, bool instanteMove = false, bool finishMove = false)
        {
            if (speed == 0) speed = moveSpeed;
            _actionCoroutine = StartCoroutine(MoveToPosition(newPos, speed));
        }
        
        public void StopCoroutineAction()
        {
            if(_actionCoroutine != null)StopCoroutine(_actionCoroutine);
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

        protected virtual void InitAfterBeingPos() { }
    }
}
