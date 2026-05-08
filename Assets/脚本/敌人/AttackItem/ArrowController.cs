using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [Header("箭的设置")]
    public float speed = 15f; // 飞行速度
    public float lifeTime = 3f; // 存在时间（防止无限飞行）
    public bool isEnemyAttack = true; // 是否是敌人的攻击（用于区分伤害目标）

    [HideInInspector] public int direction = 1; // 飞行方向（1=右，-1=左）
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        
        // 初始化物理属性
        rb.gravityScale = 0; // 不受重力影响
        rb.linearVelocity = new Vector2(direction * speed, 0); // 设置初速度

        // 自动销毁（防止内存泄漏）
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检测碰撞目标
        if (isEnemyAttack)
        {
            // 敌人的箭只伤害玩家
            if (other.CompareTag("Player"))
            {
                CharacterState playerState = other.GetComponent<CharacterState>();
                if (playerState != null)
                {
                    playerState.TakeDamge(gameObject, 0); // 调用玩家受击方法
                }
                Destroy(gameObject); // 命中后销毁
            }
        }
    }
}