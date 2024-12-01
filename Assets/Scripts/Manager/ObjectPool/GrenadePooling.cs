using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadePooling : MonoBehaviour
{
    public static GrenadePooling Instance { get; set; }

    private int grenadeQueueSize = 5;
    private Queue<GameObject> grenadeQueue = new Queue<GameObject>();
    [SerializeField] private GameObject grenadePrefab;

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
        InitializeGrenade();
    }

    private void InitializeGrenade()
    {
        for (int i = 0; i < grenadeQueueSize; i++)
        {
            GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
            grenade.gameObject.SetActive(false);
            grenadeQueue.Enqueue(grenade);
        }
    }

    public GameObject GetGrenade()
    {
        if (grenadeQueue.Count > 0)
        {
            GameObject grenade = grenadeQueue.Dequeue();
            return grenade;
        }
        else
        {
            GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
            grenade.gameObject.SetActive(false);
            grenadeQueueSize++;

            return grenade;
        }
    }

    public void ReturnGrenade(GameObject grenade)
    {
        grenade.gameObject.SetActive(false);
        grenadeQueue.Enqueue(grenade);
    }
}
