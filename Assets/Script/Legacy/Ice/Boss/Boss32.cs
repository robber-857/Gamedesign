using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Boss32 : MonoBehaviour
{
    [Header("移动设置")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpForce = 5f;
    public float walkRange = 8f;
    public float attackRange = 4f;

    [Header("随机移动点设置")]
    public Transform evadePoint1;
    public Transform evadePoint2;

    [Header("技能设置")]
    public float skillInterval = 5f;
    [Range(0, 1)] public float evadeProbability = 0.3f;
    public float hurtCooldown = 3f;
    public float attackDelay = 0.15f;

    [Header("动画设置")]
    public float deathAnimationDuration = 1.0f;

    [Header("组件引用")]
    public Animator anim;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Transform firePoint1;
    public GameObject arrowPrefab;
    public Image hpSlider;
    
    private WitchState currentState;
    private float skillTimer;
    private float hurtCooldownTimer;
    private Transform currentEvadeTarget;
    private float attackTimer;
    private bool isAttackExecuted;

    public bool isDead = false;

    [Header("生命值")] 
    public int maxHp = 10;
    public int currentHp;
    public Transform player;

    private Vector3 originalLocalScale;
    
    public GameObject overBook;

    public enum WitchState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack1,
        Hurt,
        EvadeRun
    }

    void Start()
    {
        currentState = WitchState.Idle;
        skillTimer = 0;
        hurtCooldownTimer = 0;
        attackTimer = 0;
        isAttackExecuted = false;
        
        originalLocalScale = transform.localScale;
        
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
        currentHp = maxHp;

        if (anim != null)
        {
            anim.SetTrigger("Idle");
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        if (isDead) return;
        
        if (hurtCooldownTimer > 0)
            hurtCooldownTimer -= Time.deltaTime;
            
        if (skillTimer > 0)
            skillTimer -= Time.deltaTime;

        if (player == null)
        {
            ChangeState(WitchState.Idle);
            return;
        }

        if (currentState != WitchState.EvadeRun)
        {
            FacePlayer();
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer > walkRange)
        {
            ChangeState(WitchState.Idle);
        }
        else if (distanceToPlayer > attackRange)
        {
            if (distanceToPlayer > (walkRange + attackRange) / 1.5f)
            {
                ChangeState(WitchState.Run);
            }
            else
            {
                ChangeState(WitchState.Walk);
            }
            
        }
        else if (skillTimer <= 0 && currentState != WitchState.Attack1)
        {
            UseRemoteAttack();
        }

        ExecuteCurrentState();
    }

    private void FacePlayer()
    {
        if (player == null) return;

        float playerDirection = player.position.x - transform.position.x;
        Vector3 newScale = originalLocalScale;

        newScale.x = playerDirection < 0 ? -Mathf.Abs(originalLocalScale.x) : Mathf.Abs(originalLocalScale.x);
        
        transform.localScale = newScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (other.CompareTag("PlayerAttack"))
        {
            currentHp--;
            UpdateHpView();
            
            if (currentHp <= 0)
            {
                Die();
                return;
            }

            TakeDamage();
        }
    }

    private void Die()
    {
        isDead = true;
        
        GetComponent<BoxCollider2D>().enabled = false;
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        
        if (anim != null)
        {
            anim.SetTrigger("Death");
        }
    }

    public void DeadOver()
    {
        overBook.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public void UpdateHpView()
    {
        Debug.Log("UpdateHpView");
        hpSlider.fillAmount = (float)currentHp / maxHp;
    }

    private void UseRemoteAttack()
    {
        ChangeState(WitchState.Attack1);
        attackTimer = attackDelay;
        isAttackExecuted = false;
        
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }
    }

    private void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case WitchState.Idle:
                HandleIdle();
                break;
            case WitchState.Walk:
                HandleWalk();
                break;
            case WitchState.Run:
                HandleRun();
                break;
            case WitchState.Jump:
                HandleJump();
                break;
            case WitchState.Attack1:
                HandleAttack();
                break;
            case WitchState.Hurt:
                HandleHurt();
                break;
            case WitchState.EvadeRun:
                HandleEvadeRun();
                break;
        }
    }

    private void ChangeState(WitchState newState)
    {
        if (currentState == newState) return;
        
        currentState = newState;

        if (newState == WitchState.Idle && anim != null)
        {
            anim.SetTrigger("Idle");
        }
    }

    private void SelectEvadeTarget()
    {
        currentEvadeTarget = Random.value > 0.5f ? evadePoint1 : evadePoint2;
        
        if (currentEvadeTarget == null)
        {
            Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            GameObject tempObj = new GameObject("TempEvadePoint");
            tempObj.transform.position = (Vector2)transform.position + randomDir * Random.Range(3f, 5f);
            currentEvadeTarget = tempObj.transform;
            Destroy(tempObj, 5f);
        }
    }

    #region 状态处理方法
    private void HandleIdle() 
    { 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
    }

    private void HandleWalk()
    {
        if (player == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * walkSpeed, rb.linearVelocity.y);
    }

    private void HandleRun()
    {
        if (player == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * runSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        Vector2 jumpDir = new Vector2(player.position.x - transform.position.x, 0).normalized;
        rb.AddForce(jumpDir * (runSpeed / 2), ForceMode2D.Impulse);
        
        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > (walkRange + attackRange) / 1.5f)
            {
                ChangeState(WitchState.Run);
            }
            else
            {
                ChangeState(WitchState.Walk);
            }
        }
    }

    private void HandleAttack() 
    { 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
        
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && !isAttackExecuted)
        {
            if (arrowPrefab != null && firePoint1 != null)
            {
                GameObject arrow = Instantiate(arrowPrefab, firePoint1.position, firePoint1.rotation);
                ArrowController arrowCtrl = arrow.GetComponent<ArrowController>();
                if (arrowCtrl != null)
                {
                    arrowCtrl.direction = transform.localScale.x > 0 ? 1 : -1;
                }
            }
            
            isAttackExecuted = true;
            skillTimer = skillInterval;
            ChangeState(WitchState.Idle);
        }
    }

    private void HandleHurt() 
    { 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
        
        hurtCooldownTimer = hurtCooldown;
        ChangeState(WitchState.Idle);
        
        if (Random.value < evadeProbability)
        {
            SelectEvadeTarget();
            ChangeState(WitchState.EvadeRun);
        }
        else
        {
            skillTimer = 1f;
        }
    }

    private void HandleEvadeRun()
    {
        if (currentEvadeTarget == null) 
        {
            ChangeState(WitchState.Idle);
            return;
        }
        
        Vector2 direction = (currentEvadeTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * runSpeed * 1.2f, rb.linearVelocity.y);
        
        Vector3 evadeScale = originalLocalScale;
        evadeScale.x = direction.x < 0 ? Mathf.Abs(originalLocalScale.x) : -Mathf.Abs(originalLocalScale.x);
        transform.localScale = evadeScale;
        
        float distanceToTarget = Vector2.Distance(transform.position, currentEvadeTarget.position);
        if (distanceToTarget < 1f)
        {
            FacePlayer();
            ChangeState(WitchState.Idle);
            skillTimer = 0.5f;
        }
    }
    #endregion

    public void TakeDamage()
    {
        if (isDead || currentState == WitchState.Hurt || hurtCooldownTimer > 0) return;
        
        ChangeState(WitchState.Hurt);
    }
    
}