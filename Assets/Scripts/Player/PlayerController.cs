using Cysharp.Threading.Tasks;
using EnemySpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Player, IObserverEnemy, IObserverItem
{

    private Animator animator;

    [Header("--- Slider ---")]
    public Slider HPSlider;
    public Slider EaseHPSlider;
    public Slider SPSlider;


    public ParticleSystem healingEffect;
    public ParticleSystem itemEffect;
    private Collider playerCollider;

    internal float maxHP, maxSP;

    internal float reviveSPSpeed = 7f;

    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject grenade;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        HPSlider.maxValue = HP;
        EaseHPSlider.maxValue = HP;
        SPSlider.maxValue = SP;

        maxHP = HP;
        maxSP = SP;

        playerCollider = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (playerCollider != null) playerCollider.enabled = true;
        EnemyController.OnEnemyAttack += OnNotifyEnemyAttack;
        ItemController.OnTakeItem += OnNotifyTakeItem;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyAttack -= OnNotifyEnemyAttack;
        ItemController.OnTakeItem -= OnNotifyTakeItem;
    }

    #region || --- Observer Method --- ||
    public void OnNotifyEnemyAttack(float damage)
    {
        HP -= damage;
        if (HP > 0)
        {
            animator.SetTrigger("hurt");
            UIManager.Instance.OnNotifyEnemyAttack();
            PlayerSoundManager.Instance.PlayHurtSound();
        }
        else
        {
            UIManager.Instance.OnNotifyEnemyAttack();
            PlayerDeath();
        }
    }

    public void OnNotifyTakeItem(ItemController itemController)
    {
        if (itemController.itemType == Item.ItemType.HealUp)
        {
            HP += itemController.itemEffectHP;
            UIManager.Instance.OnNotifyTakeHealUp();
            ItemSoundManager.Instance.PlayHealUpSound();
        }
        if (HP > maxHP) HP = maxHP;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.StopAttack();
        }
    }

    private void PlayerDeath()
    {
        playerCollider.enabled = false;
        gun.SetActive(false);
        grenade.SetActive(false);
        animator.SetTrigger("death");

        PlayerSoundManager.Instance.PlayDeathSound();
        AudioManager.Instance.PlayThemeDeathSound();
        UIManager.Instance.OnPlayerDeath();

    }
}
