using Interfaces;

namespace BlockBehaviors
{
    public class BreakableBehavior : BlockBehavior, IInteractable
    {
        public void Select()
        {
            boardMaster.RemoveBoardable(this);
            Destroy(gameObject);
        }

        public void Deselect() { }

        public void Swipe(Enums.Side side) { }
    }
}