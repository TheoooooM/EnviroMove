using Interfaces;

namespace BlockBehaviors
{
    public class FrogBehavior : BlockBehavior, IInteractable
    {
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
            boardMaster.GetNeighbor(boardPos,)
        }
    }
}