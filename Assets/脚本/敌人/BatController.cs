using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Ѳ��·������")]
    public Transform[] waypoints; // ·��������
    public float patrolSpeed = 2f; // Ѳ���ٶ�
    private int currentWaypointIndex = 0; // ��ǰ·�������� 
    private Rigidbody2D rb; 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {
        if (GetComponent<CharacterState>().m_Hp <= 0)
            return;
          
         PatrolAlongPath(); 
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

     
    // ���Ƶ��� gizmos
    void OnDrawGizmosSelected()
    {
         
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
        }
    }
}
