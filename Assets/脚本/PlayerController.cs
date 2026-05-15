using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("�ƶ��ٶ�")]
    public float moveSpeed = 5f;
    [Tooltip("�������ٶ�")]
    public float climbSpeed = 3f;
    
    [Header("��Ծ����")]
    [Tooltip("��Ծ��")]
    public float jumpForce = 7f;
    [Tooltip("������ľ���")]
    public float groundCheckDistance = 0.2f;
    [Tooltip("�������λ��")]
    public Transform groundCheckPoint;
    [Tooltip("����ͼ��")]
    public LayerMask groundLayer;
    
    private bool isOnLadder; 
    private float originalGravity; 

    private Rigidbody2D rb;
    private Collider2D[] ownColliders;
    private bool isGrounded; // �Ƿ��ڵ����ϣ������ж�������
    public float horizontalInput; // ˮƽ����

    private bool isAutoMoving;
    public Vector2 m_ATargetPos;

    //�Ƿ�����Ծ״̬�������ڶ���������������Ծ�жϣ�
    public bool IsJump;

    //�Ƿ��ٱ�����״̬
    public bool IsBAttack;
    //����Ϊ������״̬����ʱ��
    private float m_CurBATime;

    // ����Animator���������Ƶ��GetComponent��
    private Animator anim;
    private Vector3 initialScale;
    
    public bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ownColliders = GetComponents<Collider2D>();
        originalGravity = rb.gravityScale;
        // ����Animator���
        anim = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "ChuanSong")
        {
            moveSpeed = 10f;
            jumpForce = 25f;
        }
    }
    

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.tag == "ChuanSong")
        {
            moveSpeed =5f;
            jumpForce = 20f;
        }
    }

    public void GoToBAttackSate()
    {
        IsBAttack = true;
        m_CurBATime = 0;
    }

    private void Update()
    {
        if (IsBAttack)
        {
            m_CurBATime += Time.deltaTime;
            if (m_CurBATime >= 1)
            {
                m_CurBATime = 0;
                IsBAttack = false;
            }
            return;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (canMove)
        {
            // ��ȡˮƽ���루A/D�������Ҽ�ͷ��
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }

        

        // �ؼ��޸�1����Ծ�жϸ�Ϊֱ�Ӽ���Ƿ��ڵ��棨isGrounded��������IsJump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // ��ת��ɫ����
        FlipCharacter();

        //�Զ��ƶ�
        if (isAutoMoving)
        {
            Debug.Log("�Զ��ƶ�");
            AutoMoveToTarget();
        }

        // ���䶯���������ָ�ע�ʹ��룬�Ż����飩
        if (!isGrounded && !isOnLadder) // ������ʱ�������䶯��
        {
            if (rb.linearVelocity.y < -2.5f)
            {
                anim.SetBool("Fall", true);
            }
            if (rb.linearVelocity.y >= 0)
            {
                anim.SetBool("Fall", false);
            }
        }
        
        HandleLadderMovement();
    }

    private void FixedUpdate()
    {
        if (IsBAttack)
            return;

        if (isAutoMoving)
            return;
        
        CheckGrounded();
   
        if (canMove)
        {
            MoveCharacter();
        }
        // ������ʱ�������ܲ�����
        if (!isOnLadder)
        {
            MoveAnimtonController();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            isOnLadder = true;
            rb.gravityScale = 0; 
            IsJump = false;
            // ��������ʱ������Ծ/����/�ܲ�����
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", false);
            anim.SetBool("Run", false);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ladder")
        {
            isOnLadder = false;
            rb.gravityScale = originalGravity;
            // �뿪����ʱ�ر����ݶ���
            anim.SetBool("Climb", false);
        }
    }
    
    private void HandleLadderMovement()
    {
        if (isOnLadder)
        {
            float verticalInput = Input.GetAxisRaw("Vertical"); 
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * climbSpeed);

            // ��¥�ݶ������ƣ��д�ֱ����ʱ�������ݶ���������ֹͣ
            if (Mathf.Abs(verticalInput) > 0)
            {
                anim.SetBool("Climb", true);
            }
            else
            {
                anim.SetBool("Climb", false);
            }
        }
    }

    // ��ɫ�ƶ�
    private void MoveCharacter()
    {
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;
    }

    //��������
    public void MoveAnimtonController()
    {
        if (horizontalInput != 0)
            anim.SetBool("Run", true);
        else
            anim.SetBool("Run", false);
    }

    // ��ɫ��Ծ
    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false; // ��Ծ��������Ϊ���ڵ��棨��ֹ֡ͬ�����⵼�µĶ�����
        IsJump = true; // �����ڶ���״̬
        anim.SetBool("Jump", true);
        anim.SetBool("Fall", false); // ��Ծʱ�ر����䶯��
    }

    public void JumpMaxPower()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce*1.5f);
        isGrounded = false; // ��Ծ��������Ϊ���ڵ��棨��ֹ֡ͬ�����⵼�µĶ�����
        IsJump = true; // �����ڶ���״̬
        anim.SetBool("Jump", true);
        anim.SetBool("Fall", false); // ��Ծʱ�ر����䶯��
    }

    // ����Ƿ��ڵ�����
    private void CheckGrounded()
    {
        if (groundCheckPoint == null)
        {
            isGrounded = false;
            return;
        }

        LayerMask raycastMask = groundLayer.value == 0
            ? Physics2D.DefaultRaycastLayers
            : groundLayer;

        RaycastHit2D[] hits = Physics2D.RaycastAll(
            groundCheckPoint.position,
            Vector2.down,
            groundCheckDistance,
            raycastMask
        );

        isGrounded = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || hit.collider.isTrigger || IsOwnCollider(hit.collider))
            {
                continue;
            }

            isGrounded = true;
            break;
        }

        if (isGrounded)
        {
            IsJump = false; // ��غ�������Ծ״̬�������ڶ�����
            anim.SetBool("Jump", false);
            anim.SetBool("Fall", false);
        }
    }

    private bool IsOwnCollider(Collider2D other)
    {
        if (ownColliders == null)
        {
            return false;
        }

        foreach (Collider2D ownCollider in ownColliders)
        {
            if (other == ownCollider)
            {
                return true;
            }
        }

        return false;
    }

    // ��ת��ɫ����
    private void FlipCharacter()
    {
        if (horizontalInput != 0)
        {
            SetFacingDirection(horizontalInput);
        }
    }

    //��ȡ��ɫ���� 0��� 1�ұߣ�ע��ԭ�߼�����1/-1������0/1�ɸ�Ϊ return horizontalInput > 0 ? 1 : 0;��
    public float GetDirection()
    {
        return transform.localScale.x;
    }

    // �Զ��ƶ���Ŀ���
    private void AutoMoveToTarget()
    {
        // ������ҵ�Ŀ���ķ���
        Vector2 direction = (m_ATargetPos - (Vector2)transform.position).normalized;
        // ��Ŀ����ƶ�
        rb.linearVelocity = direction * 12;

        // ����Ŀ��㣨����С��0.1ʱֹͣ��
        if (Vector2.Distance(transform.position, m_ATargetPos) < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            isAutoMoving = false; // �����Զ��ƶ�
            Debug.Log("�ѵ���Ŀ���");
            rb.isKinematic = false;
        }

        //����ת����
        SetFacingDirection(direction.x);
    }

    private void SetFacingDirection(float directionX)
    {
        if (Mathf.Approximately(directionX, 0f))
        {
            return;
        }

        float facingSign = directionX > 0f ? 1f : -1f;
        transform.localScale = new Vector3(
            Mathf.Abs(initialScale.x) * facingSign,
            initialScale.y,
            initialScale.z
        );
    }

    //��ʼ�Զ��ƶ�
    public void StartAutoMove(Vector2 _tartpos)
    {
        m_ATargetPos = _tartpos;
        isAutoMoving = true;
        rb.isKinematic = true;
    }

    // ���ӻ����������ߣ���Scene��������ʾ��
    private void OnDrawGizmos()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                groundCheckPoint.position,
                groundCheckPoint.position + Vector3.down * groundCheckDistance
            );
        }
    }
}
