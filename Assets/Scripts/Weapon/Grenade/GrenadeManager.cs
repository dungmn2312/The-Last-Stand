using Cysharp.Threading.Tasks;
using GunSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeManager : MonoBehaviour, IObserverItem
{
    public static GrenadeManager Instance { get; set; }

    private Animator animator;

    [SerializeField] private GameObject fakeGrenade;
    public int grenadeAmount;
    public bool hasThrow;

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

    private void OnEnable()
    {
        ItemController.OnTakeItem += OnNotifyTakeItem;
    }

    private void OnDisable()
    {
        ItemController.OnTakeItem -= OnNotifyTakeItem;
    }

    private void Start()
    {
        animator = AnimatorManager.Instance.playerAnimator;
        hasThrow = false;
        grenadeAmount = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GunController gun = GunManager.Instance.GetCurrentGun().GetComponent<GunController>();
            if (!hasThrow && grenadeAmount > 0 && gun.currentState == gun.idleState)
                    PreThrow();   
        }
    }

    public void OnNotifyTakeItem(ItemController itemController)
    {
        ItemSoundManager.Instance.PlayItemSound();
        if (itemController.itemType == Item.ItemType.Grenade) grenadeAmount += itemController.itemEffectGrenade;
        UIManager.Instance.UpdateGrenadeAmount(grenadeAmount);
    }

    private async void PreThrow()
    {
        hasThrow = true;
        grenadeAmount--;
        UIManager.Instance.UpdateGrenadeAmount(grenadeAmount);
        GrenadeController grenade = GrenadePooling.Instance.GetGrenade().GetComponent<GrenadeController>();
        animator.SetTrigger("throw");

        grenade.gameObject.SetActive(true);
        GameObject gun = GunManager.Instance.GetCurrentGun();
        gun.SetActive(false);
        fakeGrenade.SetActive(true);

        await UniTask.WaitForSeconds(1.65f);

        grenade.ThrowGrenade(transform);
        

        fakeGrenade.SetActive(false);
        gun.SetActive(true);

        hasThrow = false;
    }
}
