using GunSpace;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemySpace;

public class EffectManager : MonoBehaviour, IObserverGun, IObserverGrenade, IObserverItem
{
    public static EffectManager Instance { get; set; }

    private ParticleSystem muzzleFlashEffect;
    private ParticleSystem explosionGrenadeEffect;
    private Vector3 explosionGrenadeEffectOffset;

    private Vector3 bloodHitEffectScale = new Vector3(0.5f, 0.5f, 0.5f);


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
        explosionGrenadeEffect = Instantiate(Resources.Load<ParticleSystem>("Prefabs/Effect/ExplosionGrenade"));

        explosionGrenadeEffectOffset = new Vector3(0f, 1.5f, 0f);
    }

    private void OnEnable()
    {
        GunController.OnGunShoot += OnNotifyGunShoot;
        GunController.OnHitShoot += OnNotifyHitShoot;
        GrenadeController.OnGrenadeExplode += OnNotifyGrenadeExplode;
        ItemController.OnTakeItem += OnNotifyTakeItem;
    }

    private void OnDisable()
    {
        GunController.OnGunShoot -= OnNotifyGunShoot;
        GunController.OnHitShoot -= OnNotifyHitShoot;
        GrenadeController.OnGrenadeExplode -= OnNotifyGrenadeExplode;
        ItemController.OnTakeItem -= OnNotifyTakeItem;
    }

    public void UpdateMuzzleFlashEffect(ParticleSystem effect)
    {
        muzzleFlashEffect = effect;
    }

    #region || --- Observer Method --- ||
    public void OnNotifyGunShoot(GunController gunController)
    {
        if (gunController.isShooting)
        {
            muzzleFlashEffect = gunController.muzzleFlashEffect;
            PlayEffect(muzzleFlashEffect);
            muzzleFlashEffect.transform.GetChild(0).gameObject.SetActive(true);
            Invoke("TurnOffLight", 0.05f);
        }
    }

    public void OnNotifyHitShoot(EnemyController enemy, Vector3 hitPos, int damage, bool isCrit)
    {
        ParticleSystem bloodHitEffect = EffectPooling.Instance.GetBloodHitEffect();
        bloodHitEffect.transform.position = hitPos;
        bloodHitEffect.gameObject.SetActive(true);
        bloodHitEffect.transform.localScale = bloodHitEffectScale;
        PlayEffect(bloodHitEffect);

        ReturnToPool(bloodHitEffect);
    }

    public void OnNotifyGrenadeExplode(GrenadeController grenade)
    {
        explosionGrenadeEffect.transform.position = grenade.transform.position + explosionGrenadeEffectOffset;

        PlayEffect(explosionGrenadeEffect);
    }

    public void OnNotifyTakeItem(ItemController itemController)
    {
        PlayEffect(itemController.effect);
    }
    #endregion

    public async void EnemyDeathByGrenade(EnemyController enemy)
    {
        ParticleSystem effect_1 = EffectPooling.Instance.GetBloodHitEffect();
        ParticleSystem effect_2 = EffectPooling.Instance.GetBloodExplosionEffect();

        var mainModule = effect_1.main;
        mainModule.startSize = 2;
        effect_1.transform.position = enemy.transform.position + new Vector3(0f, 1f, 0f);
        effect_2.transform.position = enemy.transform.position + new Vector3(0f, 1.1f, 0f);

        effect_1.gameObject.SetActive(true);
        PlayEffect(effect_1);
        effect_2.gameObject.SetActive(true);
        PlayEffect(effect_2);
    
        await UniTask.Delay((int)(1000 * 1f));

        EffectPooling.Instance.ReturnBloodHitEffect(effect_1);
        EffectPooling.Instance.ReturnBloodExplosionEffect(effect_2);
    }

    private async void ReturnToPool(ParticleSystem bloodHitEffect)
    {
        await UniTask.WaitForSeconds(0.5f);
        EffectPooling.Instance.ReturnBloodHitEffect(bloodHitEffect);
    }

    private void TurnOffLight()
    {
        muzzleFlashEffect.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void PlayEffect(ParticleSystem effect)
    {
        effect.Play();
    }
}