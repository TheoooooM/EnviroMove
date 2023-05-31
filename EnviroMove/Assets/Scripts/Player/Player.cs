using System;
using System.Collections;
using System.Collections.Generic;
using Inputs;
using Interfaces;
using Levels;
using TMPro;
using UnityEngine;

delegate void basicDelegate();
public class Player : MonoBehaviour, IBoardable
{
    private IBoard _board;
    
    private Vector3Int _boardPos;
    private Vector3 _moveDir;

    private Enums.Side _lastDir = Enums.Side.forward;
    private bool _moving;
    private bool _isDead;
    private basicDelegate _onMoveFinish;

    [SerializeField] private int startDelay = 5;

    [SerializeField] private TextMeshProUGUI countDownText;
    
    [SerializeField] private float moveSpeed;
    private Coroutine _actionCoroutine;
    
    [SerializeField] protected List<Enums.BlockTag> tags;
    public List<Enums.BlockTag> GetTags() => tags;
    public bool CanBlockInteract() => !_isDead;

    [SerializeField] private Animator _animator;
    [SerializeField] private List<Material> skinMat = new();
    public Material GetMat(int id) => skinMat[id];


    private Vector3Int nextPos;

    private void Awake() {
        countDownText.gameObject.SetActive(false);
    }

    void Move()
    {
        _board.CheckCameraMovement(_boardPos);
        _board.CheckFinishLevel(_boardPos);
        var dir = _board.GetPlayerDirection(_boardPos);
        if (dir != Enums.Side.none) _lastDir = dir;
        if (_board.CanMove(_boardPos, _lastDir, true, out Vector3 movePosition, out nextPos))
        {
            transform.rotation = Quaternion.LookRotation(Enums.SideVector3(_lastDir), Vector3.up);
            _moving = true;
            _animator.SetTrigger("Walk");
            //_board.Move(this, nextPos);
            MoveToPoint(movePosition, moveSpeed, false, true);
        }
        else GameOver();
    }

    private void GameOver()
    {
        _animator.ResetTrigger("Walk");
        _animator.SetTrigger("Death");
        StopCoroutineAction();
        enabled = false;
    }

    public void CompleteGameOver()=>_isDead = true;

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

    public void MoveToPoint(Vector3 newPos, float speed, bool instanteMove = false, bool finishMove = false)
    {
        if (!instanteMove) _actionCoroutine = StartCoroutine(MoveToPosition(newPos, speed, finishMove));
        else transform.position = newPos;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator MoveToPosition(Vector3 newPos, float speed, bool finishMove = false)
    {
        yield return new WaitForEndOfFrame();
        var magnitude = Vector3.Distance(transform.position, newPos);
            var startMagnitude = magnitude;
            bool haveTransi = false;
            var step = speed * Time.deltaTime;
            while (magnitude> step)
            {
                step = speed * Time.deltaTime;
                transform.position += (newPos - transform.position).normalized * step;
                magnitude -= step;
                if (finishMove && magnitude <= startMagnitude / 2 && !haveTransi)
                {
                    haveTransi = true;
                    _board.Move(this, nextPos);
                    //Debug.Break();
                    nextPos = default;
                }
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

    public bool CanMoveOn(IBoardable move, Enums.Side commingSide, Vector3Int pos)
    {
        return false;
    }

    public void MoveOn(IBoardable move, Vector3Int pos)
    { }

    public void AddOnFinishMove(Action<IBoardable> action) => _onMoveFinish += () => action?.Invoke(this);
    public void RemoveOnFinishMove(Action<IBoardable> action) => _onMoveFinish -= () => action?.Invoke(this);

    public void StartBoard()
    {
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        InteractionDetector.Instance.isActive = false;
        countDownText.gameObject.SetActive(true);
        int countdown = startDelay;
        while (countdown>0)
        {
            countDownText.text = $"{countdown}";
            yield return new WaitForSeconds(1);
            countdown--;
        }
        countDownText.gameObject.SetActive(false);
        InteractionDetector.Instance.isActive = true;
        Move();
    }
}
