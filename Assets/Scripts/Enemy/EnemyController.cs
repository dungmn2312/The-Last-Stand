using Cysharp.Threading.Tasks;
using GunSpace;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace EnemySpace
{
    public class EnemyController : Enemy, IObserverGrenade
    {
        private float damageTaked;

        internal Animator animator;
        private CapsuleCollider enemyCollider;
        internal NavMeshAgent agent;

        private GameObject player;

        [Header("--- SOUND ---")]
        public EnemySoundManager sound;

        public GameObject audioSourceSound;

        [Header("--- UI ---")]
        public Slider HPSLider;
        public Slider EaseHPSlider;
        public Canvas HPCanvas;

        public EnemyAttackType attackType;
        public GameObject attackPoint;

        public bool isAttacking;

        private bool isExploded;

        private GameObject playerCamera;

        public delegate void EnemyAttackHandler(float damage);
        public static event EnemyAttackHandler OnEnemyAttack;

        internal int animDeath1, animDeath2;
        internal int animScratch, animSmite, animExplode;
        internal int animHit, animRun;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            enemyCollider = GetComponent<CapsuleCollider>();
            player = GameObject.FindWithTag("Player");
            playerCamera = GameObject.FindWithTag("MainCamera");
        }

        private void Start()
        {
            AssignAnimationIDs();

            speed = agent.speed;

            HPSLider.maxValue = HP;
            HPSLider.value = HP;
            EaseHPSlider.maxValue = HP;
            EaseHPSlider.value = HP;

        }

        private void AssignAnimationIDs()
        {
            animDeath1 = Animator.StringToHash("enemy_death1");
            animDeath2 = Animator.StringToHash("enemy_death2");
            animScratch = Animator.StringToHash("enemy_scratch");
            animSmite = Animator.StringToHash("enemy_smite");
            animExplode = Animator.StringToHash("enemy_explode");
            animHit = Animator.StringToHash("enemy_hit");
            animRun = Animator.StringToHash("enemy_run");

        }

        private void OnEnable()
        {
            if (!enemyCollider.gameObject.activeSelf)   enemyCollider.gameObject.SetActive(true);
            HPCanvas.gameObject.SetActive(true);

            GrenadeController.OnGrenadeExplode += OnNotifyGrenadeExplode;
            GunController.OnHitShoot += OnNotifyHitShoot;
        }

        private void OnDisable()
        {
            GrenadeController.OnGrenadeExplode -= OnNotifyGrenadeExplode;
            GunController.OnHitShoot -= OnNotifyHitShoot;
        }

        private void Update()
        {
            if (HP > 0)
            {
                HPCanvas.transform.LookAt(transform.position + playerCamera.transform.forward);
                transform.LookAt(player.transform.position);
            }
        }
        #region || --- Observer Method --- ||
        public void OnNotifyGrenadeExplode(GrenadeController grenade)
        {
            foreach (Collider collider in grenade.enemyCollider)
            {
                if (collider == enemyCollider)
                {
                    damageTaked = grenade.damage;
                    TakeDamage(true, false);
                    break;
                }
            }
        }

        public void OnNotifyHitShoot(EnemyController enemy, Vector3 hitPos, int damage, bool isCrit)
        {
            if (enemy.enemyCollider == enemyCollider)
            {
                damageTaked = isCrit ? damage * 2 : damage;
                TakeDamage(false, isCrit);
            }
        }
        #endregion

        public override void TakeDamage(bool isGrenadeDamage, bool isCrit)
        {
            if (HP > 0)
            {
                if (isCrit) animator.SetTrigger(animHit);
                HP -= damageTaked;
            }

            if (HP <= 0)
            {
                EnemyDeath(isGrenadeDamage);
            }
        }

        public void InflictDamageToPlayer()
        {
            OnEnemyAttack?.Invoke(damage);
        }

        public override void Attack()
        {
            isAttacking = true;
            if (attackPoint != null) attackPoint.gameObject.SetActive(true);
            SetAnimAttack();
            
        }

        public override void StopAttack()
        {
            isAttacking = false;
            if (attackPoint != null) attackPoint.gameObject.SetActive(false);
            SetAnimAttack();
        }

        private async void SetAnimAttack()
        {
            switch (attackType)
            {
                case EnemyAttackType.Normal:
                    animator.SetBool(animScratch, isAttacking);

                    break;

                case EnemyAttackType.Smite:
                    animator.SetBool(animSmite, isAttacking);

                    break;

                case EnemyAttackType.Explode:
                    if (isAttacking)
                    {
                        isExploded = false;
                        animator.SetBool(animExplode, isAttacking);
                        await UniTask.WaitForSeconds(1.7f);
                        if (!isExploded) EnemyDeath(true);

                    }

                    break;
            }
        }

        private void EnemyDeath(bool isExplosiveDeath)
        {
            if (!isExplosiveDeath && (attackType == EnemyAttackType.Normal || attackType == EnemyAttackType.Smite))
            {
                sound.PlayDieSound();

                int deadIndex = Random.Range(1, 3);
                if (deadIndex == 1) animator.SetTrigger(animDeath1);
                else animator.SetTrigger(animDeath2);

                enemyCollider.enabled = false;
                Invoke("ReturnToPool", 2.5f);
            }
            else
            {
                sound.PlayExplosionSound();

                isExploded = true;
                EffectManager.Instance.EnemyDeathByGrenade(this);

                if (attackType == EnemyAttackType.Explode)
                {
                    TakeExplosionDamage();
                }
                enemyCollider.enabled = false;

                ReturnToPool();
            }

            HPCanvas.gameObject.SetActive(false);

            SpawnItem();
            AudioSource audioSource = audioSourceSound.GetComponent<AudioSource>();
            AudioManager.Instance.RemoveSFXSound(audioSource);

            EnemyManager.Instance.UpdateEnemyLeft();
        }

        private void SpawnItem()
        {
            GameObject item = null;
            int itemIndex = Random.Range(0, 3);
            if (itemIndex == 1) item = ItemPooling.Instance.RandomItem();
            else return;

            Vector3 itemPos = item.transform.position;
            item.transform.position = new Vector3(transform.position.x, itemPos.y, transform.position.z);
            item.SetActive(true);
        }

        public void TakeExplosionDamage()
        {
            Collider[] hitObjects = Physics.OverlapSphere(transform.position + new Vector3(0f, 1f, 0f), 3f);
            foreach (Collider hitObject in hitObjects)
            {
                if (!hitObject.isTrigger && hitObject.gameObject.layer == player.layer)
                {
                    InflictDamageToPlayer();
                    break;
                }
            }
        }

        public void ReturnToPool()
        {
            EnemyPooling.Instance.ReturnToPool(gameObject);
        }
    }
}


