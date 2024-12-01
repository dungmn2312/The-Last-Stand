using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooling : MonoBehaviour
{
    public static BulletPooling Instance { get; set; }

    private int bulletQueueSize = 10;
    private Queue<GameObject> bulletQueue;
    public GameObject bulletPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        bulletQueue = new Queue<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeBullet();    
    }

    private void InitializeBullet()
    {
        for (int i = 0; i < bulletQueueSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.gameObject.SetActive(false);
            bulletQueue.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletQueue.Count > 0)
        {
            GameObject bullet = bulletQueue.Dequeue();
            //bullet.gameObject.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.gameObject.SetActive(false);
            bulletQueueSize++;
            return bullet;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.gameObject.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}
