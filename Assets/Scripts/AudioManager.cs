using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource voicelineSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource dialogueSfxSource;

    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip dialogueClip;
    [SerializeField] private AudioClip endingClip;
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
                return -1f;
            case Sounds.Music:
                clipToPlay = instance.musicClip;
                break;
            case Sounds.Click:
                clipToPlay = instance.clickClip;
                break;
            case Sounds.Dialogue:
                clipToPlay = instance.dialogueClip;
                break;
            case Sounds.Ending:
                clipToPlay = instance.endingClip;
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
