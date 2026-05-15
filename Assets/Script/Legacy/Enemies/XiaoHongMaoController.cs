using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XiaoHongMaoController : MonoBehaviour
{
    // ״̬ö��
    public enum BossState {Idle, Chase, Attack }
    public BossState currentState;

    // �ƶ�����
    [Header("�ƶ�����")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    private float currentSpeed;

    public GameObject Target; 
    // ������
    [Header("�������")]
    
    public float attackRange = 1.5f; 
    // ��������
    [Header("��������")]
    public float attackCooldown = 2f;
    private float attackTimer;

    // �������
    private Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ��ʼ��״̬
        currentState = BossState.Chase;
        currentSpeed = moveSpeed; 
    }

    void Update()
    {
         

        // ������
        CheckForPlayer();

        if(Target!=null)
        {
            if(Target.GetComponent<CharacterState>().m_Hp<=0)
            {
                currentState = BossState.Idle;
            }
        }

        // ���ݵ�ǰ״ִ̬����Ӧ��Ϊ
        switch (currentState)
        { 
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
        if (Target == null)
            return;
        // ����Ƿ��ڹ�����Χ��
        float distanceToPlayer = Vector2.Distance(transform.position, Target.transform.position);

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
   
    // ׷����Ϊ
    void ChaseBehavior()
    {
        GetComponent<Animator>().SetBool("Walk", true); 
        // ���ù�����ʱ��
        attackTimer = 0;
    }

    // ������Ϊ
    void AttackBehavior()
    {
        GetComponent<Animator>().SetBool("Walk", false);
      
        attackTimer += Time.deltaTime;
       
        // ������ȴ������ִ�й���
        if (attackTimer >= attackCooldown)
        {
            Debug.Log("С��ñ����");
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
            case BossState.Chase:
                if (Target != null)
                {
                    moveDirection = (Target.transform.position - transform.position).normalized;
                }
                break;
        }

        moveDirection.y = 0;
        rb.linearVelocity = moveDirection * currentSpeed;
    }

    // �������
    void AttackPlayer()
    { 
        GetComponent<Animator>().SetTrigger("Attack");  
        if (Target != null && Vector2.Distance(transform.position, Target.transform.position) <= attackRange)
        {
             
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
     

    //����1-��ͨ
    public void Event_Attack1()
    {
        if(Target!=null)
        {
            Target.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
        } 
    } 
}
