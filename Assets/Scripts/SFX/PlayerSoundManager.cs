using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{

    public static PlayerSoundManager Instance { get; set; }

    public AudioSource playerAudioSource;
    public List<AudioClip> walkSound;

    public AudioClip rollSound;

    public List<AudioClip> hurtSound;
    public List<AudioClip> deathSound;

    private int index;

    private int walkSoundCount, hurtSoundCount, deathSoundCount;

    public float maxVolume = 0.25f;

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
        walkSoundCount = walkSound.Count;
        deathSoundCount = deathSound.Count;
        hurtSoundCount = hurtSound.Count;
    }

    public void PlayWalkSound()
    {
        index = Random.Range(0, walkSoundCount);
        playerAudioSource.PlayOneShot(walkSound[index]);
    }

    public void PlayRollSound()
    {
        playerAudioSource.PlayOneShot(rollSound);
    }

    public void PlayHurtSound()
    {
        index = Random.Range(0, hurtSoundCount);
        playerAudioSource.PlayOneShot(hurtSound[index]);
    }

    public void PlayDeathSound()
    {
        index = Random.Range(0, deathSoundCount);
        playerAudioSource.PlayOneShot(deathSound[index]);
    }
}
