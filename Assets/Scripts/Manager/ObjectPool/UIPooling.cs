using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPooling : MonoBehaviour
{
    public static UIPooling Instance { get; set; }

    public TextMesh floatingDamageText;
    private Queue<TextMesh> floatingDamageTextQueue = new Queue<TextMesh>();
    private int floatingDamageTextQueueSize = 10;

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
    }

    private void Start()
    {
        InitializeFloatingText();
    }

    private void InitializeFloatingText()
    {
        for (int i = 0; i < floatingDamageTextQueueSize; i++)
        {
            TextMesh textMesh = Instantiate(floatingDamageText, transform.position, Quaternion.identity, transform);
            textMesh.gameObject.SetActive(false);
            floatingDamageTextQueue.Enqueue(textMesh);
        }
    }

    public TextMesh GetFloatingText()
    {
        if (floatingDamageTextQueue.Count > 0)
        {
            TextMesh textMesh = floatingDamageTextQueue.Dequeue();
            return textMesh;
        }
        else
        {
            TextMesh textMesh = Instantiate(floatingDamageText, transform.position, Quaternion.identity, transform);
            textMesh.gameObject.SetActive(false);
            floatingDamageTextQueueSize++;
            return textMesh;
        }
    }

    public void ReturnFloatingText(TextMesh textMesh)
    {
        textMesh.gameObject.SetActive(false);
        textMesh.gameObject.transform.SetParent(transform);
        floatingDamageTextQueue.Enqueue(textMesh);
    }
}
