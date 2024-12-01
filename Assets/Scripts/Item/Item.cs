using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum ItemType
    {
        HealUp,
        Ammo,
        Grenade
    }

    public ItemType itemType;
}
