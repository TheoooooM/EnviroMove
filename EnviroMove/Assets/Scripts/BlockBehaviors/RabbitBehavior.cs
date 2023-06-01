using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class RabbitBehavior : InteractiveBlockBehavior
    {
        [SerializeField] private Animator _animator;
        private bool tunnelSet;
        private Vector3Int secondPos;

        [SerializeField] private GameObject rabbitMesh;
        [SerializeField] private Collider rabbitCollider;
        [Space]
        [SerializeField] private GameObject hole1;
        [SerializeField] private GameObject hole2;
        [Space] 
        [SerializeField] private GameObject vfxDebug;
         [SerializeField] private GameObject vfxDig;

        private Dictionary<IBoardable, Vector3Int> boardableEnters = new();

        private void Start()
        {
            vfxDebug.SetActive(false);
            //vfxDig.SetActive(false);
        }

        public override void Select()
        {
            vfxDebug.SetActive(true);
            Debug.Log("Select Rabbit");
        }

        public Vector3Int tempPos;
        public override void Deselect(IBoardable releaseBoardable)
        {
            vfxDebug.SetActive(false);
            if (releaseBoardable == null || releaseBoardable == (IBoardable)this) return;
            Vector3Int pos = boardMaster.GetPosition(releaseBoardable);
            if (!(pos.x == boardPos.x || pos.z == boardPos.z)) return;
            var topblock = boardMaster.GetNeighbor(pos, Enums.Side.up, out _);
            if (topblock == null)
            {
                Debug.Log("Dig");
                vfxDig.SetActive(true);
                _animator.SetTrigger("Dig");
                tempPos = pos + Vector3Int.up;
                CreateTunnel(tempPos);
                
            }
        }

        public void CreateTunnel(Vector3Int tunnelPos)
        {
            hole2.transform.position = boardMaster.GetWorldPos(tunnelPos) + hole2.transform.localPosition;
            hole1.SetActive(true);
            hole2.SetActive(true);
            //vfxDig.SetActive(false);
            rabbitCollider.enabled = false;
            boardMaster.SetAt(this, tunnelPos);


            secondPos = tunnelPos;

            rabbitMesh.SetActive(false);
            isInteractable = false;
            tunnelSet = true;
            tempPos = default;
        }

        public override void Swipe(Enums.Side side) { }

        public override bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            if (!tunnelSet) return base.TryMoveOn(move, commingSide, pos);
            else if (move.GetTags().Contains(Enums.BlockTag.NoTunnel)) return false;
            Debug.Log("TRy Move On Rabbit");
            MoveOn(move, pos);

            return true;
        }

        public override bool CanMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            if (!tunnelSet) return base.CanMoveOn(move, commingSide, pos);
            else if (move.GetTags().Contains(Enums.BlockTag.NoTunnel)) return false;
            else return true;
        }

        public override void MoveOn(IBoardable move, Vector3Int pos)
        {
            boardableEnters.Add(move, pos);
            move.AddOnFinishMove(Teleport);
        }

        void Teleport(IBoardable boardable)
        {
            boardable.RemoveOnFinishMove(Teleport);
            Vector3Int newPos = boardableEnters[boardable] == boardPos ? secondPos : boardPos;
            boardable.MoveToPoint(boardMaster.GetWorldPos(newPos), 0, true);
            boardable.SetPosition(newPos);
            boardMaster.SetAt(boardable, newPos);
        }
    }
}