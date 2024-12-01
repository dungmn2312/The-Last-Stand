using GunSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : Item
{
    private Animator animator;
    internal ParticleSystem effect;
    private float lifeTime = 10f;

    public delegate void TakeItemHandler(ItemController itemController);
    public static event TakeItemHandler OnTakeItem;

    internal int itemEffectHP, itemEffectAmmo, itemEffectGrenade;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();

        if (itemType == ItemType.Grenade) animator.SetBool("grenade", true);
    }

    private void OnEnable()
    {
        Invoke("ReturnToPool", lifeTime);
    }

    private void Start()
    {
        itemEffectHP = 30;
        itemEffectGrenade = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            SetItemEffect(player);

            OnTakeItem?.Invoke(this);
            
            ReturnToPool();
        }
    }

    private void SetItemEffect(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.HealUp:
                effect = player.healingEffect;
                break;

            case ItemType.Ammo:
                effect = player.itemEffect;
                GunController gun = GunManager.Instance.gunList[1].GetComponent<GunController>();
                itemEffectAmmo = gun.ammoPerMag;
                break;

            case ItemType.Grenade:
                effect = player.itemEffect;
                break;
        }
    }

    private void ReturnToPool()
    {
        ItemPooling.Instance.ReturnToPool(gameObject);
    }
}
