using System;
using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource voicelineSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource dialogueSfxSource;

    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip dialogueClip;
    [SerializeField] private AudioClip memoryPointClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip wrongClip;
    [SerializeField] private AudioClip spawnGhostClip;
    [SerializeField] private AudioClip spawnBunnyClip;
    [SerializeField] private AudioClip spawnDeerClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip fallClip;

    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip footstepFadeOutClip;
    [SerializeField] private AudioClip footstepFadeInClip;
    [SerializeField] private AudioClip keyJingleClip;
    [SerializeField] private AudioClip doorUnlockingClip;
    [SerializeField] private AudioClip carCrashClip;

    [SerializeField] private AudioClip mainMusicClip;
    [SerializeField] private AudioClip arcadeMusicClip;
    [SerializeField] private AudioClip cocytusMusicClip;
    [SerializeField] private AudioClip endingMusicClip;
    [SerializeField] private AudioClip memoryLossMusicClip;

    private Coroutine fadeCoroutine;
    private Sounds musicPlaying;

    private void Awake()
    {
        instance = this;
        
        SetVolumes();
    }

    public void PlayButtonClick() => PlaySound(Sounds.Click, false);

    public static float PlayVoiceline(AudioClip audioClip)
    {
        if (instance.voicelineSource.isPlaying)
            instance.voicelineSource.Stop();

        if (audioClip == null)
        {
            Debug.LogError("AudioClip is null.");
            return -1f;
        }

        instance.voicelineSource.clip = audioClip;
        instance.voicelineSource.Play();
        Debug.Log("Playing voice line " + audioClip.name);
        return audioClip.length;
    }

    public static float PlaySound(Sounds sound, bool isOneShot)
    {
        AudioClip clipToPlay;
        switch (sound)
        {
            default:
            case Sounds.None:
            case Sounds.MainMusic:
            case Sounds.ArcadeMusic:
            case Sounds.CocytusMusic:
            case Sounds.Ending:
                return -1f;
            case Sounds.Click:
                clipToPlay = instance.clickClip;
                break;
            case Sounds.Dialogue:
                clipToPlay = instance.dialogueClip;
                break;
            case Sounds.MemoryPoint:
                clipToPlay = instance.memoryPointClip;
                break;
            case Sounds.Shoot:
                clipToPlay = instance.shootClip;
                break;
            case Sounds.Hit:
                clipToPlay = instance.hitClip;
                break;
            case Sounds.Wrong:
                clipToPlay = instance.wrongClip;
                break;
            case Sounds.SpawnGhost:
                clipToPlay = instance.spawnGhostClip;
                break;
            case Sounds.SpawnBunny:
                clipToPlay = instance.spawnBunnyClip;
                break;
            case Sounds.SpawnDeer:
                clipToPlay = instance.spawnDeerClip;
                break;
            case Sounds.Jump:
                clipToPlay = instance.jumpClip;
                break;
            case Sounds.Damage:
                clipToPlay = instance.damageClip;
                break;
            case Sounds.Fall:
                clipToPlay = instance.fallClip;
                break;
            case Sounds.Footsteps:
                clipToPlay = instance.footstepClip;
                break;
            case Sounds.FootstepsFadeOut:
                clipToPlay = instance.footstepFadeOutClip;
                break;
            case Sounds.FootstepsFadeIn:
                clipToPlay = instance.footstepFadeInClip;
                break;
            case Sounds.KeyJingle:
                clipToPlay = instance.keyJingleClip;
                break;
            case Sounds.DoorUnlocking:
                clipToPlay = instance.doorUnlockingClip;
                break;
            case Sounds.CarCrash:
                clipToPlay = instance.carCrashClip;
                break;
        }

        if (clipToPlay == null)
        {
            Debug.LogWarning("Audio clip for " + sound + " is null");
            return -1f;
        }

        AudioSource source = isOneShot ? instance.dialogueSfxSource : instance.sfxSource;

        if (source == null)
        {
            Debug.LogError("AudioSource does not exist");
            return -1f;
        }

        if (!isOneShot)
        {
            if (source.isPlaying)
                source.Stop();

            source.clip = clipToPlay;
            source.Play();
        }
        else
            source.PlayOneShot(clipToPlay);

        return clipToPlay.length;
    }

    public static void StopLoopingSFX()
    {
        instance.dialogueSfxSource.Stop();
    }

    public static void FadeMusicIn(Sounds music)
    {
        if (instance.musicPlaying == music)
            return;

        instance.musicPlaying = music;

        float musicVolume = SaveDataManager.saveData.bgmVolume;

        if (instance.fadeCoroutine != null)
            instance.StopCoroutine(instance.fadeCoroutine);
        instance.fadeCoroutine = instance.StartCoroutine(instance.FadeMusicInCoroutine(musicVolume, music));
    }

    private IEnumerator FadeMusicInCoroutine(float musicVolume, Sounds music)
    {
        musicSource.volume = 0;

        switch (music)
        {
            default:
                yield break;
            case Sounds.MainMusic:
                musicSource.clip = mainMusicClip;
                break;
            case Sounds.ArcadeMusic:
                musicSource.clip = arcadeMusicClip;
                break;
            case Sounds.CocytusMusic:
                musicSource.clip = cocytusMusicClip;
                break;
            case Sounds.Ending:
                musicSource.clip = endingMusicClip;
                break;
            case Sounds.MemoryLossMusic:
                musicSource.clip = memoryLossMusicClip;
                break;
        }

        musicSource.Play();

        const float addTimes = 20;
        float musicAmtToAdd = musicVolume / addTimes;
        WaitForSeconds delay = new(0.3f);

        for (int i = 0; i < addTimes; i++)
        {
            yield return delay;

            musicSource.volume += musicAmtToAdd;
        }

        musicSource.volume = musicVolume;
    }

    public static void FadeMusicOut()
    {
        if (instance.musicPlaying == Sounds.None)
            return;

        float musicVolume = SaveDataManager.saveData.bgmVolume;

        if (instance.fadeCoroutine != null)
            instance.StopCoroutine(instance.fadeCoroutine);
        instance.fadeCoroutine = instance.StartCoroutine(instance.FadeMusicOutCoroutine(musicVolume));
    }

    private IEnumerator FadeMusicOutCoroutine(float musicVolume)
    {
        musicSource.volume = musicVolume;
        const float addTimes = 10;
        float musicAmtToAdd = musicVolume / addTimes;
        WaitForSeconds delay = new(0.1f);

        for (int i = 0; i < addTimes; i++)
        {
            yield return delay;

            musicSource.volume -= musicAmtToAdd;
        }

        musicSource.volume = 0;
        musicSource.Stop();

        musicPlaying = Sounds.None;
    }

    public static void SetVolumes()
    {
        if (instance == null) return;

        if (instance.voicelineSource)
            instance.voicelineSource.volume = SaveDataManager.saveData.voicelinesVolume;
        if (instance.sfxSource)
            instance.sfxSource.volume = SaveDataManager.saveData.sfxVolume;
        if (instance.dialogueSfxSource)
            instance.dialogueSfxSource.volume = SaveDataManager.saveData.sfxVolume;
        if (instance.musicSource)
            instance.musicSource.volume = SaveDataManager.saveData.bgmVolume;
    }
}
