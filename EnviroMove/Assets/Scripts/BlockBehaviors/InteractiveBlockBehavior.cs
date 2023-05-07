using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public abstract class InteractiveBlockBehavior : BlockBehavior , IInteractable
    {
        protected bool isInteractable = true;
        
        [SerializeField] private float cooldown = 1;
        protected bool onCD;

        public virtual bool IsInteractible() => isInteractable;

        public abstract void Select();

        public abstract void Deselect(IBoardable releaseBoardable);

        public abstract void Swipe(Enums.Side side);

        protected IEnumerator Cooldown()
        {
            onCD = true;
            yield return new WaitForSeconds(cooldown);
            onCD = false;
        }
    }
}