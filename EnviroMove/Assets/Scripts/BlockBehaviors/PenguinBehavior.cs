using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class PenguinBehavior : BlockBehavior, IInteractable
    {
        private bool isInteractible = true;

        public bool IsInteractible() => isInteractible;
        
        public void Select() { }
        public void Deselect(IBoardable releaseBoardable) { }

        public void Swipe(Enums.Side side)
        {
            isInteractible = false;
            Slide(side);
        }

        void Slide(Enums.Side side)
        {
            Debug.Log("Slide");
           Move(side);
           onMoveFinish += ()=>Move(side);
        }

        void Move(Enums.Side side)
        {
            Debug.Log("Move");
            if (boardMaster.TryMove(boardPos, side, out Vector3 newPos))
            {
                StartCoroutine(MoveToPosition(newPos, moveSpeed));
            }
            else
            {
                onMoveFinish -= () => Move(side);
                boardMaster.RemoveBoardable(this);
                Destroy(gameObject);
            }
        }
    }
}