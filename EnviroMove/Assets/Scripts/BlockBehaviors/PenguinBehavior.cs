using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class PenguinBehavior : BlockBehavior, IInteractable
    {
        [SerializeField] private Animator _animator;
        [Header("VFX")]
        [SerializeField] private GameObject iceVFX;
        [SerializeField] private GameObject previewVFX;
        [Header("Sound")]
        [SerializeField] AudioClip slideSound ;
        
        private bool isInteractible = true;

        public bool IsInteractible() => isInteractible;

        public void Select()
        {
            previewVFX.SetActive(true);
        }

        public void Deselect(IBoardable releaseBoardable)
        {
            previewVFX.SetActive(false);
            
        }

        public void Swipe(Enums.Side side)
        {
            isInteractible = false;
            Slide(side);
        }

        void Slide(Enums.Side side)
        {
            transform.rotation = Quaternion.LookRotation(Enums.SideVector3(Enums.InverseSide(side)), Vector3.up);
            _animator.SetTrigger("Slide");
            m_Audio.PlaySound(slideSound);
            iceVFX.SetActive(false);
            Move(side);
            onMoveFinish += () => Move(side);
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