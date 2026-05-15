using UnityEngine;

public class TwoPointHoriaontalMovement : MonoBehaviour
{
    [Header("运动点设置")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("运动参数")]
    public float moveSpeed = 2f;
    public bool useSmoothMovement = false;
    public float smoothTime = 0.5f;
    public bool isFirstRight = false;
    public bool canMove = false;
    
    private Vector3 _targetPosition;
    private Vector3 _currentVelocity;
    private bool _isMovingRight; // 标记当前是否向右移动

    
    private void Start()
    {
        if (rightPoint != null)
        {
            _targetPosition = rightPoint.position;
            _isMovingRight = true;
            UpdateFlip(); // 初始化朝向
        }
        else if (leftPoint != null)
        {
            _targetPosition = leftPoint.position;
            _isMovingRight = false;
            UpdateFlip(); // 初始化朝向
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        if (leftPoint == null || rightPoint == null)
        {
            Debug.LogWarning("请设置左侧点和右侧点！", this);
            return;
        }

        // 检测是否到达目标点
        float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
        if (distanceToTarget < 0.1f)
        {
            // 切换目标点并更新移动方向
            _isMovingRight = !_isMovingRight;
            _targetPosition = _isMovingRight ? rightPoint.position : leftPoint.position;
            UpdateFlip(); // 转向时翻转Scale
        }

        // 移动到目标位置
        if (useSmoothMovement)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                _targetPosition,
                ref _currentVelocity,
                smoothTime
            );
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// 通过修改Scale的X轴实现翻转转向
    /// </summary>
    private void UpdateFlip()
    {
        Vector3 newScale = transform.localScale;
        // 根据移动方向设置X轴缩放（向右为正，向左为负）
        if (isFirstRight)
        {
            newScale.x = _isMovingRight ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        }
        else
        {
            newScale.x = _isMovingRight ? -Mathf.Abs(newScale.x) : Mathf.Abs(newScale.x);
        }

        
        transform.localScale = newScale;
    }

    private void OnDrawGizmos()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
            Gizmos.DrawWireSphere(leftPoint.position, 0.2f);
            Gizmos.DrawWireSphere(rightPoint.position, 0.2f);
        }
    }
}
