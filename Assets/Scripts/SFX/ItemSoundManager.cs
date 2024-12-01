using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSoundManager : MonoBehaviour
{
    public static ItemSoundManager Instance { get; set; }

    public AudioSource itemAudioSource;

    public AudioClip healUpSound;
    public AudioClip itemSound;

    public float maxVolume = 0.5f;

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

    public void PlayHealUpSound()
    {
        itemAudioSource.PlayOneShot(healUpSound);
    }

    public void PlayItemSound()
    {
        itemAudioSource.PlayOneShot(itemSound);
    }
}
