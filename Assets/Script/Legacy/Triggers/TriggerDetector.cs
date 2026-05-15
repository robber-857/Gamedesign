using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    [Header("触发后要移动到的目标点")]
    public Transform customTargetPoint; 

    [Header("玩家标签")]
    public string playerTag = "Player"; 
     
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            other.GetComponent<PlayerController>().StartAutoMove(customTargetPoint.position);
            GetComponent<Collider2D>().enabled = false; 
        }
    }
}
