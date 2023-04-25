using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public abstract class InteractiveBlockBehavior : BlockBehavior , IInteractable
    {
        [SerializeField] private float cooldown = 1;
        protected bool onCD;
        
        public abstract bool IsInteractible();

        public abstract void Select();

        public abstract void Deselect();

        public abstract void Swipe(Enums.Side side);

        protected IEnumerator Cooldown()
        {
            onCD = true;
            yield return new WaitForSeconds(cooldown);
            onCD = false;
        }
    }
}