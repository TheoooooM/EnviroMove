using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class RabbitBehavior : InteractiveBlockBehavior
    {
        private bool tunnelSet;
        private Vector3Int secondPos;

        [SerializeField] private MeshRenderer rabbitMesh;
        [Space]
        [SerializeField] private GameObject hole1;
        [SerializeField] private GameObject hole2;

        private Dictionary<IBoardable, Vector3Int> boardableEnters = new();

        public override void Select() { }

        public override void Deselect(IBoardable releaseBoardable)
        {
            if (releaseBoardable == null || releaseBoardable == (IBoardable)this) return;
            Vector3Int pos = boardMaster.GetPosition(releaseBoardable);
            var topblock = boardMaster.GetNeighbor(pos, Enums.Side.up, out _);
            if (topblock == null) CreateTunnel(pos + Vector3Int.up);
        }

        void CreateTunnel(Vector3Int tunnelPos)
        {
            boardMaster.SetAt(this, tunnelPos);
            hole1.SetActive(true);
            hole2.SetActive(true);
            secondPos = tunnelPos;
            hole2.transform.position = boardMaster.GetWorldPos(tunnelPos) + hole2.transform.localPosition;
            rabbitMesh.enabled = false;
            isInteractable = false;
            tunnelSet = true;
        }

        public override void Swipe(Enums.Side side) { }

        public override bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            if (!tunnelSet) return base.TryMoveOn(move, commingSide, pos);
            
            boardableEnters.Add(move, pos);
            move.AddOnFinishMove(Teleport);

            return true;

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