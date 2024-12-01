using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeSoundManager : MonoBehaviour
{
    public static GrenadeSoundManager Instance { get; set; }

    public AudioSource grenadeAudioSource;

    public AudioClip grenadeAudioClip;

    public float maxVolume = 1f;

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

    public void PlayGrenadeExplosionSound()
    {
        grenadeAudioSource.PlayOneShot(grenadeAudioClip);
    }
}
