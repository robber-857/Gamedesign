using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum WitchState { Idle, Walk, Run, Jump, Attack1, Attack2, Hurt, EvadeRun }

public class WitchBossController : MonoBehaviour
{
    [Header("移动设置")]
    public float walkSpeed = 2f;                // 行走速度
    public float runSpeed = 5f;                 // 奔跑速度
    public float jumpForce = 5f;                // 跳跃力度
    public float walkRange = 8f;                // 行走范围（外范围）
    public float attackRange = 4f;              // 攻击范围（内范围）

    [Header("随机移动点设置")]
    public Transform evadePoint1;               // 躲避点1
    public Transform evadePoint2;               // 躲避点2

    [Header("技能设置")]
    public float skillInterval = 5f;           
    [Range(0, 1)] public float attack1Probability = 0.5f;
    [Range(0, 1)] public float evadeProbability = 0.3f; // 受伤后躲避概率
    public float hurtCooldown = 3f;             // 受伤冷却时间

    [Header("组件引用")]
    public Animator anim;                      
    public Rigidbody2D rb;                     
    public SpriteRenderer spriteRenderer;      
    public Transform firePoint1;               
    public Transform firePoint2;               
    public GameObject arrowPrefab;             
    public GameObject energyBallPrefab;        
    public Image hpSlider;

    public GameObject npcItem;

    // 状态与计时器
    private WitchState currentState;
    private WitchState previousState;           // 记录上一状态
    private float skillTimer;                  
    private bool isHurt;                       
    private bool isAttacking;
    private bool isJumping;
    private float hurtCooldownTimer;            // 受伤冷却计时器
    private Transform currentEvadeTarget;       // 当前躲避目标点

    public bool isDead =false;

    public GameObject overBook;
    
    void Start()
    {
        currentState = WitchState.Idle;
        previousState = WitchState.Idle;
        skillTimer = 0;
        hurtCooldownTimer = 0;
        
        // 更可靠的玩家查找方式
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
            
        currentHp = maxHp;
    }

    [Header("生命值")] 
    public int maxHp = 10;
    public int currentHp;
    public Transform player;                   

