using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

delegate void basicDelegate();
public class Player : MonoBehaviour, IBoardable
{
    private IBoard _board;
    
    private Vector3Int _boardPos;
    private Enums.Side _lookDir = Enums.Side.top;
    private Vector3 _moveDir;

    private bool moving;
    
    [SerializeField] private float speed;
    
    [SerializeField] protected List<Enums.BlockTag> tags;
    public List<Enums.BlockTag> GetTags() => tags;


    void UpdateAction()
    {
        if (!moving)
        {
            if (_board.TryMove(_boardPos, _lookDir, out Vector3 movePosition))
            {
                moving = true;
                StartCoroutine(MoveToPoint(movePosition));
            }
        }
    }
   

    IEnumerator MoveToPoint(Vector3 position)
    {
        while (Vector3.Distance(transform.position, position)< speed)
        {
            transform.position += _moveDir*speed;
            yield return new WaitForEndOfFrame();
        }
        moving = false;
    }
    public void SetOnBoard(Vector3Int boardPos, IBoard board)
    {
        _boardPos = boardPos;
        _board = board;
    }

    public void SetPosition(Vector3Int newBoardPos)
    {
        _boardPos = newBoardPos;
    }

    public bool TryMoveOn(IBoardable move, Enums.Side commingSide)
    {
        return false;
    }

    public void StartBoard()
    {
        //UpdateAction();
    }
}
