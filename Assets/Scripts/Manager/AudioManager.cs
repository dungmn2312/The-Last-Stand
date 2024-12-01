using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }

    [Header("--- Theme ---")]
    public AudioSource themeSources;
    public List<AudioClip> themeClips;

    [Header("--- SFX ---")]
    public List<AudioSource> sfxSources;

    public AudioClip deathSound;

    [Header("--- Sound Button ---")]
    public Button musicBtn;
    public Button sfxBtn;

    [Header("--- Sound Slider ---")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private float pressedColor = 100f / 255f;
    private float normalColor = 1f;

    private bool isSelectedMusicBtn = false, isSelectedSFXBtn = false;
    
    private float prevMusicVal, prevSFXVal;

    public float maxPlayerVolume, maxItemVolume, maxGrenadeVolume, maxEnemyVolume, maxGunVolume;

    private float adjustSFXRate;

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
        PlayThemeSound(GameManager.Instance.currentLevel);
    }

    public void ToggleMusic()
    {
        if (!isSelectedMusicBtn)    prevMusicVal = musicSlider.value;
        isSelectedMusicBtn = !isSelectedMusicBtn;
        musicSlider.value = isSelectedMusicBtn ? 0f : prevMusicVal;
    }

    public void ToggleSFX()
    {
        if (!isSelectedSFXBtn) prevSFXVal = sfxSlider.value;
        isSelectedSFXBtn = !isSelectedSFXBtn;
        sfxSlider.value = isSelectedSFXBtn ? 0f : prevSFXVal;
    }

    private void ToggleButton(Button btn, float alpha)
    {
        ColorBlock colorBlock = btn.colors;
        Color color = colorBlock.normalColor;
        color.a = alpha;
        colorBlock.normalColor = color;

        btn.colors = colorBlock;
    }

    public void AdjustMusicVolume()
    {
        themeSources.volume = musicSlider.value;

        if (musicSlider.value == 0) 
            ToggleButton(musicBtn, pressedColor);
        else if (musicSlider.value != 0 && musicBtn.colors.normalColor.a == pressedColor)
            ToggleButton(musicBtn, normalColor);
    }

    public void AdjustSFXVolume()
    {
        adjustSFXRate = sfxSlider.value == 0f ? 0f : (1f / sfxSlider.value);
        if (adjustSFXRate == 0f) Debug.Log("0000");
        
        if (sfxSources.Count != 0) SetSFXVolume();

        if (sfxSlider.value == 0)
            ToggleButton(sfxBtn, pressedColor);
        else if (sfxSlider.value != 0 && sfxBtn.colors.normalColor.a == pressedColor)
            ToggleButton(sfxBtn, normalColor);
    }

    public void PlayThemeSound(int index)
    {
        StopThemeSound();
        themeSources.PlayOneShot(themeClips[index]);
    }

    public void PlayThemeDeathSound()
    {
        StopThemeSound();
        themeSources.PlayOneShot(deathSound);
    }

    public void StopThemeSound()
    {
        themeSources.Stop();
    }

    public void AddSFXSound(AudioSource source)
    {
        sfxSources.Add(source);
    }

    public void RemoveSFXSound(AudioSource source)
    {
        sfxSources.Remove(source);
    }

    public void FindSFXSound()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("SFX");

        foreach (GameObject gameObject in gameObjects)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {

                AddSFXSound(audioSource);
            }
        }

        AdjustSFXVolume();
    }

    private void SetSFXVolume()
    {

        foreach(AudioSource audioSource in sfxSources)
        {
            switch (LayerMask.LayerToName(audioSource.gameObject.layer))
            {
                case ("Enemy"):
                    SetVolume(audioSource, maxEnemyVolume);

                    break;

                case ("Player"):
                    SetVolume(audioSource, maxPlayerVolume);

                    break;
                case ("Gun"):
                    SetVolume(audioSource, maxGunVolume);

                    break;
                case ("Grenade"):
                    SetVolume(audioSource, maxGrenadeVolume);

                    break;
                case ("Item"):
                    SetVolume(audioSource, maxItemVolume);

                    break;
            }

        }
    }

    public void SetVolume(AudioSource source, float maxVolume)
    {
        float value = adjustSFXRate == 0f ? 0f : maxVolume / adjustSFXRate;
        source.volume = value;
    }
}
