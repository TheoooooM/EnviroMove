using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Levels;
using UnityEngine;

public class BlockBehavior : MonoBehaviour, IBoardable
{
    protected Vector3Int boardPos;
    protected Level boardMaster;
    
    public virtual void SetOnBoard(Vector3Int boardPos, Level board)
    {
        this.boardPos = boardPos;
        boardMaster = board;
    }
}
