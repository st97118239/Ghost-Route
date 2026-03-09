using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource voicelineSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

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

    private void Awake()
    {
        instance = this;
        
        SetVolumes();
    }

    public void PlayButtonClick() => PlaySound(Sounds.Click);

    public static void PlayVoiceline(AudioClip audioClip)
    {
        if (instance.voicelineSource.isPlaying)
            instance.voicelineSource.Stop();

        instance.voicelineSource.clip = audioClip;
        instance.voicelineSource.Play();
    }

    public static void PlaySound(Sounds sound)
    {
        AudioClip clipToPlay;
        switch (sound)
        {
            default:
            case Sounds.None:
                return;
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
        }

        if (clipToPlay == null)
        {
            Debug.LogWarning("Audio clip for " + sound + " is null");
            return;
        }

        AudioSource source = sound == Sounds.Music ? instance.musicSource : instance.sfxSource;

        if (source == null)
        {
            Debug.LogError("AudioSource does not exist");
            return;
        }

        source.PlayOneShot(clipToPlay);
    }

    public static void SetVolumes()
    {
        if (instance == null) return;

        if (instance.voicelineSource)
            instance.voicelineSource.volume = SaveData.voicelinesVolume;
        if (instance.sfxSource)
            instance.sfxSource.volume = SaveData.sfxVolume;
        if (instance.musicSource)
            instance.musicSource.volume = SaveData.bgmVolume;
    }
}
