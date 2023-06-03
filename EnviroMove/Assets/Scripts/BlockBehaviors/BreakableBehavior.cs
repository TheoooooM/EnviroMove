using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class BreakableBehavior : InteractiveBlockBehavior
    {
        private bool _isInteractible = true;

        [SerializeField] private GameObject breakVFX;
        [SerializeField] private GameObject renderer;


        public bool IsInteractible() => _isInteractible;
        public override void Select()
        {
            boardMaster.RemoveBoardable(this);
            
            renderer?.SetActive(false);
            breakVFX?.SetActive(true);
        }

        public override void Deselect(IBoardable releaseBoardable){}

        public override void Swipe(Enums.Side side) {}
    }
}