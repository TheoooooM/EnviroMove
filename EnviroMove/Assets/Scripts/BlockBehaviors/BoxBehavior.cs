using System;
using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class BoxBehavior : BlockBehavior, IInteractable
    {
        private Vector3 _startScale;
        private float moveSpeed = .1f;

        private void Start()=> _startScale = transform.localScale;

        public void Select()=>transform.localScale = _startScale * 1.2f;
        public void Deselect()=>transform.localScale = _startScale;


        public void Swipe(Enums.Side side)
        {
            if (boardMaster.TryMove(boardPos, side, out Vector3 newPos))
            {
                StartCoroutine(MoveToPosition(newPos));
            }
        }

        private IEnumerator MoveToPosition(Vector3 newPos)
        {
            while (Vector3.Distance(transform.position, newPos)< moveSpeed)
            {
                Debug.Log("Moving");
                transform.position = (newPos - transform.position).normalized * moveSpeed;
                yield return new WaitForEndOfFrame();
            }
            transform.position = newPos;
        }
    }
}