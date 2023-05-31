using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    
    public class BoxBehavior : InteractiveBlockBehavior
    {
        private Vector3 _startScale;
        bool isInteractible = true;
        [SerializeField] private List<GameObject> directionFX = new();
        [SerializeField] private GameObject vfxDebug;
        [SerializeField] private Transform mesh;


        private void Start()
        {
            vfxDebug.SetActive(false);
            _startScale = mesh.localScale;
            onMoveFinish += () => {
                isInteractible = true;
                ChangeActivatedFX();
                UpdateNeighboor(boardPos);
            };
        }

        /// <summary>
        /// Init the block when all the board is set
        /// </summary>
        protected override void InitAfterBeingPos() {
            ChangeActivatedFX();
        }


        public bool IsInteractible() {
            return isInteractible;
        }

        public override void Select()
        {
            mesh.localScale = _startScale * 1.2f;
            vfxDebug.SetActive(true);
        }

        public override void Deselect(IBoardable releaseBoardable)
        {
            mesh.localScale = _startScale;
            vfxDebug.SetActive(false);
        }


        public override void Swipe(Enums.Side side)
        {
            isInteractible = false;
            Vector3Int lastBoardPos = boardPos;
            if (boardMaster.TryMove(boardPos, side, out Vector3 newPos)) {
                UpdateNeighboor(lastBoardPos);
                StartCoroutine(MoveToPosition(newPos, moveSpeed));
            }
        }


        private Vector3 newPos = new();
        private Vector3Int newBoardPos = new();
        private bool limitBoard = false;
        /// <summary>
        /// Change the FX which are activated
        /// </summary>
        public void ChangeActivatedFX() {
            directionFX[0].SetActive(boardMaster.CanMove(boardPos, Enums.Side.forward, false, out newPos, out newBoardPos));
            directionFX[1].SetActive(boardMaster.CanMove(boardPos, Enums.Side.right, false,out newPos, out newBoardPos));
            directionFX[2].SetActive(boardMaster.CanMove(boardPos, Enums.Side.back, false,out newPos, out newBoardPos));
            directionFX[3].SetActive(boardMaster.CanMove(boardPos, Enums.Side.left, false,out newPos, out newBoardPos));
        }

        private void UpdateNeighboor(Vector3Int pos) {
            if (boardMaster.GetNeighbor(pos, Enums.Side.forward, out limitBoard) is BoxBehavior) {
                (boardMaster.GetNeighbor(pos, Enums.Side.forward, out limitBoard) as BoxBehavior).ChangeActivatedFX();
            }
            if (boardMaster.GetNeighbor(pos, Enums.Side.right, out limitBoard) is BoxBehavior) {
                (boardMaster.GetNeighbor(pos, Enums.Side.right, out limitBoard) as BoxBehavior).ChangeActivatedFX();
            }
            if (boardMaster.GetNeighbor(pos, Enums.Side.back, out limitBoard) is BoxBehavior) {
                (boardMaster.GetNeighbor(pos, Enums.Side.back, out limitBoard) as BoxBehavior).ChangeActivatedFX();
            }
            if (boardMaster.GetNeighbor(pos, Enums.Side.left, out limitBoard) is BoxBehavior) {
                (boardMaster.GetNeighbor(pos, Enums.Side.left, out limitBoard) as BoxBehavior).ChangeActivatedFX();
            }
        }
    }
}