using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Random2DSpawner : MonoBehaviour
{
    [Header("生成配置")]
    public GameObject spawnPrefab;
    public Transform spawnAreaMin;
    public Transform spawnAreaMax;

    [Header("时间配置")]
    [Min(0)] public float minSpawnInterval = 4f;
    [Min(0)] public float maxSpawnInterval = 6f;
    [Min(0)] public float minDestroyTime = 4f;
    [Min(0)] public float maxDestroyTime = 7f;

    private Coroutine spawnCoroutine;
    private bool isSpawning = false;

    private void Start()
    {
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (isSpawning || spawnPrefab == null)
        {
            if (spawnPrefab == null)
                Debug.LogError("生成预制体未赋值！无法启动生成逻辑", this);
            return;
        }

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnLoopCoroutine());
        Debug.Log("开始在2D区域内随机生成物体", this);
    }

    public void StopSpawning()
    {
        if (!isSpawning || spawnCoroutine == null) return;

        StopCoroutine(spawnCoroutine);
        isSpawning = false;
        spawnCoroutine = null;
        Debug.Log("停止生成物体", this);
    }

    private IEnumerator SpawnLoopCoroutine()
    {
        while (isSpawning)
        {
            SpawnObjectAtRandomPosition();

            float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    private void SpawnObjectAtRandomPosition()
    {
        if (spawnAreaMin == null || spawnAreaMax == null)
        {
            Debug.LogError("生成区域标记点未赋值！请设置spawnAreaMin和spawnAreaMax", this);
            return;
        }

        float randomX = Random.Range(spawnAreaMin.position.x, spawnAreaMax.position.x);
        float randomY = Random.Range(spawnAreaMin.position.y, spawnAreaMax.position.y);
        Vector3 spawnPosition = new Vector3(randomX, randomY, transform.position.z);

        GameObject spawnedObj = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);

        float randomDestroyTime = Random.Range(minDestroyTime, maxDestroyTime);
        Destroy(spawnedObj, randomDestroyTime);

        Debug.Log($"生成物体：{spawnedObj.name}，销毁倒计时：{randomDestroyTime:F1}秒", this);
    }

    private void OnDestroy()
    {
        StopSpawning();
    }
    
}