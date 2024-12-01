using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPooling : MonoBehaviour
{
    public static EffectPooling Instance { get; set; }

    public ParticleSystem bloodHitEffect;
    private Queue<ParticleSystem> bloodHitEffectQueue = new Queue<ParticleSystem>();
    private int bloodHitEffectQueueSize = 50;

    public ParticleSystem bloodExplosionEffect;
    private Queue<ParticleSystem> bloodExplosionEffectQueue = new Queue<ParticleSystem>();
    private int bloodExplosionEffectQueueSize = 10;

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

    // Start is called before the first frame update
    void Start()
    {
        InitializeBloodHitEffect();
        InitializeBloodExplosionEffect();
    }

    private void InitializeBloodHitEffect()
    {
        for (int i = 0; i < bloodHitEffectQueueSize; i++)
        {
            ParticleSystem effect = Instantiate(bloodHitEffect, transform.position, Quaternion.identity);
            effect.gameObject.SetActive(false);
            bloodHitEffectQueue.Enqueue(effect);
        }
    }

    public ParticleSystem GetBloodHitEffect()
    {
        if (bloodHitEffectQueue.Count > 0)
        {
            ParticleSystem effect = bloodHitEffectQueue.Dequeue();

            return effect;
        }
        else
        {
            ParticleSystem effect = Instantiate(bloodHitEffect, transform.position, Quaternion.identity);
            effect.gameObject.SetActive(false);
            Debug.Log("new effect");
            bloodHitEffectQueueSize++;
            return effect;
        }
    }

    public void ReturnBloodHitEffect(ParticleSystem effect)
    {
        effect.gameObject.SetActive(false);
        bloodHitEffectQueue.Enqueue(effect);
    }

    private void InitializeBloodExplosionEffect()
    {
        for (int i = 0; i < bloodExplosionEffectQueueSize; i++)
        {
            ParticleSystem effect = Instantiate(bloodExplosionEffect, transform.position, Quaternion.identity);
            effect.gameObject.SetActive(false);
            bloodExplosionEffectQueue.Enqueue(effect);
        }
    }

    public ParticleSystem GetBloodExplosionEffect()
    {
        if (bloodExplosionEffectQueue.Count > 0)
        {
            ParticleSystem effect = bloodExplosionEffectQueue.Dequeue();

            return effect;
        }
        else
        {
            ParticleSystem effect = Instantiate(bloodExplosionEffect, transform.position, Quaternion.identity);
            effect.gameObject.SetActive(false);

            bloodExplosionEffectQueueSize++;

            return effect;
        }
    }

    public void ReturnBloodExplosionEffect(ParticleSystem effect)
    {
        effect.gameObject.SetActive(false);
        bloodExplosionEffectQueue.Enqueue(effect);
    }
}
