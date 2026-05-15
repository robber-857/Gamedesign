using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [Header("Ѳ��·������")]
    public Transform[] waypoints; // ·��������
    public float patrolSpeed = 2f; // Ѳ���ٶ�
    private int currentWaypointIndex = 0; // ��ǰ·��������

    [Header("׷������")]
    public float chaseRange = 5f; // ׷��������Χ
    public float chaseSpeed = 3f; // ׷���ٶ�
    private bool isChasing = false; // �Ƿ���׷��״̬
    private Transform target; // ׷��Ŀ�꣨��ң�

    [Header("��������")]
    public float attackRange = 1f; // ������Χ 
    public float attackInterval = 2f; // �������
    private float attackCooldown = 0f; // ��ȴ��ʱ��

    private Rigidbody2D rb;
    private Animator anim; // �����������ѡ��

    public GameObject Target;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (GetComponent<CharacterState>().m_Hp <= 0)
            return;

         if (Target == null)
            Target = GameObject.FindWithTag("Player");
        // ��ȴ��ʱ
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        // Ŀ�����ʱ���״̬�л�
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            // ������Χ��⣨������ȼ���
            if (distanceToTarget <= attackRange)
            {
                isChasing = false;
                Attack();
                return; // ����ʱ��ִ���ƶ��߼�
            }

            // ׷����Χ���
            isChasing = distanceToTarget <= chaseRange;
        }
        else
        {
            isChasing = false; // Ŀ�겻����ʱֹͣ׷��
        }

        // ����״̬�ƶ�
        if (isChasing)
        {
            ChaseTarget();
        }
        else
        {
            PatrolAlongPath();
        }
    }

    // ��·��Ѳ��
    void PatrolAlongPath()
    {
        if (waypoints.Length == 0) return;

        //�ƶ�����
        GetComponent<Animator>().SetBool("Run", true);

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        // ת�򣨸���X�᷽��ת��
        FlipSprite(direction.x);
      //  Debug.Log("���룺" + Vector2.Distance(transform.position, targetWaypoint.position));
        // ����·������л���һ��������С��0.1f��Ϊ���
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 1.0f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // ����Ѳ�߶���
        UpdateAnimation(true);
    }

    // ׷��Ŀ��
    void ChaseTarget()
    {
        if (target == null) return;

        //�ƶ�����
        GetComponent<Animator>().SetBool("Run", true);

        Vector2 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        rb.linearVelocity = direction * chaseSpeed;

        // ת��Ŀ��
        FlipSprite(direction.x);

        // ����׷������������Ѳ�߶������ã�
        UpdateAnimation(true);
    }

    // �����߼�
    void Attack()
    {
        //�ƶ�����
        GetComponent<Animator>().SetBool("Run", false); 
        rb.linearVelocity = Vector2.zero; // ����ʱֹͣ�ƶ�

        // ���Ź�������
        GetComponent<Animator>().SetTrigger("Attack");  

        attackCooldown = attackInterval; // ������ȴ
    }

    // ��ת���鳯��
    void FlipSprite(float xDirection)
    {
        if (xDirection != 0)
        {
            transform.localScale = new Vector3(
                xDirection > 0 ? 1 : -1, // X�����ſ��Ƴ���
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    // ���¶���״̬
    void UpdateAnimation(bool isMoving)
    {
        //if (anim != null)
        //{
        //    anim.SetBool("IsMoving", isMoving);
        //}
    }

    // ���Ƶ��� gizmos
    void OnDrawGizmosSelected()
    {
        // ׷����Χ����ɫ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // ������Χ����ɫ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Ѳ��·������ɫ��
        if (waypoints.Length > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.2f); // ·����
                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position); // ·����
                }
            }
        }
    }

    public void Event_Attack()
    {
        if(Target!=null)
        {
            //�ж����ڲ��ڹ�����Χ��
            float dic = Vector2.Distance(transform.position, target.transform.position);
            if(dic<=attackRange)
            {
                Target.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
            }
            
        }
    }
}