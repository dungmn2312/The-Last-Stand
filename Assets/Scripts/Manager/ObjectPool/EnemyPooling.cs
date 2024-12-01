using EnemySpace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyPooling : MonoBehaviour
{
    public static EnemyPooling Instance { get; set; }

    public GameObject normalEnemyPrefab;
    private Queue<GameObject> normalEnemyQueue = new Queue<GameObject>();
    private int normalEnemyQueueSize = 15;

    public GameObject explosiveEnemyPrefab;
    private Queue<GameObject> explosiveEnemyQueue = new Queue<GameObject>();
    private int explosiveEnemyQueueSize = 15;

    public GameObject bigBoyEnemyPrefab;
    private Queue<GameObject> bigBoyEnemyQueue = new Queue<GameObject>();
    private int bigBoyEnemyQueueSize = 15;

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

    private void Start()
    {
        AudioManager.Instance.sfxSources.Clear();
        InitializeEnemy(normalEnemyPrefab, normalEnemyQueue, normalEnemyQueueSize);
        InitializeEnemy(explosiveEnemyPrefab, explosiveEnemyQueue, explosiveEnemyQueueSize);
        InitializeEnemy(bigBoyEnemyPrefab, bigBoyEnemyQueue, bigBoyEnemyQueueSize);
    }

    private void InitializeEnemy(GameObject gameObjectPrefab, Queue<GameObject> queue, int queueSize)
    {
        int index;
        for (int i = 0; i < queueSize; i++)
        {
            index = Random.Range(0, EnemyManager.Instance.spawnPosSize); 
            GameObject enemy = Instantiate(gameObjectPrefab, EnemyManager.Instance.spawnPos[index].position, Quaternion.identity);
            queue.Enqueue(enemy);
            enemy.SetActive(false);

            //AudioSource audioSource = enemy.GetComponent<EnemyController>().audioSourceSound.GetComponent<AudioSource>();
            //AudioManager.Instance.AddSFXSound(audioSource);
        }
    }

    public GameObject GetEnemy(Enemy.EnemyAttackType type)
    {
        GameObject enemy = null;

        switch (type)
        {
            case Enemy.EnemyAttackType.Normal:
                enemy = GetSpecificEnemy(normalEnemyQueue);
                break;

            case Enemy.EnemyAttackType.Explode:
                enemy = GetSpecificEnemy(explosiveEnemyQueue);
                break;

            case Enemy.EnemyAttackType.Smite:
                enemy = GetSpecificEnemy(bigBoyEnemyQueue);
                break;
        }

        AudioSource audioSource = enemy.GetComponent<EnemyController>().audioSourceSound.GetComponent<AudioSource>();
        AudioManager.Instance.AddSFXSound(audioSource);
        AudioManager.Instance.SetVolume(audioSource, AudioManager.Instance.maxEnemyVolume);

        return enemy;
    }

    private GameObject GetSpecificEnemy(Queue<GameObject> queue)
    {
        GameObject enemy = null;
        if (queue.Count > 0)    enemy = queue.Dequeue();
        return enemy;
    }

    public void ReturnToPool(GameObject enemy)
    {
        AudioSource audioSource = enemy.GetComponent<EnemyController>().audioSourceSound.GetComponent<AudioSource>();
        AudioManager.Instance.RemoveSFXSound(audioSource);
        Enemy.EnemyAttackType type = enemy.GetComponent<EnemyController>().attackType;

        switch (type)
        {
            case Enemy.EnemyAttackType.Normal:
                EnqueueEnemy(normalEnemyQueue, enemy);
                break;

            case Enemy.EnemyAttackType.Explode:
                EnqueueEnemy(explosiveEnemyQueue, enemy);
                break;

            case Enemy.EnemyAttackType.Smite:
                EnqueueEnemy(bigBoyEnemyQueue, enemy);
                break;
        }
    }

    private void EnqueueEnemy(Queue<GameObject> queue, GameObject enemy)
    {
        enemy.gameObject.SetActive(false);
        queue.Enqueue(enemy);
    }
}
