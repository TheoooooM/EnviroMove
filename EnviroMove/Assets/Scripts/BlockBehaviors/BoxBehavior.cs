using System;
using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class BoxBehavior : BlockBehavior, IInteractable
    {
        private Vector3 _startScale;
        bool isInteractible;
        
        [SerializeField] private float moveSpeed = 1f;

        private void Start()=> _startScale = transform.localScale;


        public bool IsInteractible() => isInteractible;
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
            var magnitude = Vector3.Distance(transform.position, newPos);
            var startMagnitude = magnitude;
            while (magnitude> moveSpeed)
            {
                Debug.Log($"Moving by {(newPos - transform.position).normalized * moveSpeed}");
                transform.position += (newPos - transform.position).normalized * moveSpeed* Time.deltaTime;
                magnitude -= moveSpeed*Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.position = newPos;
        }
    }
}