using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public List<GameObject> gunList;

    public static GunManager Instance { get; set; }

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

    public GameObject GetCurrentGun()
    {
        return gunList[0].activeSelf ? gunList[0] : gunList[1];
    }

    public int GetCurrentGunIndex()
    {
        return gunList[0].activeSelf ? 0 : 1;
    }
}
