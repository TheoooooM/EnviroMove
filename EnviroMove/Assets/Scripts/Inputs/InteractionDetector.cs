using Interfaces;
using UnityEngine;

namespace Inputs
{
    public class InteractionDetector : MonoBehaviour
    {
        private IInteractable selectEntity;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Init Interaction callBacks");
            Inputs.Instance.OnTouch += TouchEffect;
            Inputs.Instance.OnRelease += ReleasetouchEffect;
            Inputs.Instance.OnSwip += SwipEffect;
        }

        void TouchEffect(Vector2 touchPos)
        {
            Debug.Log("Touch Screen");
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                selectEntity = hit.transform.GetComponent<IInteractable>();
                selectEntity?.Select();
            }
        }

        void ReleasetouchEffect(Vector2 position)=>selectEntity?.Deselect();

        void SwipEffect(Enums.Side side)=> selectEntity?.Swipe(side);
    }
}
