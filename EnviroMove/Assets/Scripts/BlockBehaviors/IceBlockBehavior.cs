using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace BlockBehaviors
{
    public class IceBlockBehavior : BlockBehavior
    {
        public override bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
        {
            List<Enums.BlockTag> moverTags = move.GetTags();
            if (moverTags.Contains(Enums.BlockTag.Penguin))
            {
                boardMaster.RemoveBoardable(this);
                Destroy(gameObject);
                return true;
            }
            return false;
        }
    }
}