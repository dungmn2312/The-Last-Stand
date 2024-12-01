using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{

    public AudioSource enemyAudioSource;

    public AudioClip explosionSound;

    public List<AudioClip> chaseSound;
    public List<AudioClip> attackSound;
    public List<AudioClip> dieSound;

    private float delay = 1f;
    private int index;
    private int chaseSoundCount, attackSoundCount, dieSoundCound;

    private bool chaseWait = false, attackWait = false;

    private float soundVolume;

    public float maxVolume = 0.5f;

    private void Start()
    {
        chaseSoundCount = chaseSound.Count;
        attackSoundCount = attackSound.Count;
        dieSoundCound = dieSound.Count;

        soundVolume = enemyAudioSource.volume;
    }

    public async void PlayExplosionSound()
    {
        enemyAudioSource.volume = 1f;
        enemyAudioSource.PlayOneShot(explosionSound);

        await UniTask.WaitForSeconds(explosionSound.length);

        enemyAudioSource.volume = soundVolume;
    }

    public void PlayChaseSound()
    {
        if (!chaseWait)
        {
            chaseWait = true;
            PlaySound(chaseSound);

        }

    }

    public void PlayAttackSound()
    {
        if (!attackWait)
        {
            attackWait = true;
            PlaySound(attackSound);
        }
    }

    public void PlayDieSound()
    {
        PlaySound(dieSound);
    }

    private async void PlaySound(List<AudioClip> list)
    {
        index = Random.Range(0, list.Count);
        enemyAudioSource.PlayOneShot(list[index]);

        if (list == chaseSound)
        {
            await UniTask.WaitForSeconds(delay + list[index].length);
            chaseWait = false;
        }
        else if (list == attackSound)
        {
            await UniTask.WaitForSeconds(list[index].length);
            attackWait = false;
        }
    }

}
