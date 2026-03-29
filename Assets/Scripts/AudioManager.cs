using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource voicelineSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopingSfxSource;

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
    private bool isFadingOut;
    private float fadeVolumeToAdd;
    private const int fadeInStepAmt = 20;
    private const int fadeOutStepAmt = 10;
    private Sounds musicPlaying;

    private void Awake()
    {
        instance = this;
        
        SetVolumes();
    }

    public void PlayButtonClick() => PlaySound(Sounds.Click, false);

    public void PlayShoot() => PlaySound(Sounds.Click, true);


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
        AudioClip clipToPlay = GetSoundClip(sound);

        if (clipToPlay == null)
        {
            Debug.LogWarning("Audio clip for " + sound + " is null");
            return -1f;
        }

        AudioSource source = instance.sfxSource;

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

    public static float PlayLoopingSound(Sounds sound)
    {
        AudioClip clipToPlay = GetSoundClip(sound);

        if (clipToPlay == null)
        {
            Debug.LogWarning("Audio clip for " + sound + " is null");
            return -1f;
        }

        AudioSource source = instance.loopingSfxSource;

        if (source == null)
        {
            Debug.LogError("AudioSource does not exist");
            return -1f;
        }

        if (source.isPlaying)
            source.Stop();

        source.clip = clipToPlay;
        source.Play();

        return clipToPlay.length;
    }

    private static AudioClip GetSoundClip(Sounds sound)
    {
        switch (sound)
        {
            default:
                return null;
            case Sounds.Click:
                return instance.clickClip;
            case Sounds.Dialogue:
                return instance.dialogueClip;
            case Sounds.MemoryPoint:
                return instance.memoryPointClip;
            case Sounds.Shoot:
                return instance.shootClip;
            case Sounds.Hit:
                return instance.hitClip;
            case Sounds.Wrong:
                return instance.wrongClip;
            case Sounds.SpawnGhost:
                return instance.spawnGhostClip;
            case Sounds.SpawnBunny:
                return instance.spawnBunnyClip;
            case Sounds.SpawnDeer:
                return instance.spawnDeerClip;
            case Sounds.Jump:
                return instance.jumpClip;
            case Sounds.Damage:
                return instance.damageClip;
            case Sounds.Fall:
                return instance.fallClip;
            case Sounds.Footsteps:
                return instance.footstepClip;
            case Sounds.FootstepsFadeOut:
                return instance.footstepFadeOutClip;
            case Sounds.FootstepsFadeIn:
                return instance.footstepFadeInClip;
            case Sounds.KeyJingle:
                return instance.keyJingleClip;
            case Sounds.DoorUnlocking:
                return instance.doorUnlockingClip;
            case Sounds.CarCrash:
                return instance.carCrashClip;
        }
    }

    public static void StopLoopingSFX()
    {
        instance.loopingSfxSource.Stop();
    }

    public static void FadeMusicIn(Sounds music)
    {
        if (instance.musicPlaying == music)
            return;

        instance.musicPlaying = music;

        if (instance.fadeCoroutine != null)
            instance.StopCoroutine(instance.fadeCoroutine);
        instance.fadeCoroutine = instance.StartCoroutine(instance.FadeMusicInCoroutine(music));
    }

    private IEnumerator FadeMusicInCoroutine(Sounds music)
    {
        isFadingOut = false;
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

        fadeVolumeToAdd = SaveDataManager.saveData.bgmVolume / fadeInStepAmt;
        WaitForSeconds delay = new(0.3f);

        for (int i = 0; i < fadeInStepAmt; i++)
        {
            yield return delay;

            musicSource.volume += fadeVolumeToAdd;

            if (!(musicSource.volume > SaveDataManager.saveData.bgmVolume)) continue;
            musicSource.volume = SaveDataManager.saveData.bgmVolume;
            yield break;
        }

        musicSource.volume = SaveDataManager.saveData.bgmVolume;
    }

    public static void FadeMusicOut()
    {
        if (instance.musicPlaying == Sounds.None)
            return;

        if (instance.fadeCoroutine != null)
            instance.StopCoroutine(instance.fadeCoroutine);
        instance.fadeCoroutine = instance.StartCoroutine(instance.FadeMusicOutCoroutine());
    }

    private IEnumerator FadeMusicOutCoroutine()
    {
        isFadingOut = true;
        fadeVolumeToAdd = musicSource.volume / fadeOutStepAmt;
        WaitForSeconds delay = new(0.1f);

        for (int i = 0; i < fadeOutStepAmt; i++)
        {
            yield return delay;

            musicSource.volume -= fadeVolumeToAdd;
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
        if (instance.loopingSfxSource)
            instance.loopingSfxSource.volume = SaveDataManager.saveData.sfxVolume;
        if (!instance.musicSource) return;
        instance.musicSource.volume = SaveDataManager.saveData.bgmVolume;

        if (instance.fadeCoroutine == null) return;
        if (SaveDataManager.saveData.bgmVolume == 0)
            instance.StopCoroutine(instance.fadeCoroutine);
        else if (instance.isFadingOut)
            instance.fadeVolumeToAdd = instance.musicSource.volume / fadeOutStepAmt;
        else
            instance.fadeVolumeToAdd = instance.musicSource.volume / fadeInStepAmt;
    }
}
