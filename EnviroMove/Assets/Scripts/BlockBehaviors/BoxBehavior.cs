using System;
using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    
    public class BoxBehavior : BlockBehavior, IInteractable
    {
        private Vector3 _startScale;
        bool isInteractible = true;
        
        

        private void Start()
        {
            _startScale = transform.localScale;
            onMoveFinish += ()=>isInteractible = true;
        }


        public bool IsInteractible() => isInteractible;
        public void Select()=>transform.localScale = _startScale * 1.2f;
        public void Deselect()=>transform.localScale = _startScale;


        public void Swipe(Enums.Side side)
        {
            isInteractible = false;
            if (boardMaster.TryMove(boardPos, side, out Vector3 newPos))
            {
                StartCoroutine(MoveToPosition(newPos));
            }
        }

        
    }
}