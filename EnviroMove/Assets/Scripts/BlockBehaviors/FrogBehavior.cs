using System.Collections;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class FrogBehavior : InteractiveBlockBehavior
    {

        [SerializeField] private GameObject tongue;
        [SerializeField] private float tongueSpeed = 3;
        private Vector3Int tonguePos;
        private basicDelegate onTongueMoveFinish;

        private Vector3Int sideBoardPosition;
        private bool sideSet;
        
        
        public override void Select()
        {
            if(!onCD)
            {
                isInteractable = false;
                tonguePos = boardPos;
                onTongueMoveFinish += Grab;
                tongue.SetActive(true);
                StartCoroutine(Cooldown());
                Grab();
            }
            
        }

        public override void Deselect(IBoardable releaseBoardable){}
        public override void Swipe(Enums.Side side) {}

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
            else if(boardLimit) WithdrawTongue();
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
        }
    }
}