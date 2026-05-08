using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapC4 : MonoBehaviour
{
    public bool isDownItem = false;
    // 伤害间隔（秒）
    public float damageInterval = 2f;

    // 存储进入火焰的玩家
    private List<CharacterState> affectedPlayers = new List<CharacterState>();
    // 计时器
    private float damageTimer = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测玩家
        if (other.CompareTag("Player"))
        {
            CharacterState playerState = other.GetComponent<CharacterState>();
            if (playerState != null && !affectedPlayers.Contains(playerState))
            {
                // 首次进入立即扣一次血
                playerState.TakeDamge(this.gameObject, 0);
                affectedPlayers.Add(playerState);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 玩家离开火焰范围时移除
        if (other.CompareTag("Player"))
        {
            CharacterState playerState = other.GetComponent<CharacterState>();
            if (playerState != null && affectedPlayers.Contains(playerState))
            {
                affectedPlayers.Remove(playerState);
            }
        }
    }

    private void Update()
    {
        // 如果有玩家在火焰范围内，开始计时
        if (affectedPlayers.Count > 0)
        {
            damageTimer += Time.deltaTime;
            
            // 达到间隔时间
            if (damageTimer >= damageInterval)
            {
                // 重置计时器
                damageTimer = 0f;
                
                // 遍历所有在火焰中的玩家扣血
                for (int i = affectedPlayers.Count - 1; i >= 0; i--)
                {
                    CharacterState playerState = affectedPlayers[i];
                    
                    // 检查玩家是否还存在
                    if (playerState != null)
                    {
                        playerState.TakeDamge(this.gameObject, 0);
                    }
                    else
                    {
                        // 移除已销毁的玩家引用
                        affectedPlayers.RemoveAt(i);
                    }
                }

                // 如果是可拾取物品，扣血后销毁（可选，根据你的需求调整）
                if (isDownItem && affectedPlayers.Count > 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            // 没有玩家时重置计时器
            damageTimer = 0f;
        }
    }

    // 可选：清理引用，防止内存泄漏
    private void OnDestroy()
    {
        affectedPlayers.Clear();
    }
}
