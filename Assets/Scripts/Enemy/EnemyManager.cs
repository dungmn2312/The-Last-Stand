using Cysharp.Threading.Tasks;
using EnemySpace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; set; }

    public int spawnPosSize = 4;
    public List<Transform> spawnPos;

    private float spawnWaitTime = 3f, waitForEndLevelTime = 5f;
    private int startEnemyAmount = 5;
    private Enemy.EnemyAttackType enemyType;

    private int enemyPerLevel;
    private int enemyCount = 0;

    private float enemyHPPerLevel;

    internal int enemyLeft;

    private int currentLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUpEnemy(int level)
    {
        currentLevel = level;
        switch (level)
        {
            case 1:
                enemyHPPerLevel = 0.8f;
                enemyPerLevel = 10;

                break;

            case 2:
                enemyHPPerLevel = 1f;
                enemyPerLevel = 15;
                break;

            case 3:
                enemyHPPerLevel = 1.2f;
                enemyPerLevel = 20;

                break;
        }

        enemyLeft = enemyPerLevel;
        UIManager.Instance.SetEnemyLeft(enemyLeft);
        SpawnEnemy(startEnemyAmount);
        InvokeRepeating("WaitForSpawn", 0f, spawnWaitTime);
    }

    public void SpawnEnemy(int enemyAmount)
    {
        for (int i = 0; i < enemyAmount; i++)
        {
            enemyType = RandomEnemyType();
            GameObject enemy = EnemyPooling.Instance.GetEnemy(enemyType);
            if (enemy != null)
            {
                enemy.SetActive(true);
                enemy.gameObject.GetComponent<EnemyController>().HP *= enemyHPPerLevel;
            }
            enemyCount++;
        }
    }

    private void WaitForSpawn()
    {
        if (enemyCount < enemyPerLevel)    SpawnEnemy(1);
    }

    private Enemy.EnemyAttackType RandomEnemyType()
    {
        int index = currentLevel == 1 ? Random.Range(0, 2) : Random.Range(0, 3);
        if (index == 0) return Enemy.EnemyAttackType.Normal;
        if (index == 1) return Enemy.EnemyAttackType.Smite;
        return Enemy.EnemyAttackType.Explode;
    }

    public void UpdateEnemyLeft()
    {
        enemyLeft--;
        UIManager.Instance.SetEnemyLeft(enemyLeft);

        if (enemyLeft == 0)
        {
            UIManager.Instance.CountDownLevel();
        }
    }
}
