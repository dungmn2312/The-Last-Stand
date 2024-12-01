using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSoundManager : MonoBehaviour
{
    public static GunSoundManager Instance { get; set; }

    public AudioSource gunAudioSource;

    public List<AudioClip> barretaShootSound;
    public AudioClip deagleShootSound;
    public AudioClip m4a1ShootSound;

    public AudioClip pistolReloadSound;
    public AudioClip rifleReloadSound;

    public AudioClip emptyMagazineSound;

    public AudioClip switchGunSound;

    private int index;

    public float maxVolume = 0.4f;

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

    public void PlayBarretaShootSound()
    {
        int shootSoundCount = barretaShootSound.Count;
        index = Random.Range(0, shootSoundCount);
        gunAudioSource.PlayOneShot(barretaShootSound[index]);
    }
    public void PlayDeagleShootSound()
    {
        gunAudioSource.PlayOneShot(deagleShootSound);
    }

    public void PlayM4A1ShootSound()
    {
        gunAudioSource.PlayOneShot(m4a1ShootSound);
    }

    public void PlayPistolReloadSound()
    {
        gunAudioSource.PlayOneShot(pistolReloadSound);
    }

    public void PlayRifleReloadSound()
    {
        gunAudioSource.PlayOneShot(rifleReloadSound);
    }

    public void PlayEmptyMagazineSound()
    {
        gunAudioSource.PlayOneShot(emptyMagazineSound);
    }

    public void PlaySwitchGunSound()
    {
        gunAudioSource.PlayOneShot(switchGunSound);
    }
}
