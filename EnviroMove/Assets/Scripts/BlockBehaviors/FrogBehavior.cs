using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class FrogBehavior : BlockBehavior, IInteractable
    {

        [SerializeField] private GameObject tongue;
        [SerializeField] private float tongueSpeed = 3;
        private basicDelegate onTongueMoveFinish;
        
        private bool isInteractable;

        public bool IsInteractible() => isInteractable;
        
        public void Select()
        {
            Grab();
        }
        public void Deselect()
        {
            //throw new System.NotImplementedException();
        }
        public void Swipe(Enums.Side side)
        {
            //throw new System.NotImplementedException();
        }


        void Grab()
        {
           var neighboor = boardMaster.GetNeighbor(boardPos,boardRotation, out bool boardLimit, out Vector3Int neightboorPos);
            var neightborWorldPos = boardMaster.GetWorldPos(neightboorPos);
            
        }

        void ExpandTongue(Vector3 position)
        {
            StartCoroutine(MoveTongue(position));
        }

        void WithdrawTongue()
        {
            StartCoroutine(MoveTongue(transform.position));
        }


        IEnumerator MoveTongue(Vector3 position)
        {
            var magnitude = Vector3.Distance(transform.position, position);
            var step = tongueSpeed * Time.deltaTime;
            while (magnitude> step)
            {
                transform.position += (position - transform.position).normalized * step;
                magnitude -= step;
                yield return new WaitForEndOfFrame();
            }
            transform.position = position;
            onTongueMoveFinish?.Invoke();
        }
    }
}