    void Update()
    {
        if(isDead) return;
        
        // 更新冷却计时器
        if (hurtCooldownTimer > 0)
            hurtCooldownTimer -= Time.deltaTime;
            
        if (skillTimer > 0)
            skillTimer -= Time.deltaTime;

        if (player == null)
        {
            ChangeState(WitchState.Idle);
            return;
        }

        // 始终面向玩家（除了特定状态）
        if (!isHurt && !isAttacking && currentState != WitchState.EvadeRun)
        {
            FacePlayer();
        }

        // 受击、攻击中或躲避时不切换状态
        if (isHurt || isAttacking || currentState == WitchState.EvadeRun)
        {
            ExecuteCurrentState();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 状态切换逻辑
        if (distanceToPlayer > walkRange)
        {
            ChangeState(WitchState.Idle);
        }
        else if (distanceToPlayer > attackRange)
        {
            // 在行走范围和攻击范围之间时，行走靠近玩家
            if (distanceToPlayer > (walkRange + attackRange) / 1.5f)
            {
                ChangeState(WitchState.Run); // 距离较远时奔跑
            }
            else
            {
                ChangeState(WitchState.Walk); // 距离较近时行走
            }
            
            // 随机跳跃（可选）
            if (Random.value < 0.05f && !isJumping)
            {
                ChangeState(WitchState.Jump);
            }
        }
        else if (skillTimer <= 0)
        {
            // 进入攻击范围且技能冷却完成
            ChooseRandomSkill();
        }

        ExecuteCurrentState();
    }

    // 始终面向玩家
    private void FacePlayer()
    {
        if (player != null)
        {
            float direction = player.position.x - transform.position.x;
            spriteRenderer.flipX = direction < 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
        anim.SetTrigger("Die");
        npcItem?.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        rb.gravityScale = 0;
        //Invoke(nameof(DestroyItem), 1.5f);
    }
    
    public void DeadOver()
    {
        if (overBook != null)
        {
            overBook.gameObject.SetActive(true);
        }
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

    private void ChooseRandomSkill()
    {
        isAttacking = true;
        if (Random.value < attack1Probability)
        {
            ChangeState(WitchState.Attack1);
        }
        else
        {
            ChangeState(WitchState.Attack2);
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
            case WitchState.Attack2:
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
        
        previousState = currentState;
        currentState = newState;

        // 重置所有动画参数
        ResetAllAnimParams();

        // 设置当前状态动画
        switch (newState)
        {
            case WitchState.Walk:
                anim.SetBool("IsWalking", true);
                break;
            case WitchState.Run:
                anim.SetBool("IsRunning", true);
                break;
            case WitchState.Jump:
                anim.SetTrigger("Jump");
                break;
            case WitchState.Attack1:
                anim.SetTrigger("Attack1");
                break;
            case WitchState.Attack2:
                anim.SetTrigger("Attack2");
                break;
            case WitchState.Hurt:
                anim.SetTrigger("Hurt");
                break;
            case WitchState.EvadeRun:
                anim.SetBool("IsRunning", true);
                SelectEvadeTarget();
                break;
        }
    }

    // 重置所有动画参数
    private void ResetAllAnimParams()
    {
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsRunning", false);
        anim.ResetTrigger("Jump");
        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("Attack2");
        anim.ResetTrigger("Hurt");
        anim.ResetTrigger("Idle");
    }

    // 选择躲避目标点
    private void SelectEvadeTarget()
    {
        currentEvadeTarget = Random.value > 0.5f ? evadePoint1 : evadePoint2;
        
        // 如果没有设置躲避点，则随机生成一个附近的点
        if (currentEvadeTarget == null)
        {
            Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            GameObject tempObj = new GameObject("TempEvadePoint");
            tempObj.transform.position = (Vector2)transform.position + randomDir * Random.Range(3f, 5f);
            currentEvadeTarget = tempObj.transform;
            Destroy(tempObj, 5f); // 5秒后自动销毁临时点
        }
    }

    private void HandleIdle() 
    { 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
        // 确保Idle动画播放
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            anim.SetTrigger("Idle");
        }
        // 空闲时也面向玩家
        FacePlayer();
    }

    private void HandleWalk()
    {
        if (player == null) return;
        
        // 计算朝向玩家的方向
        Vector2 direction = (player.position - transform.position).normalized;
        
        // 只在X轴移动（2D横向游戏）
        rb.linearVelocity = new Vector2(direction.x * walkSpeed, rb.linearVelocity.y);
        
        // 确保面向玩家
        FacePlayer();
    }

    private void HandleRun()
    {
        if (player == null) return;
        
        // 计算朝向玩家的方向
        Vector2 direction = (player.position - transform.position).normalized;
        
        // 只在X轴移动（2D横向游戏）
        rb.linearVelocity = new Vector2(direction.x * runSpeed, rb.linearVelocity.y);
        
        // 确保面向玩家
        FacePlayer();
    }

    private void HandleJump()
    {
        if (!isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            
            // 跳跃时朝向玩家
            FacePlayer();
            
            // 跳跃时向玩家方向移动
            Vector2 jumpDir = new Vector2(player.position.x - transform.position.x, 0).normalized;
            rb.AddForce(jumpDir * (runSpeed / 2), ForceMode2D.Impulse);
        }
    }

    private void HandleAttack() 
    { 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
        // 攻击时也面向玩家
        FacePlayer();
    }

    private void HandleHurt() 
    { 
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
    }

    private void HandleEvadeRun()
    {
        if (currentEvadeTarget == null) return;
        
        // 计算朝向躲避点的方向
        Vector2 direction = (currentEvadeTarget.position - transform.position).normalized;
        
        // 向躲避点移动
        rb.linearVelocity = new Vector2(direction.x * runSpeed, rb.linearVelocity.y);
        
        // 面向移动方向
        spriteRenderer.flipX = direction.x < 0;
        
        // 到达目标点
        float distanceToTarget = Vector2.Distance(transform.position, currentEvadeTarget.position);
        if (distanceToTarget < 1f)
        {
            // 到达躲避点后面向玩家
            FacePlayer();
            ChangeState(WitchState.Idle);
            
            // 到达躲避点后立即攻击
            Invoke(nameof(ChooseRandomSkill), 0.5f);
        }
    }

    public void AnimationEvent_Attack1()
    {
        if (arrowPrefab != null && firePoint1 != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint1.position, firePoint1.rotation);
            ArrowController arrowCtrl = arrow.GetComponent<ArrowController>();
            if (arrowCtrl != null)
                arrowCtrl.direction = spriteRenderer.flipX ? -1 : 1;
        }
        EndAttack();
    }

    public void AnimationEvent_Attack2()
    {
        if (energyBallPrefab != null && firePoint2 != null)
        {
            GameObject energyBall = Instantiate(energyBallPrefab, firePoint2.position, firePoint2.rotation);
            ArrowController arrowCtrl = energyBall.GetComponent<ArrowController>();
            if (arrowCtrl != null)
                arrowCtrl.direction = spriteRenderer.flipX ? -1 : 1;
            /*EnergyBallController ballCtrl = energyBall.GetComponent<EnergyBallController>();
            if (ballCtrl != null)
                ballCtrl.direction = spriteRenderer.flipX ? -1 : 1;*/
        }
        EndAttack();
    }

    public void AnimationEvent_JumpEnd()
    {
        isJumping = false;
        // 跳跃结束后回到之前的移动状态
        if (previousState == WitchState.Run)
        {
            ChangeState(WitchState.Run);
        }
        else
        {
            ChangeState(WitchState.Walk);
        }
    }

    public void AnimationEvent_HurtEnd()
    {
        isHurt = false;
        hurtCooldownTimer = hurtCooldown; // 开始受伤冷却
        
        // 受伤结束后面向玩家
        FacePlayer();
        
        // 受伤后随机选择躲避或继续攻击
        if (Random.value < evadeProbability)
        {
            ChangeState(WitchState.EvadeRun);
        }
        else
        {
            // 继续攻击
            skillTimer = 1f; // 短暂延迟后攻击
            ChangeState(WitchState.Idle);
        }
    }

    // 动画完成事件 - 用于通知代码动画已播放完毕
    public void AnimationEvent_MovementEnd()
    {
        // 如果仍然在移动状态，重新触发动画（循环）
        if (currentState == WitchState.Walk && !anim.GetBool("IsWalking"))
        {
            anim.SetBool("IsWalking", true);
        }
        else if (currentState == WitchState.Run && !anim.GetBool("IsRunning"))
        {
            anim.SetBool("IsRunning", true);
        }
    }

    private void EndAttack()
    {
        skillTimer = skillInterval;
        isAttacking = false;
        ChangeState(WitchState.Idle);
    }

    public void TakeDamage()
    {
        if (isHurt || isAttacking || hurtCooldownTimer > 0) return;
        
        isHurt = true;
        ChangeState(WitchState.Hurt);
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制行走范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, walkRange);

        // 绘制攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // 绘制面向玩家的射线
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }

        // 绘制躲避点
        if (evadePoint1 != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(evadePoint1.position, 0.5f);
        }
        if (evadePoint2 != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(evadePoint2.position, 0.5f);
        }

        // 绘制当前躲避目标
        if (currentEvadeTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentEvadeTarget.position);
            Gizmos.DrawWireSphere(currentEvadeTarget.position, 0.7f);
        }
    }
}