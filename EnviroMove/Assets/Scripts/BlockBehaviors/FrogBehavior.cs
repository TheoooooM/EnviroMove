using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class FrogBehavior : BlockBehavior, IInteractable
    {

        [SerializeField] private GameObject tongue;
        [SerializeField] private float tongueSpeed = 3;
        private Vector3Int tonguePos;
        private basicDelegate onTongueMoveFinish;

        private Vector3Int sideBoardPosition;
        private bool sideSet;
        
        private bool isInteractable = true;

        public bool IsInteractible() => isInteractable; 
        
        public void Select()
        {
            isInteractable = false;
            tonguePos = boardPos;
            onTongueMoveFinish += Grab;
            tongue.SetActive(true);
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
            var neighboor = boardMaster.GetNeighbor(tonguePos,boardRotation, out bool boardLimit, out Vector3Int neightboorPos);
            var neightborWorldPos = boardMaster.GetWorldPos(neightboorPos);
            if (!sideSet)
            {
                sideBoardPosition = neightboorPos;
                sideSet = true;
            }
            if (neighboor != null)
            {
                var tags = neighboor.GetTags();
                if (tags.Contains(Enums.BlockTag.FrogGrabbable))
                {
                    neighboor.StopCoroutineAction();
                    neighboor.MoveToPoint(boardMaster.GetWorldPos(sideBoardPosition), tongueSpeed);
                    boardMaster.Move(neighboor, sideBoardPosition);
                    WithdrawTongue();
                }
                else WithdrawTongue();
            }
            else
            {
                tonguePos = neightboorPos;
                ExpandTongue(neightborWorldPos);
            }

        }

        void ExpandTongue(Vector3 position)
        {
            StartCoroutine(MoveTongue(position));
        }

        void WithdrawTongue()
        {
            onTongueMoveFinish -= Grab;
            onTongueMoveFinish += ResetTongue;
            StartCoroutine(MoveTongue(transform.position));
            
        }

        private void ResetTongue()
        {
            sideBoardPosition = Vector3Int.zero;
            sideSet = false;
            isInteractable = true;
            tonguePos = boardPos;
            tongue.SetActive(false);
            onTongueMoveFinish -= ResetTongue;
        }


        private int tongueMoveCount = 0;
        IEnumerator MoveTongue(Vector3 position)
        {
            int index = tongueMoveCount;
            Debug.Log($"Start move tongue n°{index}");
            index++;
            var magnitude = Vector3.Distance(tongue.transform.position, position);
            var step = tongueSpeed * Time.deltaTime;
            while (magnitude> step)
            {
                tongue.transform.position += (position - tongue.transform.position).normalized * step;
                magnitude -= step;
                yield return new WaitForEndOfFrame();
            }
            tongue.transform.position = position;
            onTongueMoveFinish?.Invoke();
            Debug.Log($"End move tongue n°{index}");
        }
    }
}