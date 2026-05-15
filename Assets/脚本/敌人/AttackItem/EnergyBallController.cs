using UnityEngine;

public class EnergyBallController : MonoBehaviour
{
    [Header("元气弹设置")]
    public float speed = 10f; // 飞行速度（比箭慢一些）
    public float lifeTime = 5f; // 存在时间
    public float explosionRadius = 0.5f; // 爆炸范围（可选）
    public bool isEnemyAttack = true;

    [HideInInspector] public int direction = 1; // 飞行方向
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        
        // 初始化物理属性
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(direction * speed, 0);

        // 自动销毁
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemyAttack)
        {
            // 伤害玩家
            if (other.CompareTag("Player"))
            {
                CharacterState playerState = other.GetComponent<CharacterState>();
                if (playerState != null)
                {
                    playerState.TakeDamge(gameObject, 0);
                }
                Explode(); // 命中后爆炸销毁
            }
        }
    }

    // 爆炸效果（可选，可添加粒子特效）
    void Explode()
    {
        // 这里可以添加爆炸粒子效果

        Destroy(gameObject); // 销毁自身
    }
}