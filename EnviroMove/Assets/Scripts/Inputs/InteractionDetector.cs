using System;
using Interfaces;
using UnityEngine;

namespace Inputs
{
    public class InteractionDetector : MonoBehaviour
    {
        public static InteractionDetector Instance;
        
        private IInteractable selectEntity;

        public bool isActive =true;

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            Inputs.Instance.OnTouch += TouchEffect;
            Inputs.Instance.OnRelease += ReleasetouchEffect;
            Inputs.Instance.OnSwip += SwipEffect;
        }

        void TouchEffect(Vector2 touchPos)
        {
            if (!isActive) return;
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                selectEntity = hit.transform.GetComponent<IInteractable>();
                if (selectEntity == null) return;
                if (selectEntity.IsInteractible()) selectEntity.Select();
                else selectEntity = null;
            }
        }

        void ReleasetouchEffect(Vector2 position)
        {
            if (!isActive) return;
            IBoardable boardable = null;
            Ray ray = Camera.main.ScreenPointToRay(position);
            if(Physics.Raycast(ray, out RaycastHit hit)) boardable = hit.transform.GetComponent<IBoardable>();
            selectEntity?.Deselect(boardable);
        }


        void SwipEffect(Enums.Side side)
        {
            if (isActive) selectEntity?.Swipe(side);
        }
    }
}
