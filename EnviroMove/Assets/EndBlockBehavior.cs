using System.Collections;
using System.Collections.Generic;
using BlockBehaviors;
using Interfaces;
using UnityEngine;

public class EndBlockBehavior : BlockBehavior
{
    public override bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
    {
        return true;
    }

    public override bool CanMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
    {
        return true;
    }
}
