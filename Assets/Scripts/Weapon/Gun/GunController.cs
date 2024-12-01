using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using EnemySpace;
using System.Runtime.CompilerServices;

namespace GunSpace
{
    public class GunController : Gun, IObserverItem
    {
        public delegate void GunShootHandler(GunController gunController);
        public static event GunShootHandler OnGunShoot;

        public delegate void HitShootHandler(EnemyController enemy, Vector3 hitPos, int damage, bool isCrit);
        public static event HitShootHandler OnHitShoot;


        public GunType gunType;
        public GunName gunName;
        public FireMod fireMod;

        public Transform gunIdleTransform;
        public Transform gunAimTransform;
        public Transform bulletSpawnPos;
        public GameObject gunRes;

        public ParticleSystem muzzleFlashEffect;

        public bool isAiming;
        public bool isShooting;
        public bool isReloading;
        private bool isCrit;

        internal Animator animator;
        private int animPistolIdle = Animator.StringToHash("pistol_idle");
        private int animRifleIdle = Animator.StringToHash("rifle_idle");

        public float shootTransformTime;
        public Vector3 shootCameraPos;
        public int currentAmmo, totalAmmo;

        public IGunState idleState, aimState, shootState;
        public IGunState currentState;

        private float bulletRange = 20f;
        private float bulletSpeed = 70f;

        [SerializeField] private LayerMask enemyLayer;

        private void Awake()
        {
            animator = AnimatorManager.Instance.playerAnimator;
            
            idleState = new IdleState(this);
            aimState = new AimState(this);
        }

        // Start is called before the first frame update
        void Start()
        {

            //input = InputManager.Instance.input;
            UIManager.Instance.UpdateAmmo(currentAmmo, ref totalAmmo);
        }

        private void OnEnable()
        {
            ItemController.OnTakeItem += OnNotifyTakeItem;

            currentState = idleState;
            idleState.EnterState();

            if (gunType == GunType.Pistol)
            {
                if (animator.GetBool(animRifleIdle))
                {
                    animator.SetBool(animRifleIdle, false);
                    animator.SetBool(animPistolIdle, true);
                }
            }
            else
            {
                if (animator.GetBool(animPistolIdle))
                {
                    animator.SetBool(animPistolIdle, false);
                    animator.SetBool(animRifleIdle, true);
                }
            }

            EffectManager.Instance.UpdateMuzzleFlashEffect(muzzleFlashEffect);
        }

        private void OnDisable()
        {
            ItemController.OnTakeItem -= OnNotifyTakeItem;
        }

        private void Update()
        {
            currentState.UpdateState();
        }

        public void SwitchGun(int gunIndex)
        {
            if (gunIndex == GunManager.Instance.GetCurrentGunIndex() + 1) return;

            GunSoundManager.Instance.PlaySwitchGunSound();
            gunRes.SetActive(true);
            gameObject.SetActive(false);
            GunController gun = GunManager.Instance.gunList[gunIndex - 1].GetComponent<GunController>();
            gun.gameObject.SetActive(true);

            UIManager.Instance.SwitchGun(gun.currentAmmo, gun.totalAmmo);
        }

        public override async void Reload()
        {
            isReloading = true;

            await UniTask.WaitForSeconds(reloadTime);

            CaculateAmmoReload();
            UIManager.Instance.UpdateAmmo(currentAmmo, ref totalAmmo);

            isReloading = false;
        }

        private void CaculateAmmoReload()
        {
            if (totalAmmo >= ammoPerMag)
            {
                totalAmmo = totalAmmo - (ammoPerMag - currentAmmo);
                currentAmmo = ammoPerMag;
            }
            else
            {
                if (totalAmmo + currentAmmo >= ammoPerMag)
                {
                    totalAmmo = (totalAmmo + currentAmmo) - ammoPerMag;
                    currentAmmo = ammoPerMag;
                }
                else
                {
                    currentAmmo = currentAmmo + totalAmmo;
                    totalAmmo = 0;
                }
            }
        }

        public override async void Shoot()
        {
            if (currentAmmo > 0 && !isShooting)
            {
                isShooting = true;

                PlayShootSound();

                GameObject bullet = BulletPooling.Instance.GetBullet();
                bullet.transform.position = bulletSpawnPos.position;
                bullet.transform.rotation = bulletSpawnPos.rotation;
                bullet.gameObject.SetActive(true);

                currentAmmo--;

                OnGunShoot?.Invoke(this);

                RaycastHit hit;
                if (Physics.Raycast(bulletSpawnPos.position, bulletSpawnPos.transform.forward, out hit, bulletRange, enemyLayer))
                {
                    Vector3 targetPos = hit.point;
                    float distance = Vector3.Distance(bullet.transform.position, targetPos);
                    float time = distance / bulletSpeed;

                    LeanTween.move(bullet.gameObject, targetPos, time)
                        .setOnComplete(() =>
                        {
                            BulletPooling.Instance.ReturnBullet(bullet);

                            EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                            if (enemy != null)
                            {
                                isCrit = CheckCriticalDamage();
                                OnHitShoot?.Invoke(enemy, targetPos, damage, isCrit);
                            }
                        });

                }
                else
                {
                    Vector3 targetPos = bulletSpawnPos.position + bulletSpawnPos.transform.forward * bulletRange;

                    float distance = Vector3.Distance(bullet.transform.position, targetPos);
                    float time = distance / bulletSpeed;

                    LeanTween.move(bullet.gameObject, targetPos, time)
                        .setOnComplete(() =>
                        {
                            BulletPooling.Instance.ReturnBullet(bullet);
                        });
                }
                await UniTask.Delay((int)(fireSpeed * 1000));

                isShooting = false;

            }
        }

        public void PlayShootSound()
        {
            switch (gunName)
            {
                case (GunName.Barreta):
                    GunSoundManager.Instance.PlayBarretaShootSound();
                    break;

                case (GunName.Deagle):
                    GunSoundManager.Instance.PlayDeagleShootSound();
                    break;

                case (GunName.M4A1):
                    GunSoundManager.Instance.PlayM4A1ShootSound();
                    break;
            }
        }


        public void PlayReloadSound()
        {
            switch (gunType)
            {
                case (GunType.Pistol):
                    GunSoundManager.Instance.PlayPistolReloadSound();
                    break;

                case (GunType.Rifle):
                    GunSoundManager.Instance.PlayRifleReloadSound();
                    break;
            }
        }

        private bool CheckCriticalDamage()
        {
            int random = UnityEngine.Random.Range(0, 5);
            return random == 3;
        }

        public void ChangeState(IGunState newState)
        {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        public void OnNotifyTakeItem(ItemController itemController)
        {
            if (itemController.itemType == Item.ItemType.Ammo)
            {
                ItemSoundManager.Instance.PlayItemSound();
                GunController gun = GunManager.Instance.gunList[1].GetComponent<GunController>();
                gun.totalAmmo += itemController.itemEffectAmmo;
                if (gun.gameObject.activeSelf) UIManager.Instance.UpdateAmmo(gun.currentAmmo, ref gun.totalAmmo);
            }
        }

    }
}


