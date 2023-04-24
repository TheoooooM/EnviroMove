using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

delegate void basicDelegate();
public class Player : MonoBehaviour, IBoardable
{
    private IBoard _board;
    
    private Vector3Int _boardPos;
    private Vector3 _moveDir;

    private Enums.Side _lastDir = Enums.Side.top;
    private bool _moving;
    private basicDelegate _onMoveFinish;
    
    [SerializeField] private float moveSpeed;
    
    [SerializeField] protected List<Enums.BlockTag> tags;
    public List<Enums.BlockTag> GetTags() => tags;


    

    void Move()
    {
        var dir = _board.GetPlayerDirection(_boardPos);
        if (dir != Enums.Side.none) _lastDir = dir;
        if (_board.TryMove(_boardPos, _lastDir, out Vector3 movePosition))
        {
            _moving = true;
            StartCoroutine(MoveToPoint(movePosition));
        }
    }
   

    IEnumerator MoveToPoint(Vector3 newPos)
    {
        var magnitude = Vector3.Distance(transform.position, newPos);
            var startMagnitude = magnitude;
            var step = moveSpeed * Time.deltaTime;
            while (magnitude> step)
            {
                //Debug.Log($"Moving by {(newPos - transform.position).normalized * step}");
                transform.position += (newPos - transform.position).normalized * step;
                magnitude -= step;
                yield return new WaitForEndOfFrame();
            }
            transform.position = newPos;
            _moving = false;
            _onMoveFinish?.Invoke();
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
        Move();
        _onMoveFinish += Move;
    }
}
