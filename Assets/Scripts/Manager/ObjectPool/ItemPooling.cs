using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPooling : MonoBehaviour
{
    public static ItemPooling Instance { get; set; }

    [SerializeField] private GameObject healUpPrefab;
    private int healUpQueueSize = 5;
    private Queue<GameObject> healUpQueue = new Queue<GameObject>();

    [SerializeField] private GameObject ammoPrefab;
    private int ammoQueueSize = 5;
    private Queue<GameObject> ammoQueue = new Queue<GameObject>();

    [SerializeField] private GameObject grenadePrefab;
    private int grenadeQueueSize = 5;
    private Queue<GameObject> grenadeQueue = new Queue<GameObject>();

    private int[] RandomItemArray = new int[10];

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
        for (int i = 1; i <= 10; i++)
        {
            RandomItemArray[i-1] = i;
        }

        InitializeItem(healUpPrefab, healUpQueue, healUpQueueSize);
        InitializeItem(ammoPrefab, ammoQueue, ammoQueueSize);
        InitializeItem(grenadePrefab, grenadeQueue, grenadeQueueSize);
    }

    private void InitializeItem(GameObject gameObjectPrefab, Queue<GameObject> queue, int queueSize)
    {
        for (int i = 0; i < queueSize; i++)
        {
            GameObject item = Instantiate(gameObjectPrefab, gameObjectPrefab.transform.position, Quaternion.identity);
            queue.Enqueue(item);
            item.SetActive(false);
        }
    }

    public GameObject GetItem(Item.ItemType type)
    {
        GameObject item = null;

        switch (type)
        {
            case Item.ItemType.HealUp:
                item = GetSpecificItem(healUpQueue);
                break;

            case Item.ItemType.Ammo:
                item = GetSpecificItem(ammoQueue);
                break;

            case Item.ItemType.Grenade:
                item = GetSpecificItem(grenadeQueue);
                break;
        }

        return item;
    }

    private GameObject GetSpecificItem(Queue<GameObject> queue)
    {
        GameObject item = queue.Dequeue();
        return item;
    }

    public void ReturnToPool(GameObject item)
    {
        Item.ItemType type = item.GetComponent<Item>().itemType;

        switch (type)
        {
            case Item.ItemType.HealUp:
                EnqueueItem(healUpQueue, item);
                break;

            case Item.ItemType.Ammo:
                EnqueueItem(ammoQueue, item);
                break;

            case Item.ItemType.Grenade:
                EnqueueItem(grenadeQueue, item);
                break;
        }
    }

    private void EnqueueItem(Queue<GameObject> queue, GameObject item)
    {
        item.gameObject.SetActive(false);
        queue.Enqueue(item);
    }

    public GameObject RandomItem()
    {
        int index = Random.Range(1, RandomItemArray.Length + 1);
        GameObject item = null;
        if (index < 5)
        {
            item = GetItem(Item.ItemType.Ammo);
        }
        else if (5 <= index && index < 8)
        {
            item = GetItem(Item.ItemType.Grenade);
        }
        else
        {
            item = GetItem(Item.ItemType.HealUp);
        }

        return item;
    }
} 
