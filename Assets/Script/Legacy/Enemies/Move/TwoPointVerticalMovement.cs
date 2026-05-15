using UnityEngine;

public class TwoPointVerticalMovement : MonoBehaviour
{
    [Header("运动点设置")]
    public Transform topPoint;    // 上终点
    public Transform bottomPoint; // 下终点

    [Header("运动参数")]
    public float moveSpeed = 2f;
    public bool useSmoothMovement = false;
    public float smoothTime = 0.5f;
    public bool isFirstUp = false; // 初始是否向上移动
    public float pauseTime = 2f;   // 两端点停顿时间（秒）
    
    public bool canMove = false;

    private Vector3 _targetPosition;
    private Vector3 _currentVelocity;
    private bool _isMovingUp;      // 是否向上移动
    private bool _isPausing;       // 是否正在停顿
    private float _pauseTimer;     // 停顿计时器

    private void Start()
    {
        // 初始化目标点和移动方向
        if (isFirstUp && topPoint != null)
        {
            _targetPosition = topPoint.position;
            _isMovingUp = true;
        }
        else if (bottomPoint != null)
        {
            _targetPosition = bottomPoint.position;
            _isMovingUp = false;
        }

        _isPausing = false;
        _pauseTimer = 0;
    }

    private void Update()
    {

        if (!canMove) return;
        
        // 检查是否设置了运动点
        if (topPoint == null || bottomPoint == null)
        {
            Debug.LogWarning("请设置上终点和下终点！", this);
            return;
        }

        // 如果正在停顿，执行停顿逻辑
        if (_isPausing)
        {
            _pauseTimer += Time.deltaTime;
            if (_pauseTimer >= pauseTime)
            {
                // 停顿结束，开始移动
                _isPausing = false;
                _pauseTimer = 0;
                // 切换目标点
                _isMovingUp = !_isMovingUp;
                _targetPosition = _isMovingUp ? topPoint.position : bottomPoint.position;
            }
            return; // 停顿期间不执行移动逻辑
        }

        // 检测是否到达目标点（仅判断Y轴）
        float distanceToTarget = Mathf.Abs(transform.position.y - _targetPosition.y);
        if (distanceToTarget < 0.1f)
        {
            // 到达端点，开始停顿
            _isPausing = true;
            return;
        }

        // 执行移动逻辑（仅Y轴移动）
        if (useSmoothMovement)
        {
            // 平滑移动（保持X和Z轴不变，只动Y轴）
            Vector3 targetPos = new Vector3(transform.position.x, _targetPosition.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref _currentVelocity,
                smoothTime
            );
        }
        else
        {
            // 匀速移动（仅Y轴）
            Vector3 newPosition = transform.position;
            newPosition.y = Mathf.MoveTowards(
                transform.position.y,
                _targetPosition.y,
                moveSpeed * Time.deltaTime
            );
            transform.position = newPosition;
        }
    }

    // 场景视图绘制辅助线
    private void OnDrawGizmos()
    {
        if (topPoint != null && bottomPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(topPoint.position, bottomPoint.position);
            Gizmos.DrawWireSphere(topPoint.position, 0.2f);
            Gizmos.DrawWireSphere(bottomPoint.position, 0.2f);
        }
    }
}