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
        [Space]
        [SerializeField] private GameObject hole1;
        [SerializeField] private GameObject hole2;

        private Dictionary<IBoardable, Vector3Int> boardableEnters = new();

        public override void Select()
        {
            Debug.Log("Select Rabbit");
        }

        public Vector3Int tempPos;
        public override void Deselect(IBoardable releaseBoardable)
        {
            if (releaseBoardable == null || releaseBoardable == (IBoardable)this) return;
            Vector3Int pos = boardMaster.GetPosition(releaseBoardable);
            var topblock = boardMaster.GetNeighbor(pos, Enums.Side.up, out _);
            if (topblock == null)
            {
                Debug.Log("Dig");
                _animator.SetTrigger("Dig");
                tempPos = pos + Vector3Int.up;
            }
        }

        public void CreateTunnel(Vector3Int tunnelPos)
        {
            boardMaster.SetAt(this, tunnelPos);
            hole1.SetActive(true);
            hole2.SetActive(true);
            
            secondPos = tunnelPos;
            hole2.transform.position = boardMaster.GetWorldPos(tunnelPos) + hole2.transform.localPosition;
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