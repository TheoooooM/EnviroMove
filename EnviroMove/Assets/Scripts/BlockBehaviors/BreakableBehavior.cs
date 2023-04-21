using Interfaces;

namespace BlockBehaviors
{
    public class BreakableBehavior : BlockBehavior, IInteractable
    {
        private bool _isInteractible = true;


        public bool IsInteractible() => _isInteractible;
        public void Select()
        {
            boardMaster.RemoveBoardable(this);
            Destroy(gameObject);
        }

        public void Deselect() { }

        public void Swipe(Enums.Side side) { }
    }
}