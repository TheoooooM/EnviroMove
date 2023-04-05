using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Levels;
using UnityEngine;

public class BlockBehavior : MonoBehaviour, IBoardable
{
    protected Vector3Int boardPos;
    protected IBoard boardMaster;
    
    public virtual void SetOnBoard(Vector3Int boardPos, IBoard board)
    {
        this.boardPos = boardPos;
        boardMaster = board;
    }

    public void SetPosition(Vector3Int newBoardPos)
    {
        boardPos = newBoardPos;
    }

    public void StartBoard()
    { }
}
