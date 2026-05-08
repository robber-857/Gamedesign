
using UnityEngine;

public class BossController : MonoBehaviour
{
    // ״̬ö��
    public enum BossState { Patrol, Chase, Attack }
    public BossState currentState;

    // �ƶ�����
    [Header("�ƶ�����")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    private float currentSpeed;

    // Ѳ�߲���
    [Header("Ѳ������")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    private int currentPatrolPoint = 0;
    private float patrolWaitTimer;

    // ������
    [Header("�������")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;
    private Transform playerTransform;

    // ��������
    [Header("��������")]
    public float attackCooldown ; 
    private float attackTimer;

    // �������
    private Rigidbody2D rb;
 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       
        // ��ʼ��״̬
        currentState = BossState.Patrol;
        currentSpeed = moveSpeed;

        // ȷ����Ѳ�ߵ�
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("��ΪBOSS����Ѳ�ߵ㣡");
        }
    }

    void Update()
    { 
        // ������
        CheckForPlayer();

        // ���ݵ�ǰ״ִ̬����Ӧ��Ϊ
        switch (currentState)
        {
            case BossState.Patrol:
                PatrolBehavior();
                break;
            case BossState.Chase:
                ChaseBehavior();
                break;
            case BossState.Attack:
                AttackBehavior();
                break;
        }

        // ���¶���״̬
        UpdateAnimations();

        
    }

    void FixedUpdate()
    {
        // Ӧ���ƶ�
        if (currentState != BossState.Attack)
        {
            Move();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // �������Ƿ��ڸ�֪��Χ��
    void CheckForPlayer()
    {
        // ������
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCollider != null)
        {
            playerTransform = playerCollider.transform;

            // ����Ƿ��ڹ�����Χ��
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                currentState = BossState.Attack;
                currentSpeed = 0;
            }
            else
            {
                currentState = BossState.Chase;
                currentSpeed = chaseSpeed;
            }
        }
        else
        {
            // û�м�⵽��ң��ص�Ѳ��״̬
            currentState = BossState.Patrol;
            currentSpeed = moveSpeed;
            playerTransform = null;
        }
    }

    // Ѳ����Ϊ
    void PatrolBehavior()
    {
        GetComponent<Animator>().SetBool("Walk", true);
        if (patrolPoints.Length == 0) return;

        // �ƶ�����ǰѲ�ߵ�
        Transform targetPoint = patrolPoints[currentPatrolPoint];
        Vector2 direction = (targetPoint.position - transform.position).normalized;

        // ����Ƿ񵽴�Ѳ�ߵ�
        if (Vector2.Distance(transform.position, targetPoint.position) < 2.5f)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                // �л�����һ��Ѳ�ߵ�
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
                patrolWaitTimer = 0;
            }
        }
    }

    // ׷����Ϊ
    void ChaseBehavior()
    {
        GetComponent<Animator>().SetBool("Walk", true);
        if (playerTransform == null) return;

        // ���ù�����ʱ��
        attackTimer = 0;
    }

    // ������Ϊ
    void AttackBehavior()
    {
        GetComponent<Animator>().SetBool("Walk", false);
        if (playerTransform == null) return;

        attackTimer += Time.deltaTime;

        // ������ȴ������ִ�й���
        if (attackTimer >= attackCooldown)
        {
            AttackPlayer();
            attackTimer = 0;
        }
    }

    // �ƶ��߼�
    void Move()
    {
        Vector2 moveDirection = Vector2.zero;

        switch (currentState)
        {
            case BossState.Patrol:
                if (patrolPoints.Length > 0)
                {
                    moveDirection = (patrolPoints[currentPatrolPoint].position - transform.position).normalized;
                }
                break;
            case BossState.Chase:
                if (playerTransform != null)
                {
                    moveDirection = (playerTransform.position - transform.position).normalized;
                }
                break;
        }

        moveDirection.y = 0;
        rb.linearVelocity = moveDirection * currentSpeed;
    }

    // �������
    void AttackPlayer()
    {
        int ranattacktype = Random.Range(0, 100);
        if(ranattacktype>=0 && ranattacktype <30)
        {
            GetComponent<Animator>().SetTrigger("Attack1");
        }
        else if(ranattacktype >= 30 && ranattacktype < 60)
        {
            GetComponent<Animator>().SetTrigger("Attack2");
        }
        else if (ranattacktype >= 60 && ranattacktype < 100)
        {
            GetComponent<Animator>().SetTrigger("Attack3");
        } 
    }

    // ���¶���״̬
    void UpdateAnimations()
    { 

        // ��ת��ɫ����
        if (rb.linearVelocity.x != 0)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(rb.linearVelocity.x) * Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    // ����Gizmos������
    void OnDrawGizmosSelected()
    {
        // ���Ƽ�ⷶΧ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // ���ƹ�����Χ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // ����Ѳ��·��
        Gizmos.color = Color.blue;
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);

                    // ���Ƶ���һ���������
                    int nextPoint = (i + 1) % patrolPoints.Length;
                    if (patrolPoints[nextPoint] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextPoint].position);
                    }
                }
            }
        }
    }

    //����1-��ͨ
    public void Event_Attack1()
    {
        Debug.Log("BOSS����1");
        playerTransform.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
    }

    //����2 
    public void Event_Attack2()
    {
        Debug.Log("BOSS����2");
        Rigidbody2D rid = playerTransform.gameObject.GetComponent<Rigidbody2D>();
        Vector2 dir = playerTransform.position - transform.position; 
        rid.AddForce(dir.normalized * 10,ForceMode2D.Impulse);
        playerTransform.gameObject.GetComponent<PlayerController>().GoToBAttackSate();
        playerTransform.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
    }

    //����3 
    public void Event_Attack3()
    {
        Debug.Log("BOSS����3");
        Rigidbody2D rid = playerTransform.gameObject.GetComponent<Rigidbody2D>();
        Vector2 dir = playerTransform.position - transform.position;
        rid.AddForce((dir ).normalized* 10, ForceMode2D.Impulse);
        rid.AddForce(Vector2.up.normalized * 10, ForceMode2D.Impulse);
        playerTransform.gameObject.GetComponent<PlayerController>().GoToBAttackSate();
        playerTransform.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
    }
}