using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Levels;
using UnityEngine;

delegate void basicDelegate();
public class Player : MonoBehaviour, IBoardable
{
    private IBoard _board;
    
    private Vector3Int _boardPos;
    private Vector3 _moveDir;

    private Enums.Side _lastDir = Enums.Side.forward;
    private bool _moving;
    private basicDelegate _onMoveFinish;

    [SerializeField] private float startDelay = 2;
    
    [SerializeField] private float moveSpeed;
    private Coroutine _actionCoroutine;
    
    [SerializeField] protected List<Enums.BlockTag> tags;
    public List<Enums.BlockTag> GetTags() => tags;

    [SerializeField] private Animator _animator;


    

    void Move()
    {
        _board.CheckCameraMovement(_boardPos);
        _board.CheckFinishLevel(_boardPos);
        var dir = _board.GetPlayerDirection(_boardPos);
        if (dir != Enums.Side.none) _lastDir = dir;
        if (_board.TryMove(_boardPos, _lastDir, out Vector3 movePosition))
        {
            _moving = true;
            _animator.SetTrigger("Walk");
            MoveToPoint(movePosition, moveSpeed);
        }
        else GameOver();
    }

    private void GameOver()
    {
        _animator.SetTrigger("Death");
        StopCoroutineAction();
        enabled = false;
    }

    public void AsyncGameOver()
    {
        _board.FinishLevel();
        Destroy(gameObject);
    }
    

    public void StopCoroutineAction()
    {
        if(_actionCoroutine != null)StopCoroutine(_actionCoroutine);
    }

    public void Grab(Vector3 newPos, float speed = 0)
    {
        _animator.SetTrigger("Grab");
        MoveToPoint(newPos, speed);
    }

    public void MoveToPoint(Vector3 newPos, float speed, bool instanteMove = false)
    {
        if (!instanteMove) _actionCoroutine = StartCoroutine(MoveToPosition(newPos, speed));
        else transform.position = newPos;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator MoveToPosition(Vector3 newPos, float speed)
    {
        var magnitude = Vector3.Distance(transform.position, newPos);
            var startMagnitude = magnitude;
            var step = speed * Time.deltaTime;
            while (magnitude> step)
            {
                //Debug.Log($"Moving by {(newPos - transform.position).normalized * step}");
                transform.position += (newPos - transform.position).normalized * step;
                magnitude -= step;
                yield return new WaitForEndOfFrame();
            }
            transform.position = newPos;
            _moving = false;
            _actionCoroutine = null;
            _onMoveFinish?.Invoke();
            _onMoveFinish = null;
            if(_actionCoroutine == null) Move();
    }
    public void SetOnBoard(Vector3Int boardPos, Enums.Side boardRotation, IBoard board)
    {
        _boardPos = boardPos;
        _board = board;
    }

    public void SetPosition(Vector3Int newBoardPos)
    {
        _boardPos = newBoardPos;
        
    }

    public bool TryMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
    {
        return false;
    }

    public void AddOnFinishMove(Action<IBoardable> action) => _onMoveFinish += () => action?.Invoke(this);
    public void RemoveOnFinishMove(Action<IBoardable> action) => _onMoveFinish -= () => action?.Invoke(this);

    public void StartBoard()
    {
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(startDelay);
        Move();
    }
}
