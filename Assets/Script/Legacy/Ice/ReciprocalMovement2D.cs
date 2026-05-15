using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TwoPointReciprocalMove2D : MonoBehaviour
{

    public bool isHorizontalMove = true;
    public bool isVerticalMove = false;


    public Transform pointA;
    public Transform pointB;


    [Range(0.1f, 20f)] public float moveSpeed = 1f;

    private Transform _currentTarget;
    private Rigidbody2D _rb2D;
    private const float _threshold = 0.01f;
    private bool _isMoveValid = true;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _rb2D ??= gameObject.AddComponent<Rigidbody2D>();
        _rb2D.bodyType = RigidbodyType2D.Kinematic;

        _currentTarget = pointB;
        
        CheckMovePointsValid();
    }

    private void Update()
    {
        if (isHorizontalMove && isVerticalMove)
        {
            isVerticalMove = false;
        }

        CheckMovePointsValid();
        if (!_isMoveValid) return;

        MoveBetweenTwoPoints();
    }

    private void CheckMovePointsValid()
    {
        if (pointA == null || pointB == null)
        {
            _isMoveValid = false;
            return;
        }
        _isMoveValid = true;
    }

    private void MoveBetweenTwoPoints()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = _currentTarget.position;

        float distance = Vector2.Distance(currentPos, targetPos);
        if (distance < _threshold)
        {
            _currentTarget = _currentTarget == pointA ? pointB : pointA;
            targetPos = _currentTarget.position;
        }

        targetPos = GetConstrainedTargetPos(targetPos);

        float step = moveSpeed * Time.deltaTime;
        _rb2D.MovePosition(Vector2.MoveTowards(currentPos, targetPos, step));
    }

    private Vector2 GetConstrainedTargetPos(Vector2 targetPos)
    {
        if (isHorizontalMove)
        {
            targetPos.y = pointA.position.y;
        }
        else if (isVerticalMove)
        {
            targetPos.x = pointA.position.x;
        }
        return targetPos;
    }

    #region 外部赋值专用方法
    public void SetMovePoints(Transform newPointA, Transform newPointB)
    {
        pointA = newPointA;
        pointB = newPointB;
        _currentTarget = pointB;
        CheckMovePointsValid();
    }

    public void SetPointA(Transform newPointA) => pointA = newPointA;

    public void SetPointB(Transform newPointB) => pointB = newPointB;
    #endregion
    
}