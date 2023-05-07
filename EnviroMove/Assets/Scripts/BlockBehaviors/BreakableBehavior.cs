using Interfaces;

namespace BlockBehaviors
{
    public class BreakableBehavior : InteractiveBlockBehavior
    {
        private bool _isInteractible = true;


        public bool IsInteractible() => _isInteractible;
        public override void Select()
        {
            boardMaster.RemoveBoardable(this);
            Destroy(gameObject);
        }

        public override void Deselect(IBoardable releaseBoardable){}

        public override void Swipe(Enums.Side side) {}
    }
}